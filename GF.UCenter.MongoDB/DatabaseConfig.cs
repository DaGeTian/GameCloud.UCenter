namespace GF.UCenter.MongoDB
{
    using System.Collections.Generic;
    using System.Security.Authentication;
    using global::MongoDB.Driver;

    public class DatabaseConfig
    {
        private readonly string mechanism = "SCRAM-SHA-1";
        private string connectionString;
        private string databaseName;
        private string host;
        private int port;
        private string username;
        private string password;
        private bool useSsl;

        public DatabaseConfig(string connectionString, string databaseName)
        {
            this.connectionString = connectionString;
            this.databaseName = databaseName;
            SetupConfig();
        }

        public MongoClientSettings MongoClientSettings { get; set; }

        private void SetupConfig()
        {
            var dbType = connectionString.Contains(".documents.azure.com:") ? DatabaseType.DocumentDB : DatabaseType.MongoDB;
            if (dbType == DatabaseType.DocumentDB)
            {
                // DocumentDB Connection string example
                // mongodb://username:password@host:port/[database]?ssl=true
                string connectionValue = connectionString.Substring(10, connectionString.Length - 10);
                this.username = connectionValue.Substring(0, connectionValue.IndexOf(":"));
                connectionValue = connectionValue.Remove(0, this.username.Length + 1);
                this.password = connectionValue.Substring(0, connectionValue.IndexOf("@"));
                connectionValue = connectionValue.Remove(0, this.password.Length + 1);
                this.host = connectionValue.Substring(0, connectionValue.IndexOf(":"));
                connectionValue = connectionValue.Remove(0, this.host.Length + 1);
                if (connectionValue.IndexOf("/") >= 0)
                {
                    this.port = int.Parse(connectionValue.Substring(0, connectionValue.IndexOf("/")));
                }
                else
                {
                    this.port = int.Parse(connectionValue);
                }

                int index_SSL = connectionValue.IndexOf("?ssl=");
                if (index_SSL >= 0)
                {
                    if (connectionValue.Substring(index_SSL + 5, 4).Equals("true"))
                    {
                        this.useSsl = true;
                    }
                }

                this.MongoClientSettings = new MongoClientSettings();
                this.MongoClientSettings.Server = new MongoServerAddress(this.host, this.port);
                this.MongoClientSettings.UseSsl = this.useSsl;

                // DocumentDB needs to use SSL
                if (this.useSsl)
                {
                    this.MongoClientSettings.SslSettings = new SslSettings();
                    this.MongoClientSettings.SslSettings.EnabledSslProtocols = SslProtocols.Tls12;

                    MongoIdentity identity = new MongoInternalIdentity(databaseName, this.username);
                    MongoIdentityEvidence evidence = new PasswordEvidence(this.password);

                    this.MongoClientSettings.Credentials = new List<MongoCredential>()
                    {
                        new MongoCredential(mechanism, identity, evidence)
                    };
                }
            }
            else
            {
                // MongoDB connection string example
                // mongodb://host:port/[database]
                string connectionValue = connectionString.Substring(10, connectionString.Length - 10);
                this.host = connectionValue.Substring(0, connectionValue.IndexOf(":"));
                connectionValue = connectionValue.Remove(0, this.host.Length + 1);
                if (connectionValue.IndexOf("/") >= 0)
                {
                    this.port = int.Parse(connectionValue.Substring(0, connectionValue.IndexOf("/")));
                }
                else
                {
                    this.port = int.Parse(connectionValue);
                }

                this.MongoClientSettings = new MongoClientSettings();
                this.MongoClientSettings.Server = new MongoServerAddress(this.host, this.port);
            }
        }
    }
}
