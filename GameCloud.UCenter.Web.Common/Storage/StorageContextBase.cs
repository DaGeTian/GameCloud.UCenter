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
        //---------------------------------------------------------------------
        Task<string> UploadBlobAsync(string blobName, Stream stream, CancellationToken token);

        //---------------------------------------------------------------------
        Task<string> CopyBlobAsync(string sourceBlobName, string targetBlobName, CancellationToken token);
    }
}
