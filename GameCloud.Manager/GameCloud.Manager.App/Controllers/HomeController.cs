using System;
using System.Globalization;
using System.IO;
using GameCloud.Manager.App.Common;
using GameCloud.Manager.App.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GameCloud.Manager.App.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IOptions<AppSettings> appSettings;
        private readonly PluginManager manager;

        public HomeController(
            IHostingEnvironment hostingEnvironment,
            IOptions<AppSettings> appSettings,
            PluginManager manager
            )
        {
            this.hostingEnvironment = hostingEnvironment;
            this.appSettings = appSettings;
            this.manager = manager;
        }

        public IActionResult Index()
        {
            ViewData["BuildVersion"] = "v" + appSettings.Value.BuildVersion;
            return View(this.manager.Plugins);
        }

        public IActionResult Plugin(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var plugin = this.manager.GetPlugin(name);
                if (plugin != null)
                {
                    return View(plugin);
                }
            }

            return NotFound();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
