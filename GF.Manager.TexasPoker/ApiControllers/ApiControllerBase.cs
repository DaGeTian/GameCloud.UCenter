namespace GF.Manager.TexasPoker.ApiControllers
{
    using Database;
    using System.ComponentModel.Composition;
    using System.Web.Http;
    using UCenter.Common.Settings;
    using UCenter.Web.Common;
    using global::TexasPoker.Database;

    /// <summary>
    /// Provide an API controller base class.
    /// </summary>
    [Export]
    [ActionExecutionFilter]
    public class ApiControllerBase : ApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiControllerBase" /> class.
        /// </summary>
        /// <param name="database">Indicating the database.</param>
        /// <param name="settings">Indicating the settings.</param>
        [ImportingConstructor]
        public ApiControllerBase(TexasPokerDatabaseContext database, Settings settings)
        {
            this.Database = database;
            this.Settings = settings;
        }

        /// <summary>
        /// Gets the database context.
        /// </summary>
        protected TexasPokerDatabaseContext Database { get; private set; }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        protected Settings Settings { get; private set; }
    }
}
