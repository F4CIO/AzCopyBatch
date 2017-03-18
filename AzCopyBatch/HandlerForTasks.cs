using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using CraftSynth.BuildingBlocks.Common;
using CraftSynth.BuildingBlocks.IO;
using CraftSynth.BuildingBlocks.IO.AzureStorage;
using CraftSynth.BuildingBlocks.Logging;
using Console = CraftSynth.BuildingBlocks.UI.Console;
using Misc = CraftSynth.BuildingBlocks.Common.Misc;

namespace AzCopyBatch
{
	public class HandlerForTasks
	{
		public static bool Execute(CustomTraceLog log)
		{
			string iniFilePath = Misc.ApplicationPhysicalExeFilePathWithoutExtension + ".ini";
	        var lines = File.ReadLines(iniFilePath);
			List<string> tasks = null;
			foreach (string line in lines)
			{
				if (line.Trim().StartsWith("[Tasks]"))
				{
					tasks = new List<string>();
				}
				else if (tasks != null && line.Trim().Length > 0 && !line.StartsWith("--"))
				{
					tasks.Add(line.Trim());
				}
			}
			log.DecreaseIdent();

			bool ExecuteNextTaskAfterError = FileSystem.GetSettingFromIniFile("ExecuteNextTaskAfterError", iniFilePath, true, false, true, false, false, false);
			int TimeoutForEveryTaskInMinutes = FileSystem.GetSettingFromIniFile("TimeoutForEveryTaskInMinutes", iniFilePath, true, -1, true, -1, false, -1);
			
			string timestamp = DateTime.Now.ToDateAndTimeInSortableFormatForAzureBlob();
		
			int taskIndex = 0;
			bool allSuccess = true;
			string task;
			foreach (string taskTemplate in tasks)
			{
				task = taskTemplate.Replace("[T]", timestamp);
				if (task.ToLower().StartsWith("run"))
				{
					try
					{
						allSuccess = HandlerForTask_Run.Execute(log, taskIndex, task, timestamp, TimeoutForEveryTaskInMinutes) && allSuccess;
					}
					catch (Exception exception)
					{
						HandlerForLoging.LogException(exception, log);
						log.AddLine(exception.Message);
						allSuccess = false;
						if (!ExecuteNextTaskAfterError)
						{
							log.AddLine("Aborting...");
							break;
						}
					}
				}else if (task.ToLower().StartsWith("azcopy")) 
				{ 
					try
					{
						allSuccess = HandlerForTask_AzCopy.Execute(log, taskIndex, task, timestamp, TimeoutForEveryTaskInMinutes) && allSuccess ;
					}
					catch (Exception exception)
					{
						HandlerForLoging.LogException(exception, log);
						log.AddLine(exception.Message);
						allSuccess = false;
						if (!ExecuteNextTaskAfterError)
						{
							log.AddLine("Aborting...");
							break;
						}
					}
				}else if (task.ToLower().StartsWith("delete"))
				{
					try
					{
						allSuccess = HandlerForTask_Delete.Execute(taskIndex, taskTemplate, timestamp, log) && allSuccess;
					}
					catch (Exception exception)
					{
						HandlerForLoging.LogException(exception, log);
						allSuccess = false;
						if (!ExecuteNextTaskAfterError)
						{

							log.AddLine("Aborting...");
							break;
						}
					}
					log.DecreaseIdent();
				}
				else
				{
					throw new Exception("Task not recognized. Task index:"+taskIndex);
				}
				taskIndex++;
			}

			return allSuccess;

		}
	}
}
