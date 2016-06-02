using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace GF.UCenter.MongoDB
{
    [Export]
    public class DatabaseContextSettings
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        // maybe we will need it in the future.
        public MongoCollectionSettings CollectionSettings { get; set; }

        // maybe we will need it in the future.
        public MongoDatabaseSettings DatabaseSettings { get; set; }
    }
}
