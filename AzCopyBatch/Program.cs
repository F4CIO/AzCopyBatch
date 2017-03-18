using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CraftSynth.BuildingBlocks.Common;
using CraftSynth.BuildingBlocks.Logging;

namespace AzCopyBatch
{
	class Program
	{
		static int Main(string[] args)
		{
			int r = 0;

			CustomTraceLog log = new CustomTraceLog("--------------------------------------------------------------------------------------------", true, false, CustomTraceLogAddLinePostProcessingEvent, CustomTraceLogAddLinePreProcessingEvent);
			try
			{
				log.AddLineAndIncreaseIdent("Starting...");

				AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs args1)
				                                              {
					                                              var ex = (Exception) args1.ExceptionObject;
					                                              HandlerForLoging.LogException(ex, log);
					                                              Environment.Exit(1);
				                                              };
				

				List<Process> sameNameProcesses = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).ToList();
				//sameNameProcesses.AddRange(Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName+".vshost").ToList());
				if (sameNameProcesses.Count() > 1)
				{
					log.AddLine("Another instance is allready running. Closing this one...");
				}
				else
				{
					CheckIniFileParameters(log);

					bool allSuccess = true;
					allSuccess = allSuccess && HandlerForTasks.Execute(log);

					log.AddLine(allSuccess?"Success!":"Failure!");
					HandlerForMails.SendReportIfNecessary(allSuccess, log);


					log.DecreaseIdent();
					log.AddLine("Finished with " + (allSuccess ? "success!" : "failure!"));
				}
			}
			catch (Exception exception)
			{
				HandlerForLoging.LogException(exception, log);
				r = -1;
			}

			return r;
		}

		private static void CheckIniFileParameters(CustomTraceLog log)
		{
			log.AddLineAndIncreaseIdent("Checking .ini file...");

			string iniFilePath = CraftSynth.BuildingBlocks.Common.Misc.ApplicationPhysicalExeFilePathWithoutExtension + ".ini";
			log.AddLine("Ini file path: "+iniFilePath);
			
			log.AddLineAndIncreaseIdent("Content:");
			var lines = File.ReadLines(iniFilePath);
			List<string> tasks = null;
			string currentLine;
			foreach (string line in lines)
			{
				currentLine = line;
				if (currentLine.Trim().StartsWith("[Tasks]"))
				{
					tasks = new List<string>();
				}
				else if (tasks != null && currentLine.Trim().Length > 0 && !currentLine.StartsWith("--"))
				{
					tasks.Add(currentLine);
				}

				log.AddLine(currentLine);
			}
			log.DecreaseIdent();

			bool ExecuteNextTaskAfterError = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<bool>("ExecuteNextTaskAfterError", iniFilePath, true, false, true, false, false, false);
			bool SendMailOnSuccess = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<bool>("SendMailOnSuccess", iniFilePath, true, false, true, false, false, false);
			string MailSubjectOnSuccess = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("MailSubjectOnSuccess", iniFilePath, SendMailOnSuccess, null, SendMailOnSuccess, null, false, null);
			bool SendMailOnError = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<bool>("SendMailOnError", iniFilePath, true, false, true, false, false, false);
			string MailSubjectOnError = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("MailSubjectOnError", iniFilePath, SendMailOnSuccess, null, SendMailOnSuccess, null, false, null);
			int TimeoutForEveryTaskInMinutes = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<int>("TimeoutForEveryTaskInMinutes", iniFilePath, true, -1, true, -1, false, -1);
	
			bool SmtpViaOffice365 = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<bool>("SmtpViaOffice365", iniFilePath, true, false, true, false, false, false);
			string SmtpAddress = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("SmtpAddress", iniFilePath, true, null, true, null, false, null);
			int SmtpPort = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<int>("SmtpPort", iniFilePath, false, 25, true, 25, false, -1);
			string SmtpFrom = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("SmtpFrom", iniFilePath, true, null, true, null, false, null);
			string SmtpToCsv = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("SmtpToCsv", iniFilePath, true, null, true, null, false, null);
			string SmtpUsername = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("SmtpUsername", iniFilePath, true, null, true, null, false, null);
			string SmtpPassword = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("SmtpPassword", iniFilePath, true, null, true, null, false, null);
			log.AddLine("Tasks count:"+tasks.Count);

			log.AddLineAndDecreaseIdent("Checking .ini file done.");
		}
		
		private static void CustomTraceLogAddLinePreProcessingEvent(CustomTraceLog sender, ref string line, ref bool inNewLine, ref int level)
		{
			line = HandlerForCommon.HideSensitiveInformation(line);
		}

		private static void CustomTraceLogAddLinePostProcessingEvent(CustomTraceLog sender, string line, bool inNewLine, int level, string lineVersionSuitableForLineEnding, string lineVersionSuitableForNewLine)
		{
			HandlerForLoging.LogAction(line);
			Console.WriteLine(line);
		}
	}
}
