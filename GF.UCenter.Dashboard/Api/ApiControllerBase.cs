using System.ComponentModel.Composition;
using System.Web.Http;
using GF.UCenter.Common.Settings;
using GF.UCenter.MongoDB;

namespace GF.UCenter.Dashboard.Api
{
    [Export]
    public class ApiControllerBase : ApiController
    {
        [ImportingConstructor]
        public ApiControllerBase(DatabaseContext database, Settings settings)
        {
            this.Database = database;
            this.Settings = settings;
        }

        protected DatabaseContext Database { get; private set; }

        protected Settings Settings { get; private set; }
    }
}
