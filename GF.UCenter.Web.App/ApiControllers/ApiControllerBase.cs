namespace GF.UCenter.Web.App.ApiControllers
{
    using System.ComponentModel.Composition;
    using System.Web.Http;
    using MongoDB;
    using UCenter.Common.Settings;

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
