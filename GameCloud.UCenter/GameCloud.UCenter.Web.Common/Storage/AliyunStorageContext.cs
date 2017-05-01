using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using GameCloud.UCenter.Common.Settings;
using GameCloud.UCenter.Web.Common.Logger;

namespace GameCloud.UCenter.Web.Common.Storage
{
    [Export("Storage.Ali", typeof(IStorageContext))]
    [Export(typeof(AliyunStorageContext))]
    public class AliyunStorageContext : IStorageContext
    {
        //---------------------------------------------------------------------
        private readonly OssClient client;
        private readonly Settings settings;
        private readonly string bucketName;
        private bool bucketCreated = false;

        //---------------------------------------------------------------------
        [ImportingConstructor]
        public AliyunStorageContext(Settings settings)
        {
            this.settings = settings;
            this.bucketName = settings.AliOssBucketName;
            this.client = new OssClient(
                settings.AliOssEndpoint,
                settings.AliOssAccessKeyId,
                settings.AliOssAccessKeySecret);

        }

        //---------------------------------------------------------------------
        public Task<string> CopyBlobAsync(string sourceBlobName, string targetBlobName, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        //---------------------------------------------------------------------
        public async Task<string> UploadBlobAsync(string blobName, Stream stream, CancellationToken token)
        {
            try
            {
                string key = $"{this.settings.ImageContainerName}/{blobName}";
                this.CreateBucketIfNotExists();
                var result = await Task<PutObjectResult>.Factory.FromAsync(
                    this.client.BeginPutObject,
                    this.client.EndPutObject,
                    this.bucketName,
                    key,
                    stream,
                    null);
                var uri = this.client.GeneratePresignedUri(this.bucketName, key);

                return uri.GetLeftPart(UriPartial.Path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                CustomTrace.TraceError(ex, "Error while upload file to ali oss");
                throw;
            }
        }

        //---------------------------------------------------------------------
        private void CreateBucketIfNotExists()
        {
            if (!this.bucketCreated && !this.client.DoesBucketExist(this.bucketName))
            {
                this.client.CreateBucket(bucketName);

                // set the read access right public.
                this.client.SetBucketAcl(this.bucketName, CannedAccessControlList.PublicRead);
                this.bucketCreated = true;
            }
        }
    }
}
