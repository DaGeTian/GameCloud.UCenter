using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Couchbase.N1QL;
using GF.UCenter.CouchBase;
using GF.UCenter.CouchBase.Models;
using GF.UCenter.Web.Models;

namespace GF.UCenter.Web.ApiControllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class OrdersController : ApiControllerBase
    {
        [ImportingConstructor]
        public OrdersController(CouchBaseContext db)
            : base(db)
        {
        }

        public async Task<PaginationResponse<OrderRaw>> Get([FromUri] string accountId = null, [FromUri]string keyword = null, [FromUri] string orderby = null, [FromUri] int page = 1, [FromUri] int count = 1000)
        {
            var expression = string.IsNullOrEmpty(accountId)
                ? new PaginationQueryExpression<OrderEntity>(page, count)
                : new PaginationQueryExpression<OrderEntity>(page, count, o => o.AccountId == accountId);

            if (!string.IsNullOrEmpty(keyword))
            {
                expression.AddLikeItem(e => e.AccountId, keyword);
                expression.AddLikeItem(e => e.AppId, keyword);
                expression.AddLikeItem(e => e.RawData, keyword);
            }

            if (!string.IsNullOrEmpty(orderby))
            {
                expression.AddOrderByItem(orderby, OrderByType.ASC);
            }

            var ordersResponse = await this.DatabaseContext.Bucket.PaginationQuerySlimAsync<OrderEntity>(expression);


            var accountIds = ordersResponse.Raws.Where(o => o.AccountId != null).Select(o => o.AccountId).Distinct().ToList();
            List<AccountEntity> accounts = new List<AccountEntity>();

            // get one by one for did not find how to use 'in condition' in N1QL
            if (accountIds.Count() > 0)
            {
                // use foreach here is because once used linq expression, the stupid couchbase query will return nothing and without any exception.
                // you even don't know what happened. it seemed like the query never happened.
                // use foreach to try to get recored one by one and hope can catch more details. it passed.
                foreach (var id in accountIds)
                {
                    var account = await this.DatabaseContext.Bucket.FirstOrDefaultAsync<AccountEntity>(a => a.Id == id, false);
                    if (account != null)
                    {
                        accounts.Add(account);
                    }
                }
            }

            var response = new PaginationResponse<OrderRaw>()
            {
                Page = ordersResponse.Page,
                PageSize = ordersResponse.PageSize,
                Total = ordersResponse.Total,
                Raws = ordersResponse.Raws.Select(o =>
                {
                    var raw = new OrderRaw();
                    raw.AccountId = o.AccountId;
                    var account = accounts.FirstOrDefault(a => a.Id == o.AccountId);
                    if (account != null)
                    {
                        raw.AccountName = account.AccountName;
                    }

                    raw.AppId = o.AppId;
                    raw.CreatedTime = o.CreatedTime;
                    raw.CompletedTime = o.CompletedTime;
                    raw.RawData = o.RawData;
                    raw.OrderStatus = o.OrderStatus;
                    raw.OrderId = o.Id;

                    return raw;
                })
            };

            return response;
        }
    }
}