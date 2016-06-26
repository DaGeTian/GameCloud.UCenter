using System.ComponentModel.Composition.Hosting;
using GF.UCenter.Common;

namespace GF.Manager.Web.App.ApiControllers
{
    using System.ComponentModel.Composition;
    using Microsoft.AspNetCore.Mvc;
    using UCenter.Common.Settings;
    using UCenter.MongoDB;

    /// <summary>
    /// Provide an API controller base class.
    /// </summary>
    [Export]
    public class ApiControllerBase : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiControllerBase" /> class.
        /// </summary>
        /// <param name="database">Indicating the database.</param>
        /// <param name="settings">Indicating the settings.</param>
        [ImportingConstructor]
        public ApiControllerBase(DatabaseContext database, Settings settings)
        {
            //Database = database;
            //Settings = settings;
            var exportProvider = CompositionContainerFactory.Create();

            SettingsInitializer.Initialize<Settings>(
                exportProvider,
                SettingsDefaultValueProvider<Settings>.Default,
                AppConfigurationValueProvider.Default);

            SettingsInitializer.Initialize<DatabaseContextSettings>(
                exportProvider,
                SettingsDefaultValueProvider<DatabaseContextSettings>.Default,
                AppConfigurationValueProvider.Default);

            Database = exportProvider.GetExportedValue<DatabaseContext>();
            Settings = exportProvider.GetExportedValue<Settings>();
        }

        /// <summary>
        /// Gets the database context.
        /// </summary>
        protected DatabaseContext Database { get; private set; }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        protected Settings Settings { get; private set; }
    }
}
