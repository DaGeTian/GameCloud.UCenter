using System.ComponentModel;
using System.ComponentModel.Composition;

namespace GameCloud.UCenter
{
    [Export]
    public class Settings
    {
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