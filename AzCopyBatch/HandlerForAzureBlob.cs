using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

using CraftSynth.BuildingBlocks.Common;
using CraftSynth.BuildingBlocks.IO.AzureStorage;

namespace AzCopyBatch
{   //Z1UUBIAH3, and PI0GBPVDUI6
	public class HandlerForAzureBlob
	{
		#region Private Members
		private CloudStorageAccount storageAccount;
		private CloudBlobClient blobClient;
		private const string containersPathPrefix = "";
		#endregion

		#region Properties
		#endregion

		#region Public Methods
		private static string ExtractBlobName(string destinationBlobPath)
		{
			string blobName;
				if (destinationBlobPath.ToLower().StartsWith("http://"))
				{
					destinationBlobPath = destinationBlobPath.Substring("http://".Length);
				}
			if (destinationBlobPath.ToLower().StartsWith("https://"))
			{
				destinationBlobPath = destinationBlobPath.Substring("https://".Length);
			}
			string containerPath = (containersPathPrefix+destinationBlobPath.Replace('\\', '/').Trim('/')).ToLower();
			int secondSlash = containerPath.IndexOfNthOccurrence("/", 2);
			if (secondSlash > 0)
			{
				blobName = containerPath.Substring(secondSlash + 1);
				containerPath = containerPath.Substring(0, secondSlash).Split('/')[1]; 
			}
			else
			{
				blobName = containerPath;
				containerPath = null;
			}
			return blobName;
		}

		private static string ExtractContainerPath(string destinationBlobPath)
		{
			string blobName;
			if (destinationBlobPath.ToLower().StartsWith("http://"))
			{
				destinationBlobPath = destinationBlobPath.Substring("http://".Length);
			}
			if (destinationBlobPath.ToLower().StartsWith("https://"))
			{
				destinationBlobPath = destinationBlobPath.Substring("https://".Length);
			}
			string containerPath = (containersPathPrefix + destinationBlobPath.Replace('\\', '/').Trim('/')).ToLower();
			int secondSlash = containerPath.IndexOfNthOccurrence("/",2);
			if (secondSlash > 0)
			{
				blobName = containerPath.Substring(secondSlash + 1);
				containerPath = containerPath.Substring(0, secondSlash).Split('/')[1];
			}
			else
			{
				blobName = containerPath;
				containerPath = null;
			}
			return containerPath;
		}


		public bool BlobExists(string blobPath)
		{
			bool r = false;

			var containerPath = ExtractContainerPath(blobPath);
			var blobName = ExtractBlobName(blobPath);

			CloudBlobContainer container = blobClient.GetContainerReference(containerPath);
			if (container.Exists())
			{
				CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
				r = blob.Exists();
			}

			return r;
		}

		public string GetBlobUri(string blobPath)
		{
			string r;

			var containerPath = ExtractContainerPath(blobPath);
			var blobName = ExtractBlobName(blobPath);

			CloudBlobContainer container = blobClient.GetContainerReference(containerPath);
			if (!container.Exists())
			{
				throw new Exception(string.Format("Container '{0}' does not exist.", containerPath));
			}
			else
			{
				CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
				if (!blob.Exists())
				{
					throw new Exception(string.Format("Blob '{0}' does not exist in container '{1}'.", blobName, containerPath));
				}
				else
				{
					r = blob.Uri.AbsoluteUri;
				}
			}

			return r;
		}

		public bool IsContainer(string path)
		{
			bool r = false;

			path = (containersPathPrefix + path.Replace('\\', '/').Trim('/')).ToLower();
			r = ExtractContainerPath(path) == path;

			return r;
		}

		public string UploadFile(string localFilePath, string destinationBlobPath)
		{
			var containerPath = ExtractContainerPath(destinationBlobPath);
			var blobName = ExtractBlobName(destinationBlobPath);

			CloudBlobContainer container = blobClient.GetContainerReference(containerPath);
			bool created = container.CreateIfNotExists();

			if (created)
			{
				var perm = new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Blob};
				container.SetPermissions(perm);
			}

			CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
			using (var fileStream = System.IO.File.OpenRead(localFilePath))
			{
				blob.UploadFromStream(fileStream);
			}

			return blob.Uri.AbsoluteUri;
		}

		public void DownloadBlobToFile(string sourceBlobPath, string destinationFilePath)
		{
			var containerPath = ExtractContainerPath(sourceBlobPath);
			var blobName = ExtractBlobName(sourceBlobPath);

			CloudBlobContainer container = blobClient.GetContainerReference(containerPath);
			if (!container.Exists())
			{
				throw new Exception(string.Format("Blob container '{0}' not found.", containerPath));
			}

			CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
			if (!blob.Exists())
			{
				throw new Exception(string.Format("Blob '{0}' in container '{1}' not found.", blobName, containerPath));
			}

			blob.DownloadToFile(destinationFilePath, FileMode.Create);

			//return blob.Uri.AbsoluteUri;
		}

		public void DeleteContainer(string path)
		{
			CloudBlobContainer container = blobClient.GetContainerReference(path);
			if (!container.Exists())
			{
				throw new Exception(string.Format("Blob container '{0}' not found.", path));
			}

			container.Delete();
		}

		public void DeleteBlob(string sourceBlobPath)
		{
			var containerPath = ExtractContainerPath(sourceBlobPath);
			var blobName = ExtractBlobName(sourceBlobPath);

			CloudBlobContainer container = blobClient.GetContainerReference(containerPath);
			if (!container.Exists())
			{
				throw new Exception(string.Format("Blob container '{0}' not found.", containerPath));
			}

			CloudBlockBlob blob = container.GetBlockBlobReference(sourceBlobPath);
			if (!blob.Exists())
			{
				throw new Exception(string.Format("Blob '{0}' in container '{1}' not found.", blobName, containerPath));
			}

			blob.Delete();

			//return blob.Uri.AbsoluteUri;
		}

		public int DeletePath(string path)
		{
			int itemsDeleted = 0;

			BlobUrl url = new BlobUrl(path);
			if (url.Kind == BlobUrlKind.Account)
			{
				throw new Exception("Deletion of storage account is not implemented.");
			}
			else if (url.Kind == BlobUrlKind.Container)
			{
				DeleteContainer(url.Url);
				itemsDeleted++;
			}
			else if(url.Kind== BlobUrlKind.SubfolderOrBlob)
			{
				if (BlobExists(url.Url))
				{
					DeleteBlob(url.Url);
					itemsDeleted++;
				}
				else
				{
					CloudBlobContainer container = blobClient.GetContainerReference(url.ContainerUrl);
					if (!container.Exists())
					{
						throw new Exception(string.Format("Blob container '{0}' not found.", path));
					}
					
					CloudBlobDirectory dir = container.GetDirectoryReference(url.BlobName);
					if (dir == null)
					{
						throw new Exception(string.Format("Blob directory '{0}' not found.", url.BlobName));
					}
					var matchedBlobs = dir.ListBlobs(true, BlobListingDetails.None, null, null);
					List<string> blobsUrlsToDelete = new List<string>();
					foreach (IListBlobItem listBlobItem in matchedBlobs)
					{
						//blobsUrlsToDelete.Add(listBlobItem.Uri.AbsoluteUri);
						CloudBlockBlob blob = container.GetBlockBlobReference(listBlobItem.Uri.AbsoluteUri);
						blob.Delete();
						itemsDeleted++;
					}
					//foreach (string urlToDelete in blobsUrlsToDelete)
					//{
					//	DeleteBlob(urlToDelete);
					//}
				}
			}

			return itemsDeleted;
		}
		#endregion

		#region Constructors And Initialization
		public HandlerForAzureBlob(string accountName, string accountKey)
		{
			string connectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", accountName, accountKey);
			this.storageAccount = CloudStorageAccount.Parse(connectionString);
			this.blobClient = this.storageAccount.CreateCloudBlobClient();
		}
		#endregion

		#region Deinitialization And Destructors
		#endregion

		#region Event Handlers
		#endregion

		#region Private Methods
		#endregion

		#region Helpers
		#endregion
	
	}
}