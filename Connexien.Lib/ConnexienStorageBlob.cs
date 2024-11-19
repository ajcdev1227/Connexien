using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Connexien.Lib
{
    public sealed class ConnexienStorageContainers
    {
        public class StorageContainer
        {
            private readonly string _value;

            public static readonly StorageContainer ContentItems = new StorageContainer("site-content-items");
            public static readonly StorageContainer SiteContent = new StorageContainer("content");
            public static readonly StorageContainer CustomForms = new StorageContainer("site-custom-form-request");
            public static readonly StorageContainer ProfileImages = new StorageContainer("site-profile-images");
            public static readonly StorageContainer ProfileImagesResized = new StorageContainer("site-profile-images-resized");
            public static readonly StorageContainer Publications = new StorageContainer("site-publications");

            public StorageContainer(string value)
            {
                _value = value;
            }

            public override String ToString()
            {
                return _value;
            }
        }
    }

    public static class ConnexienStorageBlob
    {
        public static void UploadFileToStorageBlob(HttpPostedFileBase file,
                                            ConnexienStorageContainers.StorageContainer storageContainer,
                                            string fileName,
                                            Dictionary<string, string> storageConfig = null)
        {
            BlobClient blobClient = GetBlobClient(storageContainer, fileName, ref storageConfig);

			var ext = Path.GetExtension(file.FileName);
            var mimeType = Utilities.MimeTypeMap.GetMimeType(ext);
			BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders
			{
				ContentType = mimeType
			};

			blobClient.Upload(file.InputStream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });
        }

        public static void DeleteFileFromStorageBlob(string fileName, ConnexienStorageContainers.StorageContainer storageContainer, Dictionary<string, string> storageConfig = null)
        {
            BlobClient blobClient = GetBlobClient(storageContainer, fileName, ref storageConfig);
            blobClient.DeleteIfExists();
        }

        public static string GetPathOfBlob(string fileName, ConnexienStorageContainers.StorageContainer storageContainer, Dictionary<string, string> storageConfig = null)
        {
            BlobClient blobClient = GetBlobClient(storageContainer, fileName, ref storageConfig);

            if (blobClient.Exists())
            {
                return blobClient.Uri.ToString();
            }

            return string.Empty;
        }

        public static byte[] DownloadBlob(string fileName, ConnexienStorageContainers.StorageContainer storageContainer, Dictionary<string, string> storageConfig = null)
        {
            BlobClient blobClient = GetBlobClient(storageContainer, fileName, ref storageConfig);

            if (blobClient.Exists())
            {
                MemoryStream memoryStream = new MemoryStream();
                blobClient.DownloadTo(memoryStream);
                return memoryStream.ToArray();
            }

            return null;
        }

        public static Dictionary<string, string> GetDictionaryOfAppSettings()
        {
            return ConfigurationManager.AppSettings.AllKeys.ToDictionary(x => x, x => ConfigurationManager.AppSettings[x]);
        }

        private static BlobClient GetBlobClient(ConnexienStorageContainers.StorageContainer storageContainer, string fileName, ref Dictionary<string, string> storageConfig)
        {
            if (storageConfig == null)
            {
                storageConfig = GetDictionaryOfAppSettings();
            }

            BlobContainerClient blobContainerClient = new BlobContainerClient(storageConfig["Azure:StorageConnectionString"], storageContainer.ToString());
			blobContainerClient.CreateIfNotExists();
			BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
            return blobClient;
        }
    }
}
