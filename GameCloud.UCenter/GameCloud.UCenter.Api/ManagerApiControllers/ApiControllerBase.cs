using System.ComponentModel.Composition;
using GameCloud.UCenter.Common.Settings;
using GameCloud.UCenter.Database;
using Microsoft.AspNetCore.Mvc;

namespace GameCloud.UCenter.Api.ManagerApiControllers
{
    /// <summary>
    /// Provide an API controller base class.
    /// </summary>
    [Export]
    public class ManagerApiControllerBase : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerApiControllerBase" /> class.
        /// </summary>
        /// <param name="ucenterDb">Indicating the database.</param>
        /// <param name="ucenterEventDb">Indicating the database.</param>
        /// <param name="settings">Indicating the settings.</param>
        [ImportingConstructor]
        public ManagerApiControllerBase(
            UCenterDatabaseContext ucenterDb,
            UCenterEventDatabaseContext ucenterEventDb,
            Settings settings)
        {
            this.UCenterDatabase = ucenterDb;
            this.UCenterEventDatabase = ucenterEventDb;
            this.Settings = settings;
        }

        /// <summary>
        /// Gets the database context.
        /// </summary>
        protected UCenterDatabaseContext UCenterDatabase { get; private set; }

        /// <summary>
        /// Gets the database context.
        /// </summary>
        protected UCenterEventDatabaseContext UCenterEventDatabase { get; private set; }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        protected Settings Settings { get; private set; }
    }
}
