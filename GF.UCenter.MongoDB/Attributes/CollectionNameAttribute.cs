namespace GF.UCenter.MongoDB.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CollectionNameAttribute : Attribute
    {
        public CollectionNameAttribute(string collectionName)
        {
            this.CollectionName = collectionName;
        }

        public string CollectionName { get; private set; }
    }
}
