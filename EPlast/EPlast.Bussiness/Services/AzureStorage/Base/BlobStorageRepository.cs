﻿using EPlast.Bussiness.Interfaces.AzureStorage.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EPlast.Bussiness.Services.AzureStorage.Base
{
    public abstract class BlobStorageRepository : IBlobStorageRepository
    {
        private readonly IAzureBlobConnectionFactory _connectionFactory;
        public BlobStorageRepository(IAzureBlobConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<CloudBlockBlob> GetBlobAsync(string blobName, string containerName)
        {
            var cloudBlobContainer = await _connectionFactory.GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);

            return blockBlob;
        }
        public async Task<string> GetBlobBase64Async(string blobName, string containerName)
        {
            var cloudBlobContainer = await _connectionFactory.GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);

            blockBlob.FetchAttributes();
            byte[] arr = new byte[blockBlob.Properties.Length];
            blockBlob.DownloadToByteArray(arr, 0);
            var azureBase64 = Convert.ToBase64String(arr);
            var result = $"data:image/{Path.GetExtension(blobName)};base64," + azureBase64;
            return result;
        }
        public async Task DeleteBlobAsync(string blobName, string containerName)
        {
            var cloudBlobContainer = await _connectionFactory.GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
            await blockBlob.DeleteIfExistsAsync();
        }

        public async Task UploadBlobAsync(IFormFile blobfile, string fileName, string containerName)
        {
            var cloudBlobContainer = await _connectionFactory.GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
            using (var fileStream = (blobfile.OpenReadStream()))
            {
                blockBlob.UploadFromStream(fileStream);
            }
        }
        public async Task UploadBlobForBase64Async(string base64, string fileName, string containerName)
        {
            var cloudBlobContainer = await _connectionFactory.GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

            byte[] bytes = Convert.FromBase64String(base64);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                blockBlob.UploadFromStream(ms);
            }

        }

    }
}
