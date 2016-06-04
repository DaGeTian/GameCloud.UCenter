namespace GF.UCenter.Web.App.Controllers
{
    using System.ComponentModel.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using MongoDB;
    using MongoDB.Adapters;

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HomeController : Controller
    {
        private readonly DatabaseContext db;

        [ImportingConstructor]
        public HomeController(DatabaseContext db)
        {
            this.db = db;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserList()
        {
            return this.View();
        }

        public ActionResult SingleUser()
        {
            return this.View();
        }

        public async Task<ActionResult> OrderList(CancellationToken token, string accountId = null)
        {
            if (!string.IsNullOrEmpty(accountId))
            {
                var account = await this.db.Accounts.GetSingleAsync(accountId, token);
                ViewBag.AccountId = account.Id;
                ViewBag.AccountName = account.AccountName;
            }

            return this.View();
        }

        public ActionResult SingleOrder(string id)
        {
            ViewBag.OrderId = id;
            return this.View();
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