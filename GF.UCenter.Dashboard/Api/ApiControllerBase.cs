using System.ComponentModel.Composition;
using System.Web.Http;
using GF.UCenter.Common.Settings;
using GF.UCenter.CouchBase;

namespace GF.UCenter.Dashboard.Api
{
    [Export]
    public class ApiControllerBase : ApiController
    {
        [ImportingConstructor]
        public ApiControllerBase(CouchBaseContext database, Settings settings)
        {
            this.Database = database;
            this.Settings = settings;
        }

        protected CouchBaseContext Database { get; private set; }

        protected Settings Settings { get; private set; }
    }
}
