

using GF.Database;

namespace GF.Manager.TexasPoker.Controllers
{
    using System.ComponentModel.Composition;
    using System.Web.Mvc;

    /// <summary>
    /// Provide a class for home controller.
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EventController : ControllerBase
    {
        private readonly DatabaseContext database;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="database">Indicating the database context</param>
        [ImportingConstructor]
        public EventController(DatabaseContext database)
        {
            this.database = database;
        }

        public ActionResult BuyChipsEvents() { return View(); }
        public ActionResult BuyCoinsEvents() { return View(); }
        public ActionResult BuyVIPEvents() { return View(); }
        public ActionResult ChipBuyGiftEvents() { return View(); }
        public ActionResult CoinBuyGiftEvents() { return View(); }
        public ActionResult ChipBuyItemEvents() { return View(); }
        public ActionResult CoinBuyItemEvents() { return View(); }
        public ActionResult ChipsChangeByManagerEvents() { return View(); }
        public ActionResult DailyGetChipsEvents() { return View(); }
        public ActionResult LostAllSendChipsEvents() { return View(); }
        public ActionResult PlayerGetChipsFromOtherEvents() { return View(); }
        public ActionResult PlayerReportEvents() { return View(); }
        public ActionResult PlayerSendOtherChipsEvents() { return View(); }
        public ActionResult TexasPokerEvents() { return View(); }
    }
}