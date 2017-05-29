using System;
using GameCloud.Database;
using GameCloud.Database.Attributes;
using GameCloud.UCenter.Common.Portable.Models.AppClient;

namespace GameCloud.UCenter.Database.Entities
{
    [CollectionName("AccountWechat")]
    public class AccountWechatEntity : EntityBase
    {
        public string AccountId { get; set; }
        public string Unionid { get; set; }
        public string OpenId { get; set; }
        public string AppId { get; set; }
        public string NickName { get; set; }
        public Gender Gender { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Headimgurl { get; set; }
    }
}