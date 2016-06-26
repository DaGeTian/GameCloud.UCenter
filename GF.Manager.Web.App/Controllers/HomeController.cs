namespace GF.Manager.Web.App.Controllers
{
    using System.ComponentModel.Composition;
    using Microsoft.AspNetCore.Mvc;

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HomeController : Controller
    {
        [ImportingConstructor]
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// Get the user list page.
        /// </summary>
        /// <returns>action result.</returns>
        public IActionResult UserList()
        {
            return View();
        }
    }
}
