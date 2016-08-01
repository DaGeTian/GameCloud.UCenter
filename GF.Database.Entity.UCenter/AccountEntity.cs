using System;
using GF.Database.Entity.Common;
using GF.Database.Entity.Common.Attributes;
using GF.UCenter.Common.Portable.Models.AppClient;

namespace GF.Database.Entity.UCenter
{
    /// <summary>
    /// Provide a class for account document.
    /// </summary>
    [CollectionName("Account")]
    public class AccountEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the account name.
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the super password.
        /// </summary>
        public string SuperPassword { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the last login date time.
        /// </summary>
        public DateTime LastLoginDateTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the account is gust user.
        /// </summary>
        public bool IsGuest { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the profile image.
        /// </summary>
        public string ProfileImage { get; set; }

        /// <summary>
        /// Gets or sets the profile image thumbnail.
        /// </summary>
        public string ProfileThumbnail { get; set; }

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// Gets or sets the identity number.
        /// </summary>
        public string IdentityNum { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public string PhoneNum { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string Email { get; set; }
    }
}