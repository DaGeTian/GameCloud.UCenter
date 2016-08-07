using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GF.Manager.Handlers;

namespace GF.Manager.Controllers
{
    public class BaseController : Controller
    {
        public BaseController(PluginManager manager)
        {
            this.Manager = manager;
        }

        public PluginManager Manager { get; private set; }

        // GET: Base
        public ActionResult Index()
        {
            return View();
        }
    }
}