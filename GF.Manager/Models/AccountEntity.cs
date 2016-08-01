using GF.Database.Entity.Common.Attributes;

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