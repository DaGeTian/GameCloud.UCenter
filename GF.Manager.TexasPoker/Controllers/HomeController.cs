using GF.Database;

namespace GF.Manager.TexasPoker.Controllers
{
    using System.ComponentModel.Composition;
    using System.Web.Mvc;
    using global::TexasPoker.Database;
    using MongoDB;

    /// <summary>
    /// Provide a class for home controller.
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HomeController : ControllerBase
    {
        private readonly TexasPokerDatabaseContext database;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="database">Indicating the database context</param>
        [ImportingConstructor]
        public HomeController(TexasPokerDatabaseContext database)
        {
            this.database = database;
        }

        /// <summary>
        /// Get the index page.
        /// </summary>
        /// <returns>action result.</returns>
        public ActionResult Index()
        {
            return View("PlayerList");
        }

        public ActionResult PlayerList()
        {
            return View();
        }

        public ActionResult BotList()
        {
            return View();
        }

        public ActionResult EventReport()
        {
            return View();
        }

        public ActionResult Analytics()
        {
            return View();
        }

        public ActionResult PlatformChargeRate()
        {
            return View();
        }

        public ActionResult AppConfig()
        {
            return View();
        }
    }
}