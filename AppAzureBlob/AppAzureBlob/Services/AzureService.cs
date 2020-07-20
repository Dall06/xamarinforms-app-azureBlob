using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAzureBlob.Services
{
    public enum AzureContainer
    {
        Images,
        Text
    }

    public class AzureService
    {
        static CloudBlobContainer GetContainer(AzureContainer containerType)
        {
            // Get container according a string azure connection
            var account = CloudStorageAccount.Parse(Config.Constants.StorageConnection);
            var client = account.CreateCloudBlobClient();

            return client.GetContainerReference(containerType.ToString().ToLower());
        }

        public static async Task<List<string>> GetFilesListAsync(AzureContainer containerType)
        {
            // Get all blob files from the specific container
            var container = GetContainer(containerType);
            var list = new List<string>();
            BlobContinuationToken token = null;

            do
            {
                var result = await container.ListBlobsSegmentedAsync(token);
                if (result.Results.Count() > 0)
                {
                    var blobs = result.Results.Cast<CloudBlockBlob>().Select(b => b.Name);
                    list.AddRange(blobs);
                }
                token = result.ContinuationToken;
            } while (token != null);

            return list;
        }

        public static async Task<byte[]> GetFileAsync(AzureContainer containerType, string fileName)
        {
            // Download blob file with the name "fileName" from container "containerType"
            var container = GetContainer(containerType);
            var blob = container.GetBlobReference(fileName);

            if (await blob.ExistsAsync())
            {
                await blob.FetchAttributesAsync();
                byte[] blobBytes = new byte[blob.Properties.Length]; 
                await blob.DownloadToByteArrayAsync(blobBytes, 0);

                return blobBytes;
            }
            return null;
        }

        public static async Task<string> UploadFileAsync(AzureContainer containerType, Stream stream)
        {
            var container = GetContainer(containerType);
            await container.CreateIfNotExistsAsync();

            // Guid is a unique global structure
            var name = Guid.NewGuid().ToString();
            var fileBlob = container.GetBlockBlobReference(name);
            await fileBlob.UploadFromStreamAsync(stream);

            return name;
        }

        public static async Task<bool> DeleteFileAsync(AzureContainer containerType, string fileName)
        {
            // Deletes blob file from a container
            var container = GetContainer(containerType);
            var blob = container.GetBlobReference(fileName);

            return await blob.DeleteIfExistsAsync();
        }

        public static async Task<bool> DeleteContainerAsync(AzureContainer containerType)
        {
            // Deletes a container
            var container = GetContainer(containerType);

            return await container.DeleteIfExistsAsync();
        }
    }
}
