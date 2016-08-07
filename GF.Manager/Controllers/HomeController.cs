using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GF.Manager.Handlers;
using GF.Manager.Models;
using GF.UCenter.Web.Common.Logger;
using Newtonsoft.Json;

namespace GF.Manager.Controllers
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class HomeController : BaseController
    {
        [ImportingConstructor]
        public HomeController(PluginManager manager)
            : base(manager)
        {
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

        [HttpGet]
        public ActionResult Index(CancellationToken token)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Plugin(string name)
        {
            Plugin plugin = null;
            if (!string.IsNullOrEmpty(name))
            {
                plugin = this.Manager.Plugins.FirstOrDefault(p => p.Name.ToLowerInvariant() == name);
            }

            ViewBag.PluginName = name;

            // var plugin = await this.manager.GetPlugin(this.CurrentUser.Name, name, token);
            return View(plugin);
        }

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

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                var actionName = filterContext.ActionDescriptor.ActionName.ToLower();
                if (actionName.Contains("plugin") || actionName == "index")
                {
                    this.ViewBag.StartupScript =
                            string.Format(CultureInfo.InvariantCulture,
                            "var $plugins={0};",
                            JsonConvert.SerializeObject(this.Manager.Plugins));
                }
            }
            catch (Exception ex)
            {
                CustomTrace.TraceError(ex, "Inject plugin json failed");
            }

            base.OnActionExecuted(filterContext);
        }
    }
}