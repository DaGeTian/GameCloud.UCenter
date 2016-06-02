using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GF.UCenter.MongoDB.Adapters;
using GF.UCenter.MongoDB.Entity;
using MongoDB.Driver;

namespace GF.UCenter.MongoDB
{
    [Export]
    public class DatabaseContext
    {
        private readonly DatabaseContextSettings settings;
        private readonly IMongoClient client;
        private readonly IMongoDatabase database;

        [ImportingConstructor]
        private DatabaseContext(DatabaseContextSettings settings)
        {
            this.settings = settings;
            this.client = new MongoClient(settings.ConnectionString);
            this.database = client.GetDatabase(this.settings.DatabaseName);
        }

        public IMongoDatabase Database
        {
            get
            {
                return this.database;
            }
        }

        public DatabaseContextSettings Settings
        {
            get { return this.settings; }
        }
    }
}
