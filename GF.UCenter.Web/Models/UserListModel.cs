using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GF.UCenter.CouchBase;

namespace GF.UCenter.Web.Models
{
    public class UserListModel
    {
        public int Total { get; set; }

        public int Page { get; set; }

        public int Count { get; set; }

        public IEnumerable<AccountEntity> Users { get; set; }
    }
}