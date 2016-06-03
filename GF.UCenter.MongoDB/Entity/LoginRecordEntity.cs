using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GF.UCenter.Common.Portable;
using GF.UCenter.MongoDB.Attributes;

namespace GF.UCenter.MongoDB.Entity
{
    [CollectionName("LoginRecord")]
    public class LoginRecordEntity : EntityBase
    {
        public string AccountName { get; set; }

        public string AccountId { get; set; }

        public DateTime LoginTime { get; set; }

        public string ClientIp { get; set; }

        public string UserAgent { get; set; }

        public UCenterErrorCode Code { get; set; }

        public string LoginArea { get; set; }

        public string Comments { get; set; }
    }
}
