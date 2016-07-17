using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GF.Manager.TexasPoker.Controllers
{
    public class HomeController : Controller
    {
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
        public ActionResult CardGameEvents() { return View(); }

        public ActionResult EventReport()
        {
            return View();
        }

        public ActionResult Analytics()
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