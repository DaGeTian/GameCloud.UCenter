using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GF.MongoDB.Common.Attributes;

namespace GF.Manager.Models
{
    [CollectionName("Account")]
    public class AccountEntity
    {
        public string Name { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}