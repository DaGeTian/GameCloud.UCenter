using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GF.Manager.Handlers;
using GF.Manager.Models;

namespace GF.Manager.Controllers
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class HomeController : Controller
    {
        private readonly PluginManager manager;

        [ImportingConstructor]
        public HomeController(PluginManager manager)
        {
            this.manager = manager;
        }

        public AccountEntity CurrentUser
        {
            get
            {
                // todo: return current login user.
                return new AccountEntity()
                {
                    Name = "fake",
                };
            }
        }

        private Task<object> Post(Guid pluginId, string parameters, string body)
        {
            var hander = manager.GetHandler(pluginId);
            return hander.SendAsync(parameters, body);
        }

        public async Task<ActionResult> Index(CancellationToken token)
        {
            var plugins = await this.manager.GetPlugins(this.CurrentUser.Name, token);
            return View(plugins);
        }

        [HttpGet]
        public ActionResult Plugin()
        {
            // var plugin = await this.manager.GetPlugin(this.CurrentUser.Name, name, token);
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(AccountEntity account, string returnUrl)
        {
            // todo: validate account.
            if (false)
            {
                return View(account);
            }

            // todo: login user.

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}