using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameCloud.UCenter.Web.Common.Storage
{
    public interface IStorageContext
    {
        /// <summary>
        /// Upload blob.
        /// </summary>
        /// <param name="blobName">Indicating the blob name.</param>
        /// <param name="stream">Indicating the file stream.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>The blob full url.</returns>
        Task<string> UploadBlobAsync(string blobName, Stream stream, CancellationToken token);

        /// <summary>
        /// Copy blob inside storage.
        /// </summary>
        /// <param name="sourceBlobName">Indicating the source blob name.</param>
        /// <param name="targetBlobName">Indicating the target blob name.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        Task<string> CopyBlobAsync(string sourceBlobName, string targetBlobName, CancellationToken token);
    }
}
