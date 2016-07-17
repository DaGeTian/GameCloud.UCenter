namespace GF.Manager.UCenter.ApiControllers
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using global::MongoDB.Driver;
    using GF.UCenter.MongoDB;
    using GF.UCenter.MongoDB.Entity;
    using GF.UCenter.Web.Common.Modes;
    using GF.UCenter.MongoDB.Adapters;
    using GF.UCenter.Common.Settings;

    /// <summary>
    /// Provide a controller for users.
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/users")]
    public class UsersController : ApiControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController" /> class.
        /// </summary>
        /// <param name="database">Indicating the database context.</param>
        /// <param name="settings">Indicating the settings.</param>
        [ImportingConstructor]
        public UsersController(DatabaseContext database, Settings settings)
            : base(database, settings)
        {
        }

        /// <summary>
        /// Get user list.
        /// </summary>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <param name="keyword">Indicating the keyword.</param>
        /// <param name="orderby">Indicating the order by name.</param>
        /// <param name="page">Indicating the page number.</param>
        /// <param name="count">Indicating the count.</param>
        /// <returns>Async return user list.</returns>
        public async Task<PaginationResponse<AccountEntity>> Get(
            CancellationToken token,
            [FromUri]string keyword = null,
            [FromUri] string orderby = null,
            [FromUri] int page = 1,
            [FromUri] int count = 1000)
        {
            Expression<Func<AccountEntity, bool>> filter = null;

            if (!string.IsNullOrEmpty(keyword))
            {
                filter = a => a.AccountName.Contains(keyword)
                    || a.Email.Contains(keyword)
                    || a.PhoneNum.Contains(keyword);
            }

            var total = await this.Database.Accounts.CountAsync(filter, token);

            IQueryable<AccountEntity> queryable = this.Database.Accounts.Collection.AsQueryable();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }

            var accounts = queryable.Skip((page - 1) * count).Take(count).ToList();

            // todo: add orderby support.
            var model = new PaginationResponse<AccountEntity>
            {
                Page = page,
                PageSize = count,
                Raws = accounts,
                Total = total
            };

            return model;
        }

        /// <summary>
        /// Get single user details.
        /// </summary>
        /// <param name="id">Indicating the user id.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async return user details.</returns>
        public async Task<AccountEntity> Get(string id, CancellationToken token)
        {
            var account = await this.Database.Accounts.GetSingleAsync(id, token);

            return account;
        }

        /// <summary>
        /// Get single user details.
        /// </summary>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Get new user count</returns>
        [HttpGet]
        [Route("newUserCount")]
        public async Task<long> GetNewUserCount(CancellationToken token)
        {
            return await this.Database.Accounts.CountAsync(a => a.CreatedTime < DateTime.Today, null, token);
        }

        /// <summary>
        /// Get single user details.
        /// </summary>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Get new user count</returns>
        //[HttpGet]
        //[Route("newUserCount")]
        //public async Task<long> GetNewUserCount(CancellationToken token)
        //{
        //    return await this.Database.Accounts.CountAsync(a => a.CreatedTime < DateTime.Today, null, token);
        //}
    }
}
