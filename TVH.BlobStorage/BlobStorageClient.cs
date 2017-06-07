using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVH.BlobStorage
{
    public class BlobStorageClient
    {
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;

        public BlobStorageClient(string connectionString)
        {
            _storageAccount = CloudStorageAccount.Parse(connectionString);
            _blobClient = _storageAccount.CreateCloudBlobClient();
        }

        public async Task<byte[]> GetBlob(string containerName, string blobName)
        {
            // Get container reference
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            
            // Get blob reference and properties
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
            if (await blob.ExistsAsync() == false)
                throw new Exception(String.Format("Could not find blob '{0}' in container '{1}'", blobName, containerName));

            blob.FetchAttributes();
            long blobLength = blob.Properties.Length;
            byte[] blobContent = new byte[blobLength];

            // Get blob content
            await blob.DownloadToByteArrayAsync(blobContent, 0);

            // Return blob content
            return blobContent;
        }
    }
}
