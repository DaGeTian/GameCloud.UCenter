namespace GF.Manager.TexasPoker.Controllers
{
    using System.ComponentModel.Composition;
    using System.Web.Mvc;
    using UCenter.MongoDB;

    /// <summary>
    /// Provide a class for home controller.
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HomeController : Controller
    {
        private readonly DatabaseContext database;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="database">Indicating the database context</param>
        [ImportingConstructor]
        public HomeController(DatabaseContext database)
        {
            this.database = database;
        }

        /// <summary>
        /// Get the index page.
        /// </summary>
        /// <returns>action result.</returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserList()
        {
            return View();
        }

        public ActionResult RoboList()
        {
            return View();
        }

        public ActionResult BuyChipsEvents() { return View(); }
        public ActionResult ChipBuyGiftEvents() { return View(); }
        public ActionResult ChipBuyItemEvents() { return View(); }
        public ActionResult DailyGetChipsEvents() { return View(); }
        public ActionResult LostAllSendChipsEvents() { return View(); }
        public ActionResult PlayerSendOtherChipsEvents() { return View(); }
        public ActionResult PlayerGetChipsFromOtherEvents() { return View(); }
        public ActionResult BuyCoinsEvents() { return View(); }
        public ActionResult CoinBuyGiftEvents() { return View(); }
        public ActionResult CoinBuyItemEvents() { return View(); }
        public ActionResult BuyVIPEvents() { return View(); }
        public ActionResult TexasPokerEvents() { return View(); }
        public ActionResult PlayerReportEvents() { return View(); }
        

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

        public ActionResult ActivityList()
        {
            return View();
        }

        public ActionResult AppConfig()
        {
            return View();
        }

        public ActionResult ClubList()
        {
            return View();
        }
    }
}