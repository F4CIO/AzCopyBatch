using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CraftSynth.BuildingBlocks.Common;
using CraftSynth.BuildingBlocks.IO;
using CraftSynth.BuildingBlocks.IO.AzureStorage;
using CraftSynth.BuildingBlocks.Logging;

namespace AzCopyBatch
{
	public class HandlerForTask_Delete
	{
		public static bool Execute(int taskIndex, string taskTemplate, string currentTimestampString, CustomTraceLog log)
		{
			bool allSuccess = true;
			log.AddLineAndIncreaseIdent("Deleting folders or azure storage items...");
			

			List<string> parts = taskTemplate.GetParameters();

			//extract destination path:
			string destination = parts[1];
			log.AddLine("Item(s) to delete: " + destination);	
			
			//extract destination key:
			string azureStorageKey = taskTemplate.GetParameterValue<string>("/destkey", false, null, true, null, false, null, '/',':');
			log.AddLine("Azure storage key: " + (azureStorageKey == null ? "This is not Azure storage" : azureStorageKey.Bubble(20, "...")));

			if (destination.OccurrencesCount("[T]") > 1)
			{
				throw new Exception(String.Format("Under task t{0} in destination path '{1}' multiple timestamps [T] were found. Only none or one occurrance is supported.", taskIndex, destination));
			}
			else if (destination.OccurrencesCount("[T]") == 0)
			{//just delete or empty single item
				//extract /DeleteOnlyContent info
				bool deleteOnlyContent = taskTemplate.GetParameterPresence("/DeleteOnlyContent", false, false, '/');
				log.AddLine("DeleteOnlyContent:" + deleteOnlyContent);

				if (string.IsNullOrEmpty(azureStorageKey))
				{//just delete or empty single folder
					log.AddLine("Deleting folder '" + destination + "' ...");
					try
					{
						if (!deleteOnlyContent)
						{
							Directory.Delete(destination, true);
							log.AddLine("Deleted.");
						}
						else
						{
							List<string> subfoldersPaths = CraftSynth.BuildingBlocks.IO.FileSystem.GetFolderPaths(destination);
							List<string> filesPaths = CraftSynth.BuildingBlocks.IO.FileSystem.GetFilePaths(destination);
							log.AddLine("Subitems left to process: ");
							int i = subfoldersPaths.Count+filesPaths.Count;
							log.AddLine("..." + i);
							foreach (string subfolder in subfoldersPaths)
							{
								Directory.Delete(subfolder, true);
								i--;
								log.AddLine("..." + i);
							}
							foreach (string filePath in filesPaths)
							{
								File.Delete(filePath);
								i--;
								log.AddLine("..." + i);
							}
							log.AddLine("Emptied.");
						}
					}
					catch (Exception e)
					{
						log.AddLine("Failed.");
						HandlerForLoging.LogException(e, log);
						allSuccess = false;
					}
				}
				else
				{//just delete or empty single container,directory or blob
					BlobUrl destinationUrl = new BlobUrl(destination);
					destinationUrl.Key = azureStorageKey;
					log.AddLine(string.Format("Deleting azure storage item: {0}", destinationUrl.Url));
					try
					{
						var h = new CraftSynth.BuildingBlocks.IO.AzureStorage.HandlerForBlobs();
						if (!deleteOnlyContent)
						{
							allSuccess = h.Delete(destinationUrl) > 0 && allSuccess;
							log.AddLine("Deleted.");
						}
						else
						{
							var childs = h.GetChildren(destinationUrl, true);
							log.AddLine("Subitems left to process: ");
							int i = childs.Count;
							log.AddLine("..."+i);
							foreach (BlobUrl child in childs)
							{
								allSuccess = h.Delete(child) > 0 && allSuccess;
								i--;
								log.AddLine("..."+i);
							}
						}
					}
					catch (Exception e)
					{
						log.AddLine("Failed.");
						HandlerForLoging.LogException(e, log);
						allSuccess = false;
					}
				}
			}
			else
			{
				//extract /KeepLastXDays:1 info:
				int? keepLastXDays = null;
				if (taskTemplate.GetParameterPresence("/KeepLastXDays", false, false, '/', ':'))
				{
					keepLastXDays = taskTemplate.GetParameterValue<int>("/KeepLastXDays", true, -1, true, -1, false, -1, '/', ':');
				}
				log.AddLine("KeepLastXDays: " + (keepLastXDays == null ? "No" : keepLastXDays.Value.ToString()));

				//extract /KeepLastInMonth info
				bool keepLastInMonth = taskTemplate.GetParameterPresence("/KeepLastInMonth", false, false, '/');
				log.AddLine("KeepLastInMonth:" + keepLastInMonth);

				//extract /KeepLastInYear info
				bool keepLastInYear = taskTemplate.GetParameterPresence("/KeepLastInYear", false, false, '/');
				log.AddLine("KeepLastInYear:" + keepLastInYear);

				//extract /DeleteOnlyContent info
				bool deleteOnlyContent = taskTemplate.GetParameterPresence("/DeleteOnlyContent", false, false, '/');
				log.AddLine("DeleteOnlyContent:" + deleteOnlyContent);
				
				//Traverse all children, collect ones with timestamp in name, check age and delete if too old:

				if (azureStorageKey.IsNullOrWhiteSpace())
				{//folders	
					var deletionItems = HandlerForPaths.GetDestinationsFromDestinationWithWildcard(destination, null, taskIndex);

					//leave only very old items so they can be deleted: 
					deletionItems = ConsiderRetentionAndReturnItemsForDeletion(deletionItems, currentTimestampString, keepLastXDays, keepLastInMonth, keepLastInYear);

					//delete old items:
					log.AddLine("Deleting " + deletionItems.Count + " folder(s) ...");
					int deletedCount = 0;
					foreach (KeyValuePair<object, DateTime> deletionItem in deletionItems)
					{
						try
						{
							if (!deleteOnlyContent)
							{
								Directory.Delete(deletionItem.Key.ToString(), true);
							}
							else
							{
								List<string> subfoldersPaths = CraftSynth.BuildingBlocks.IO.FileSystem.GetFolderPaths(deletionItem.Key.ToString());
								List<string> filesPaths = CraftSynth.BuildingBlocks.IO.FileSystem.GetFilePaths(deletionItem.Key.ToString());
								log.AddLine("Subitems left to process: ");
								int i = subfoldersPaths.Count + filesPaths.Count;
								log.AddLine("..." + i);
								foreach (string subfolder in subfoldersPaths)
								{
									Directory.Delete(subfolder, true);
									i--;
									log.AddLine("..." + i);
								}
								foreach (string filePath in filesPaths)
								{
									File.Delete(filePath);
									i--;
									log.AddLine("..." + i);
								}
							}
							deletedCount++;
						}
						catch (Exception e)
						{
							HandlerForLoging.LogException(e, log);
							allSuccess = false;
						}
					}
					if (deletedCount == deletionItems.Count)
					{
						if (deleteOnlyContent)
						{
							log.AddLine("Emptied successfully.");
						}
						else
						{
							log.AddLine("Deleted successfully.");
						}
					}else
					{
						if (deleteOnlyContent)
						{
							log.AddLine("Emptied just " + deletedCount + " items.");
						}
						else
						{
							log.AddLine("Deleted just " + deletedCount + " items.");
						}
					}
				}
				else
				{//azure storage items
					var deletionItems = HandlerForPaths.GetDestinationsFromDestinationWithWildcard(destination, azureStorageKey, taskIndex);

					//leave only very old items so they can be deleted: 
					deletionItems = ConsiderRetentionAndReturnItemsForDeletion(deletionItems, currentTimestampString, keepLastXDays, keepLastInMonth, keepLastInYear);

					//delete old items:	
					log.AddLine(string.Format("Deleting {0} Azure storage item(s) ...", deletionItems.Count));
					int deletedCount = 0;
					HandlerForBlobs h = new HandlerForBlobs();
					foreach (KeyValuePair<object, DateTime> deletionItem in deletionItems)
					{
						(deletionItem.Key as BlobUrl).Key = azureStorageKey;
						try
						{
							bool r = false;
							if (!deleteOnlyContent)
							{
								r = h.Delete(deletionItem.Key as BlobUrl) > 0;
							}
							else
							{
								var childs = h.GetChildren((deletionItem.Key as BlobUrl), true);
								log.AddLine("Subitems left to process: ");
								int i = childs.Count;
								log.AddLine("..." + i);
								r = true;
								foreach (BlobUrl child in childs)
								{
									r = h.Delete(child) > 0 && r;
									i--;
									log.AddLine("..." + i);
								}
							}
							allSuccess = allSuccess && r;
							deletedCount++;
						}
						catch (Exception e)
						{
							HandlerForLoging.LogException(e, log);
							allSuccess = false;
						}
					}
					if (deletedCount == deletionItems.Count)
					{
						log.AddLine("Deleted successfully.");
					}
					else
					{
						log.AddLine("Deleted just " + deletedCount + " items.");
					}
				}
			}

			return allSuccess;
		}

		private static List<KeyValuePair<object, DateTime>> ConsiderRetentionAndReturnItemsForDeletion(List<KeyValuePair<object, DateTime>> deletionItems, string currentTimestampString, int? keepLastXDays, bool keepLastInMonth, bool keepLastInYear)
		{
			DateTime currentTimestamp = currentTimestampString.ParseDateAndTimeInSortableFormatForAzureBlob().Value;
			deletionItems.Sort((a, b) =>DateTime.Compare(a.Value,b.Value) );

			List<KeyValuePair<object, DateTime>> itemsToPreserve = new List<KeyValuePair<object, DateTime>>();
			
			foreach (KeyValuePair<object, DateTime> deletionItem in deletionItems)
			{
				if (keepLastXDays.HasValue)
				{
					DateTime latestItemToDelete = currentTimestamp.Date.AddDays(-keepLastXDays.Value);

					if (DateTime.Compare(deletionItem.Value, latestItemToDelete) <= 0)
					{
						//should be deleted
					}
					else
					{
						itemsToPreserve.Add(deletionItem);
					}
				}

				if (keepLastInMonth)
				{
					var lastInSameMonth =  deletionItems.Where(i => i.Value.Year==deletionItem.Value.Year && i.Value.Month==deletionItem.Value.Month).Max(i => i.Value);
					if (deletionItem.Value == lastInSameMonth)
					{
						itemsToPreserve.Add(deletionItem);
					}
				}

				if (keepLastInYear)
				{
					var lastInSameYear = deletionItems.Where(i => i.Value.Year == deletionItem.Value.Year).Max(i => i.Value);
					if (deletionItem.Value == lastInSameYear)
					{
						itemsToPreserve.Add(deletionItem);
					}
				}
			}

			for (int i = deletionItems.Count - 1; i >= 0; i--)
			{
				if (itemsToPreserve.Exists(item => item.Value == deletionItems[i].Value))
				{
					deletionItems.RemoveAt(i);
				}
			}

			return deletionItems;
		}
	}
}
