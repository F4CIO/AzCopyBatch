using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CraftSynth.BuildingBlocks.Common;
using CraftSynth.BuildingBlocks.IO;
using CraftSynth.BuildingBlocks.IO.AzureStorage;

namespace AzCopyBatch
{
	public class HandlerForPaths
	{
		/// <summary>
		/// Matches against template in destination parameter and returns all folder paths or azure storage items (containers, directories or blobs) that contain timestamp. 
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="azureStorageKey"></param>
		/// <param name="taskIndex"></param>
		/// <returns></returns>
		public static List<KeyValuePair<object, DateTime>> GetDestinationsFromDestinationWithWildcard(string destination, string azureStorageKey, int? taskIndex = null)
		{
			//get folder that holds childlen with timestamp in their names:
			List<KeyValuePair<object, DateTime>> r = new List<KeyValuePair<object, DateTime>>();

			string destinationBeforeTimestamp = destination.GetSubstringBefore("[T]");
			while (!destinationBeforeTimestamp.EndsWith("/") && !destinationBeforeTimestamp.EndsWith(@"\") &&
				   destinationBeforeTimestamp.Length > 0)
			{
				destinationBeforeTimestamp = destinationBeforeTimestamp.RemoveLastXChars(1);
			}
			destinationBeforeTimestamp = destinationBeforeTimestamp.TrimEnd('/').TrimEnd('\\');
			if (destinationBeforeTimestamp.Length == 0)
			{
				if (taskIndex.HasValue)
				{
					throw new Exception(String.Format("Under task t{0} in destination path '{1}' timestamp [T] was found at invalid position.", taskIndex, destination));
				}
				else
				{
					throw new Exception(String.Format("destination path '{1}' timestamp [T] was found at invalid position.", destination));
				}
			}

			if (string.IsNullOrEmpty(azureStorageKey))
			{//work with file system
				//collect ones with timestamp in name:
				DateTime? dt = null;
				var destinationsWithTimestamp = FileSystem.GetFolderPaths(destinationBeforeTimestamp);
				foreach (string f in destinationsWithTimestamp)
				{
					dt = LocateAndParseTimestamp(Path.GetFileName(f));
					if (dt != null)
					{
						string reparsed = destination.Replace("[T]", dt.Value.ToDateAndTimeInSortableFormatForAzureBlob());
						string reparsed2 = destination.Replace("[T]", dt.Value.ToDateAndTimeAsYYYYMMDDHHMM());
						if (
							string.Compare(reparsed, f, StringComparison.OrdinalIgnoreCase) == 0 ||
							string.Compare(reparsed2, f, StringComparison.OrdinalIgnoreCase) == 0
							)
						{
							r.Add(new KeyValuePair<object, DateTime>(f, dt.Value));
						}
					}
				}
			}
			else
			{//work with azure storage
				//collect ones with timestamp in name:
				DateTime? dt = null;
				BlobUrl destinationUrlBeforeTimestamp = new BlobUrl(destinationBeforeTimestamp);
				destinationUrlBeforeTimestamp.Key = azureStorageKey;
				HandlerForBlobs h = new CraftSynth.BuildingBlocks.IO.AzureStorage.HandlerForBlobs();
				List<BlobUrl> childs = h.GetChildren(destinationUrlBeforeTimestamp, true);

				for (int i = childs.Count - 1; i >= 0; i--)
				{
					if (childs[i].Kind == BlobUrlKind.Container)
					{
						dt = LocateAndParseTimestamp(childs[i].ContainerName);
					}
					else if (childs[i].Kind == BlobUrlKind.SubfolderOrBlob)
					{
						dt = LocateAndParseTimestamp(childs[i].BlobName);
					}

					if (dt == null)
					{
						//timestamp couldn't be extracted- skip it
					}
					else
					{
						string reparsed = destination.Replace("[T]", dt.Value.ToDateAndTimeInSortableFormatForAzureBlob());
						string reparsed2 = destination.Replace("[T]", dt.Value.ToDateAndTimeAsYYYYMMDDHHMM());

						if (
							string.Compare(childs[i].Url, reparsed, StringComparison.OrdinalIgnoreCase) == 0 ||
							string.Compare(childs[i].Url, reparsed2, StringComparison.OrdinalIgnoreCase) == 0 
							)
						{
							r.Add(new KeyValuePair<object, DateTime>(childs[i], dt.Value));
						}
					}
				}
			}

			return r;
		}

		/// <summary>
		/// returns null if error occcured.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="throwErrorIfInInvalidFormat"></param>
		/// <param name="errorCaseResult"></param>
		/// <returns></returns>
		public static DateTime? LocateAndParseTimestamp(string s)
		{
			DateTime? r = null;
			for (int i = 0; i < s.Length - 1; i++)
			{
				r = s.Substring(i).ParseDateAndTimeInSortableFormatForAzureBlob(true);
				
				if (r == null)
				{
					r = s.Substring(i).ParseDateAndTimeAsYYYYMMDDHHMM(true);
				}

				if (r != null)
				{
					break;
				}
			}
			return r;
		}
	}
}
