namespace GF.UCenter.Dashboard.Controllers
{
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using GF.UCenter.CouchBase;

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HomeController : Controller
    {
        private readonly CouchBaseContext db;

        [ImportingConstructor]
        public HomeController(CouchBaseContext db)
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

        public async Task<ActionResult> OrderList(string accountId = null)
        {
            if (!string.IsNullOrEmpty(accountId))
            {
                var account = await db.Bucket.FirstOrDefaultAsync<AccountEntity>(a => a.Id == accountId, false);
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