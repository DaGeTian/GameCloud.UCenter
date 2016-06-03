using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GF.UCenter.MongoDB.Attributes;

namespace GF.UCenter.MongoDB.Entity
{
    [CollectionName("KeyPlaceholder")]
    public class KeyPlaceholderEntity : EntityBase
    {
        public string Name { get; set; }

        public KeyType Type { get; set; }

        public string AccountId { get; set; }

        public string AccountName { get; set; }
    }

    public enum KeyType
    {
        Name,
        Phone,
        Email
    }
}
