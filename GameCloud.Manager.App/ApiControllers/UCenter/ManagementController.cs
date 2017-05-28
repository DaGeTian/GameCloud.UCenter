
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using GameCloud.Manager.PluginContract.Models;
using GameCloud.Manager.PluginContract.Requests;
using GameCloud.UCenter.Common.Settings;
using GameCloud.UCenter.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace GameCloud.UCenter.Api.Manager.ApiControllers
{
    [Export]
    public class ManagementController : ManagerApiControllerBase
    {
        private readonly IHostingEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementController" /> class.
        /// </summary>
        /// <param name="ucenterDb">Indicating the database context.</param>
        /// <param name="ucenterventDb">Indicating the database context.</param>
        /// <param name="settings">Indicating the settings.</param>
        /// <param name="environment">Indicating the hosting environment.</param>
        [ImportingConstructor]
        public ManagementController(
            UCenterDatabaseContext ucenterDb,
            UCenterEventDatabaseContext ucenterventDb,
            Settings settings,
            IHostingEnvironment environment)
            : base(ucenterDb, ucenterventDb, settings)
        {
            this.environment = environment;
        }

        [HttpPost]
        [Route("api/ucenter/configcenter")]
        public IReadOnlyList<PluginAppSetting> ManageAppSettings([FromBody]UpdateRequestInfo<PluginAppSetting[]> request)
        {
            if (request.Method == PluginRequestMethod.Read)
            {
                return this.GetCurrentAppSettings();
            }
            else if (request.Method == PluginRequestMethod.Update)
            {
                this.UpdateAppSettings(request.Body);

                return this.GetCurrentAppSettings();
            }
            else
            {
                throw new NotSupportedException($"Not support request method: {request.Method}");
            }
        }

        private IReadOnlyList<PluginAppSetting> GetCurrentAppSettings()
        {
            var appConfigFile = this.GetAppConfigFile();
            XDocument xdoc = XDocument.Load(appConfigFile);
            var appSettingsNode = xdoc.XPathSelectElement("configuration/appSettings");
            var settings = appSettingsNode
                .Elements()
                .Select(e => new PluginAppSetting(
                    e.Attribute("key").Value,
                    e.Attribute("value").Value
                    ))
                .ToList();

            return settings;
        }

        private void UpdateAppSettings(IReadOnlyList<PluginAppSetting> settings)
        {
            var appConfigFile = this.GetAppConfigFile();

            XDocument xdoc = XDocument.Load(appConfigFile);
            var appSettingsNode = xdoc.XPathSelectElement("configuration/appSettings");
            foreach (var setting in settings)
            {
                var node = appSettingsNode.XPathSelectElement($"add[@key='{setting.Key}']");
                if (node != null)
                {
                    node.SetAttributeValue("value", setting.Value);
                }
                else
                {
                    node = new XElement("add");
                    node.SetAttributeValue("key", setting.Key);
                    node.SetAttributeValue("value", setting.Value);
                    appSettingsNode.Add(node);
                }
            }

            xdoc.Save(appConfigFile);
        }

        private string GetAppConfigFile()
        {
            var path = Path.Combine(this.environment.ContentRootPath, "app.config");
            if (!System.IO.File.Exists(path))
            {
                var fileName = new FileInfo(this.GetType().Assembly.Location).Name + ".config";
                path = Path.Combine(this.environment.ContentRootPath, fileName);
            }

            return path;
        }
    }
}
