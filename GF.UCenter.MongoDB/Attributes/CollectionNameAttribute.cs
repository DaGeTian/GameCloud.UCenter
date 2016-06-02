using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GF.UCenter.MongoDB.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CollectionNameAttribute : Attribute
    {
        public CollectionNameAttribute(string collectionName)
        {
            this.CollectionName = collectionName;
        }

        public string CollectionName { get; private set; }
    }
}
