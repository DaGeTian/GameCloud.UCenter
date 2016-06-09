namespace GF.UCenter.Common.Settings
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    [Export]
    public class Settings
    {
        [DefaultValue("UCenter")]
        public string BucketName { get; set; }

        [DefaultValue(1000)]
        public uint DefaultOperationLifespan { get; set; }

        [DefaultValue(true)]
        public bool EnableTcpKeepAlives { get; set; }

        [DefaultValue(1000 * 60 * 60)]
        public uint TcpKeepAliveTime { get; set; }

        [DefaultValue(1000)]
        public uint TcpKeepAliveInterval { get; set; }

        [DefaultValue(false)]
        public bool UseSsl { get; set; }

        [DefaultValue(10)]
        public int PoolMaxSize { get; set; }

        [DefaultValue(5)]
        public int PoolMinSize { get; set; }

        [DefaultValue(12000)]
        public int PoolSendTimeout { get; set; }

        [DefaultValue("UseDevelopmentStorage=true")]
        public string PrimaryStorageConnectionString { get; set; }

        public string SecondaryStorageConnectionString { get; set; }

        [DefaultValue("images")]
        public string ImageContainerName { get; set; }

        [DefaultValue("default_profile_image_male.jpg")]
        public string DefaultProfileImageForMaleBlobName { get; set; }

        [DefaultValue("default_profile_thumbnail_male.jpg")]
        public string DefaultProfileThumbnailForMaleBlobName { get; set; }

        [DefaultValue("default_profile_image_female.jpg")]
        public string DefaultProfileImageForFemaleBlobName { get; set; }

        [DefaultValue("default_profile_thumbnail_female.jpg")]
        public string DefaultProfileThumbnailForFemaleBlobName { get; set; }

        [DefaultValue("profile_image_{0}.jpg")]
        public string ProfileImageForBlobNameTemplate { get; set; }

        [DefaultValue("profile_thumbnail_{0}.jpg")]
        public string ProfileThumbnailForBlobNameTemplate { get; set; }

        [DefaultValue(128)]
        public int MaxThumbnailWidth { get; set; }

        [DefaultValue(128)]
        public int MaxThumbnailHeight { get; set; }
    }
}