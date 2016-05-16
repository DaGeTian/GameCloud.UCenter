namespace GF.UCenter.Web.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using CouchBase;
    using Models;
    using NLog;

    /// <summary>
    /// Home page controller
    /// </summary>
    [Export]
    public class HomeController : Controller
    {
        /// <summary>
        ///     Get the index page
        /// </summary>
        /// <returns>The index page view</returns>
        public ActionResult Index()
        {
            return this.View();
        }

        public ActionResult UserList()
        {
            return this.View();
        }

        public ActionResult SingleUser()
        {
            return this.View();
        }

        public ActionResult OrderList()
        {
            return this.View();
        }

        public ActionResult SingleOrder()
        {
            return this.View();
        }

        /// <summary>
        ///     Get the about page view.
        /// </summary>
        /// <returns>The about page view.</returns>
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return this.View();
        }

        /// <summary>
        ///     Get the contact page view.
        /// </summary>
        /// <returns>The contact page view.</returns>
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return this.View();
        }
    }
}