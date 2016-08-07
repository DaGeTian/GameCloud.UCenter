using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using GF.Database;
using GF.Manager.Contract;
using GF.Manager.Models;
using GF.UCenter.Web.Common.Logger;
using Newtonsoft.Json;

namespace GF.Manager.Handlers
{
    [Export]
    public class PluginManager
    {
        private IReadOnlyList<Plugin> plugins;
        private DateTime lastUpdateTime = DateTime.MinValue;
        private TimeSpan updateInterval = TimeSpan.FromMinutes(10);
        private readonly ConcurrentDictionary<string, PluginHandler> handlers = new ConcurrentDictionary<string, PluginHandler>();
        private readonly Regex manifestRegex = new Regex(@"manifest\.((?<path>views|scripts)\.)?(?<file>[^.]+\.[^.]+)$");
        private readonly DatabaseContext context;

        [ImportingConstructor]
        public PluginManager(DatabaseContext context)
        {
            this.context = context;
        }

        public IReadOnlyList<Plugin> Plugins
        {
            get
            {
                if (this.plugins == null)
                {
                    this.plugins = this.LoadPlugins();
                }

                return this.plugins;
            }
        }

        private IReadOnlyList<Plugin> LoadPlugins()
        {
            var path = HttpRuntime.AppDomainAppPath;
            // var path = Path.GetDirectoryName(this.GetType().Assembly.Location);
            var dlls = Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories);
            var manifestPath = Path.Combine(path, "plugins");
            var plugins = new List<Plugin>();
            foreach (var dll in dlls)
            {
                var plugin = this.LoadPlugin(dll, manifestPath);
                if (plugin != null)
                {
                    plugins.Add(plugin);
                }
            }

            return plugins;
        }

        private Plugin LoadPlugin(string assemblyFile, string manifestPath)
        {
            try
            {
                var baseType = typeof(PluginEntryPoint);
                var assembly = Assembly.LoadFrom(assemblyFile);

                var entryPoints = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType)).ToList();
                if (entryPoints.Count > 1)
                {
                    CustomTrace.TraceError("Too much plugins in one assembly :{0}, Types: {1}",
                        assembly.FullName,
                        string.Join(",", entryPoints.Select(e => e.Name)));

                    return null;
                }

                var entryPoint = entryPoints[0];

                var pluginAttribute = entryPoint.GetCustomAttribute<PluginMetadataAttribute>();
                if (pluginAttribute == null)
                {
                    CustomTrace.TraceError("Type: {0} have no plugin metadata setting.", entryPoint.FullName);
                    return null;
                }

                this.InitPluginManifest(pluginAttribute.Name, manifestPath, assembly);

                Plugin plugin = new Plugin();
                plugin.EntryPoint = Activator.CreateInstance(entryPoint) as PluginEntryPoint;
                plugin.Name = pluginAttribute.Name;
                plugin.DisplayName = pluginAttribute.DisplayName;
                plugin.Description = pluginAttribute.Description;
                plugin.Items = new List<PluginItem>();
                plugin.Categories = new List<PluginCategory>();

                var categoryAttributes = entryPoint.GetCustomAttributes<PluginCategoryMetadataAttribute>();

                foreach (var attr in categoryAttributes)
                {
                    var category = new PluginCategory()
                    {
                        Name = attr.Name,
                        DisplayName = attr.DisplayName,
                        Description = attr.Description,
                        Items = new List<PluginItem>()
                    };

                    plugin.Categories.Add(category);
                }

                var methods = entryPoint.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Where(m => m.GetCustomAttribute<PluginItemMetadataAttribute>() != null)
                    .ToList();

                foreach (var method in methods)
                {
                    var attr = method.GetCustomAttribute<PluginItemMetadataAttribute>();
                    var parameters = method.GetParameters();
                    if (parameters.Count() != 1)
                    {
                        CustomTrace.TraceError("item {0} parameter invalid.", attr.Name);
                    }

                    var item = new PluginItem()
                    {
                        Name = attr.Name,
                        DisplayName = attr.DisplayName,
                        Description = attr.Description,
                        EntryMethod = method,
                        View = attr.View ?? attr.Name + ".html",
                        Type = attr.Type,
                        Controller = attr.Controller
                    };

                    var category = plugin.Categories.FirstOrDefault(c => c.Name == attr.Category);
                    if (category != null)
                    {
                        category.Items.Add(item);
                    }
                    else
                    {
                        plugin.Items.Add(item);
                    }
                }

                return plugin;
            }
            catch (Exception ex)
            {
                CustomTrace.TraceError("Load plugin assembly: {0} error: {1}", assemblyFile, ex);
                return null;
            }
        }

        private void InitPluginManifest(string pluginName, string manifestPath, Assembly assembly)
        {
            var resources = assembly.GetManifestResourceNames();
            var assemblyName = assembly.GetName().Name;

            var manifestFolder = Path.Combine(manifestPath, pluginName);
            foreach (var resource in resources)
            {
                if (!manifestRegex.IsMatch(resource))
                {
                    continue;
                }

                var match = manifestRegex.Match(resource);
                var path = Path.Combine(manifestFolder, match.Groups["path"].Value);
                Directory.CreateDirectory(path);
                var filePath = Path.Combine(path, match.Groups["file"].Value);
                using (Stream stream = assembly.GetManifestResourceStream(resource))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string content = reader.ReadToEnd();
                    File.WriteAllText(filePath, content);
                }
            }
        }
    }
}