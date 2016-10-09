using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.UCenter.Common.Settings;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GameCloud.UCenter.Web.Common.Storage
{
    /// <summary>
    /// Provide a class for storage account context.
    /// </summary>
    [Export("Storage.Azure", typeof(IStorageContext))]
    [Export(typeof(AzureStorageContext))]
    public class AzureStorageContext : IStorageContext
    {
        private readonly CloudBlobContainer container;
        private readonly CloudBlobContainer secondaryContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageContext" /> class.
        /// </summary>
        /// <param name="settings">Indicating the settings.</param>
        [ImportingConstructor]
        public AzureStorageContext(Settings settings)
        {
            var account = CloudStorageAccount.Parse(settings.PrimaryStorageConnectionString);
            var client = account.CreateCloudBlobClient();
            this.container = client.GetContainerReference(settings.ImageContainerName);
            this.container.CreateIfNotExistsAsync();

            if (!string.IsNullOrEmpty(settings.SecondaryStorageConnectionString))
            {
                this.secondaryContainer = CloudStorageAccount.Parse(settings.SecondaryStorageConnectionString)
                    .CreateCloudBlobClient()
                    .GetContainerReference(settings.ImageContainerName);
            }
        }

        /// <summary>
        /// Create container if not exists.
        /// </summary>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        public async Task CreateContainerIfNotExists(CancellationToken token)
        {
            await this.container.CreateIfNotExistsAsync(token);

            if (this.secondaryContainer != null)
            {
                await this.secondaryContainer.CreateIfNotExistsAsync(token);
            }
        }

        /// <summary>
        /// Upload blob.
        /// </summary>
        /// <param name="blobName">Indicating the blob name.</param>
        /// <param name="stream">Indicating the file stream.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>The blob full url.</returns>
        public async Task<string> UploadBlobAsync(string blobName, Stream stream, CancellationToken token)
        {
            var blob = this.container.GetBlockBlobReference(blobName);
            await blob.UploadFromStreamAsync(stream, token);

            if (this.secondaryContainer != null)
            {
                var secondBlob = this.secondaryContainer.GetBlockBlobReference(blobName);
                await this.CopyBlobAsync(blob, secondBlob, token);
            }

            return blob.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Copy blob inside storage.
        /// </summary>
        /// <param name="sourceBlobName">Indicating the source blob name.</param>
        /// <param name="targetBlobName">Indicating the target blob name.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        public async Task<string> CopyBlobAsync(string sourceBlobName, string targetBlobName, CancellationToken token)
        {
            var sourceBlob = this.container.GetBlockBlobReference(sourceBlobName);
            var targetBlob = this.container.GetBlockBlobReference(targetBlobName);
            await targetBlob.StartCopyAsync(sourceBlob, token);

            return targetBlob.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Copy blob inside storage.
        /// </summary>
        /// <param name="sourceBlob">Indicating the source blob.</param>
        /// <param name="targetBlob">Indicating the target blob.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        public async Task<string> CopyBlobAsync(CloudBlockBlob sourceBlob, CloudBlockBlob targetBlob, CancellationToken token)
        {
            bool tryCopy = true;
            string copyId = string.Empty;
            int retryCount = 3;
            TimeSpan timeOut = TimeSpan.FromMinutes(3);
            while (tryCopy)
            {
                if (!string.IsNullOrEmpty(copyId))
                {
                    await targetBlob.AbortCopyAsync(copyId, token);
                }

                DateTime startTime = DateTime.Now;
                copyId = await targetBlob.StartCopyAsync(sourceBlob, token);
                while (DateTime.Now - startTime < timeOut)
                {
                    Thread.Sleep(100);
                    await targetBlob.FetchAttributesAsync(token);
                    switch (targetBlob.CopyState.Status)
                    {
                        case CopyStatus.Invalid:
                        case CopyStatus.Pending:
                            continue;
                        case CopyStatus.Success:
                            return targetBlob.Uri.AbsoluteUri;
                        case CopyStatus.Aborted:
                        case CopyStatus.Failed:
                        default:
                            retryCount--;
                            if (retryCount == 0)
                            {
                                tryCopy = false;
                            }

                            break;
                    }
                }
            }

            throw new Exception("Copy blob failed");
        }
    }
}