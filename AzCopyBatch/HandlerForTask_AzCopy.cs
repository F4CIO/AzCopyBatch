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
	public class HandlerForTask_AzCopy
	{
		public static bool Execute(CustomTraceLog log, int taskIndex, string task, string timestamp, int TimeoutForEveryTaskInMinutes)
		{
			bool allSuccess = true;
			log.AddLineAndIncreaseIdent("Executing task: [t" + taskIndex + "] " + task);//task.Split(' ')[1].Trim() + " ---> " +task.Split(' ')[2].Trim());

			int createdDummyFiles = FixEmptyFoldersIfNecessary(ref task, false, 0, ref allSuccess, log);

			string azCopySubfolderPath = Path.Combine(Path.Combine(CraftSynth.BuildingBlocks.Common.Misc.ApplicationRootFolderPath, "AzCopy"));
			string taskLogFileName = Path.GetFileName(Misc.ApplicationPhysicalExeFilePathWithoutExtension) + "_" + timestamp + "_t" +taskIndex + ".log";
			string taskLogFilePath = Path.Combine(azCopySubfolderPath, taskLogFileName);
			log.AddLine("Log file path:" + taskLogFilePath);

			string taskWithoutCustomParameters = RemoveCustomParameters(task);

			string command = taskWithoutCustomParameters.Split(' ')[0].Trim();
			string parameters = taskWithoutCustomParameters.Substring(command.Length + 1).Trim() + " /V:" + taskLogFileName;
			allSuccess = Console.ExecuteCommand(command, parameters, TimeoutForEveryTaskInMinutes * 60000, azCopySubfolderPath, log, false) == 0 && allSuccess;
			//2014.06.15 20:22:55 (local)     >>
			//2014.06.15 20:22:57 (local)     >> Transfer summary:
			//2014.06.15 20:22:58 (local)     >> -----------------
			//2014.06.15 20:22:59 (local)     >> Total files transferred: 99
			//2014.06.15 20:23:00 (local)     >> Transfer successfully:   99
			//2014.06.15 20:23:01 (local)     >> Transfer failed:         0
			//2014.06.15 20:23:02 (local)     Done.
			List<string> logLines = log.ToString().Split('\n').ToList();
			int totalFilesTransferred = -1;
			int transferSuccessfully = -1;
			int transferFailed = -1;

			for (int i = logLines.Count - 1; i >= 0 && i >= logLines.Count - 10; i--)
			{
				if (logLines[i].Contains("Transfer failed:"))
				{
					transferFailed = Int32.Parse(logLines[i].GetSubstringAfterLastOccurence("Transfer failed:").Trim());
				}
				else if (logLines[i].Contains("Transfer successfully:"))
				{
					transferSuccessfully = Int32.Parse(logLines[i].GetSubstringAfterLastOccurence("Transfer successfully:").Trim());
				}
				else if (logLines[i].Contains("Total files transferred:"))
				{
					totalFilesTransferred = Int32.Parse(logLines[i].GetSubstringAfterLastOccurence("Total files transferred:").Trim());
				}

				if (totalFilesTransferred > -1 && transferSuccessfully > -1 && transferFailed > -1)
				{
					break;
				}
			}

			if (totalFilesTransferred == -1 || transferSuccessfully == -1 || transferFailed == -1)
			{
				throw new Exception("Can not parse AzCopy result. Did it execute?");
			}
			else
			{
				allSuccess = allSuccess && (totalFilesTransferred == transferSuccessfully && transferFailed == 0);
			}

			FixEmptyFoldersIfNecessary(ref task, true, createdDummyFiles, ref allSuccess, log);

			return allSuccess;
		}

		public static string RemoveCustomParameters(string task)
		{
			string taskWithoutCustomParameters = task;
			bool skipFixingEmptyFolders = task.GetParameterPresence("/skipFixingEmptyFolders", false, false, '/', null);
			if (skipFixingEmptyFolders)
			{
				taskWithoutCustomParameters = taskWithoutCustomParameters.RemoveParameter("/skipFixingEmptyFolders", false, null, '/',
					'"');
			}
			return taskWithoutCustomParameters;
		}

		public static int FixEmptyFoldersIfNecessary(ref string task, bool azCopyWasExecuted, int createdDummyFiles, ref bool allSuccess, CustomTraceLog log)
		{
			int affectedDummyFiles = 0;

			var parameters = task.GetParameters(false, false, '"');
			string source = parameters[0];
			string destination = parameters[1];

			bool sourceKeyPresent = task.GetParameterPresence("/sourceKey", false, false, '/', null);
			bool destKeyPresent = task.GetParameterPresence("/destKey", false, false, '/', null);
			bool skipFixingEmptyFolders = task.GetParameterPresence("/skipFixingEmptyFolders", false, false, '/', null);

			if (!skipFixingEmptyFolders)
			{
				if (!sourceKeyPresent && destKeyPresent && !azCopyWasExecuted)
				{//files are about to be uploaded to azure storage -create dummy files in empty folders to trigger their creation at azure storage
					affectedDummyFiles = CreateDummyFilesInEmptyFolders(source, log);
				}else if (!sourceKeyPresent && destKeyPresent && azCopyWasExecuted)
				{//files were uploaded to azure -remove local temporary dummy files
					affectedDummyFiles = RemoveDummyFilesFromFolders(source, log);
					allSuccess = allSuccess && (affectedDummyFiles == createdDummyFiles);
					if (affectedDummyFiles != createdDummyFiles)
					{
						log.AddLine("Error: Number of dummy files created does not match the number of dummy files deleted.");
					}
				}
				else if (sourceKeyPresent && !destKeyPresent && azCopyWasExecuted)
				{//files were just downloaded from azure storage to local - remove all dummy files 
					affectedDummyFiles = RemoveDummyFilesFromFolders(destination, log);
				}
			}
			
			return affectedDummyFiles;
		}


		public static int CreateDummyFilesInEmptyFolders(string folderPath, CustomTraceLog log)
		{
			log.AddLine("Detecting empty folders...");
			var emptyFoldersPaths = CraftSynth.BuildingBlocks.IO.FileSystem.GetEmptyFoldersPathsRecursively(folderPath);
			log.AddLine(emptyFoldersPaths.Count+" found.", false);
			log.AddLine("Creating dummy files inside empty folders...");
			int i = 0;
			foreach (string emptyFoldersPath in emptyFoldersPaths)
			{
				File.WriteAllText(Path.Combine(emptyFoldersPath,"EmptyFolderIndicator.txt"), string.Empty);
				i++;
			}
			log.AddLine(i+" created.", false);
			return i;
		}

		public static int RemoveDummyFilesFromFolders(string folderPath, CustomTraceLog log)
		{
			log.AddLine("Detecting dummy files...");
			var filesPaths = CraftSynth.BuildingBlocks.IO.FileSystem.GetFilePaths(folderPath,true , "EmptyFolderIndicator.txt");
			log.AddLine(filesPaths.Count + " found.", false);
			log.AddLine("Deleting dummy files...");
			int i = 0;
			foreach (string filePath in filesPaths)
			{
				File.Delete(filePath);
				i++;
			}
			log.AddLine(i + " deleted.", false);
			return i;
		}
	}
}
