
using System.ComponentModel.Composition;

namespace GF.Manager.Web.App.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using UCenter.MongoDB;
    using UCenter.MongoDB.Adapters;

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HomeController : Controller
    {
        private readonly DatabaseContext database;

        [ImportingConstructor]
        public HomeController(DatabaseContext database)
        {
            this.database = database;
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

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
            return this.View();
        }

        /// <summary>
        /// Get the order list page.
        /// </summary>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <param name="accountId">Indicating the account id.</param>
        /// <returns>action result.</returns>
        public async Task<IActionResult> OrderList(CancellationToken token, string accountId = null)
        {
            if (!string.IsNullOrEmpty(accountId))
            {
                var account = await this.database.Accounts.GetSingleAsync(accountId, token);
                ViewBag.AccountId = account.Id;
                ViewBag.AccountName = account.AccountName;
            }

            return this.View();
        }

        /// <summary>
        /// Gets the single order page.
        /// </summary>
        /// <param name="id">indicating the order id.</param>
        /// <returns>action result.</returns>
        public IActionResult SingleOrder(string id)
        {
            ViewBag.OrderId = id;
            return this.View();
        }

    }
}
