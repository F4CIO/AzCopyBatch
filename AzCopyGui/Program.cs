using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using AzCopyBatch;
using CraftSynth.BuildingBlocks.Logging;

namespace AzCopyGui
{
	static class Program
	{
		private static FormMain _formMain;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main()
		{
			int r = 0;
			//string logFilePath = CraftSynth.BuildingBlocks.Common.Misc.ApplicationPhysicalExeFilePathWithoutExtension + ".log";
			string logFileContent = string.Empty;//File.ReadAllText(logFilePath)
			CustomTraceLog log = new CustomTraceLog(logFileContent, true, false, CustomTraceLogAddLinePostProcessingEvent, CustomTraceLogAddLinePreProcessingEvent);
			log.AddLine("--------------------------------------------------------------------------------------------");
			//try
			//{
				log.AddLineAndIncreaseIdent("Starting...");

				//AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs args1)
				//											  {
				//												  var ex = (Exception) args1.ExceptionObject;
				//												  AzCopyBatch.HandlerForLoging.LogException(ex, log);
				//												  //Environment.Exit(1);
				//												  throw ex;
				//											  };
				

				List<Process> sameNameProcesses = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).ToList();
				//sameNameProcesses.AddRange(Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName+".vshost").ToList());
				if (sameNameProcesses.Count() > 1)
				{
					log.AddLine("Another instance is allready running. Closing this one...");
				}
				else
				{
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					_formMain = new FormMain(log);
		
					Application.Run(_formMain);
			
					log.AddLine("Done...");

					log.DecreaseIdent();
				}
			//}
			//catch (Exception exception)
			//{
			//	HandlerForLoging.LogException(exception, log);
			//	r = -1;
			//	throw;
			//}

			return r;
		}

		private static void CustomTraceLogAddLinePreProcessingEvent(CustomTraceLog sender, ref string line, ref bool inNewLine, ref int level)
		{
			line = HandlerForCommon.HideSensitiveInformation(line);
		}

		private static void CustomTraceLogAddLinePostProcessingEvent(CustomTraceLog sender, string line, bool inNewLine, int level, string lineVersionSuitableForLineEnding, string lineVersionSuitableForNewLine)
		{
			HandlerForLoging.LogAction(line, inNewLine);
			if (_formMain != null && _formMain.Visible)
			{
				if (inNewLine)
				{
					_formMain.AppendTextToTbLog(line);
				}
				else
				{
					_formMain.AppendTextToTbLog(line.Substring(line.IndexOf(')')+1));
				}
			}
		}

	
	}
}
