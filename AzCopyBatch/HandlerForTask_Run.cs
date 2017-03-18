using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CraftSynth.BuildingBlocks.Common;
using CraftSynth.BuildingBlocks.Logging;
using Console = CraftSynth.BuildingBlocks.UI.Console;
using Misc = CraftSynth.BuildingBlocks.Common.Misc;

namespace AzCopyBatch
{
	public class HandlerForTask_Run
	{
		public static bool Execute(CustomTraceLog log, int taskIndex, string task, string timestamp, int TimeoutForEveryTaskInMinutes)
		{
			bool allSuccess = true;
			log.AddLineAndIncreaseIdent("Executing task: [t" + taskIndex + "] " + task);//task.Split(' ')[1].Trim() + " ---> " +task.Split(' ')[2].Trim());

			task = task.Substring("run ".Length).Trim();

			string successIndicator = task.GetParameterValue<string>("/successIndicator",false,null,true,null,false,null,'/',':',true,'"');
			string failIndicator = task.GetParameterValue<string>("/failIndicator",false,null,true,null,false,null,'/',':',true,'"');

			string taskWithoutCustomParameters = RemoveCustomParameters(task);
			
			string command = null;
			if (taskWithoutCustomParameters.StartsWith("\""))
			{
				command = taskWithoutCustomParameters.GetSubstring("\"", "\"");
			}
			else
			{
				command = taskWithoutCustomParameters.Split(' ')[0].Trim();
			}

			string workingFolder = null;
			try
			{
				if (File.Exists(command))
				{
					workingFolder = Path.GetDirectoryName(command);
				}
			}
			catch (Exception)
			{
				workingFolder = null;
			}

			string parameters = null;
			try
			{
				if (taskWithoutCustomParameters.StartsWith("\""))
				{
					parameters = taskWithoutCustomParameters.Substring(("\"" + command + "\"").Length).Trim();
				}
				else
				{
					parameters = taskWithoutCustomParameters.Substring(command.Length).Trim();
				}
			}catch(Exception)
			{
				parameters = null;
			}
				
			allSuccess = Console.ExecuteCommand(command, parameters, TimeoutForEveryTaskInMinutes * 60000, workingFolder, log, false, true, successIndicator, failIndicator) == 0 && allSuccess;
            //12 percent processed.
            //21 percent processed.
            //30 percent processed.
            //43 percent processed.
            //51 percent processed.
            //60 percent processed.
            //73 percent processed.
            //82 percent processed.
            //90 percent processed.
            //Processed 184 pages for database 'Test1', file 'Test1' on file 1.
            //100 percent processed.
            //Processed 1 pages for database 'Test1', file 'Test1_log' on file 1.
            //BACKUP DATABASE successfully processed 185 pages in 0.146 seconds (9.899 MB/sec)
            //.
            //1>
			//List<string> logLines = log.ToString().Split('\n').ToList();
			//int totalFilesTransferred = -1;
			//int transferSuccessfully = -1;
			//int transferFailed = -1;

			//for (int i = logLines.Count - 1; i >= 0 && i >= logLines.Count - 10; i--)
			//{
			//	if (logLines[i].Contains("Transfer failed:"))
			//	{
			//		transferFailed = Int32.Parse(logLines[i].GetSubstringAfterLastOccurence("Transfer failed:").Trim());
			//	}
			//	else if (logLines[i].Contains("Transfer successfully:"))
			//	{
			//		transferSuccessfully = Int32.Parse(logLines[i].GetSubstringAfterLastOccurence("Transfer successfully:").Trim());
			//	}
			//	else if (logLines[i].Contains("Total files transferred:"))
			//	{
			//		totalFilesTransferred = Int32.Parse(logLines[i].GetSubstringAfterLastOccurence("Total files transferred:").Trim());
			//	}

			//	if (totalFilesTransferred > -1 && transferSuccessfully > -1 && transferFailed > -1)
			//	{
			//		break;
			//	}
			//}

			//if (totalFilesTransferred == -1 || transferSuccessfully == -1 || transferFailed == -1)
			//{
			//	throw new Exception("Can not parse AzCopy result. Did it execute?");
			//}
			//else
			//{
			//	allSuccess = allSuccess && (totalFilesTransferred == transferSuccessfully && transferFailed == 0);
			//}
			
			return allSuccess;
		}

		public static string RemoveCustomParameters(string task)
		{
			string taskWithoutCustomParameters = task;

			bool successIndicatorParamExist = task.GetParameterPresence("/successIndicator", false, false, '/', ':');
			if (successIndicatorParamExist)
			{
				taskWithoutCustomParameters = taskWithoutCustomParameters.RemoveParameter("/successIndicator", false, null, '/','"',':');
			}

			bool failIndicatorParamExist = task.GetParameterPresence("/failIndicator", false, false, '/', ':');
			if (failIndicatorParamExist)
			{
				taskWithoutCustomParameters = taskWithoutCustomParameters.RemoveParameter("/failIndicator", false, null, '/', '"', ':');
			}

			return taskWithoutCustomParameters;
		}
	}
}
