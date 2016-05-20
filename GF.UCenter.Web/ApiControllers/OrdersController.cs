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

        public async Task<ListModel<OrderRaw>> Get([FromUri] string accountId = null, [FromUri]string keyword = null, [FromUri] string orderby = null, [FromUri] int page = 1, [FromUri] int count = 1000)
        {
            // use the following stupid logic because join query always give empty result in couchbase.
            ListModel<OrderRaw> result;
            string where = $"type='{OrderEntity.DocumentType}'";
            if (!string.IsNullOrEmpty(accountId))
            {
                where = $"{where} AND accountId='{accountId}'";
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                where = $"{where} AND appId LIKE '%{keyword}%' AND orderStatus LIKE '%{keyword}%' AND rawData LIKE '%{keyword}%'";
            }

            string taken = $"LIMIT {count} OFFSET {(page - 1) * count}";
            string totalCountQuery = $"SELECT COUNT(1) FROM {this.DatabaseContext.Bucket.Name} WHERE {where};";
            string rawsQuery = $"SELECT {this.DatabaseContext.Bucket.Name}.* FROM {this.DatabaseContext.Bucket.Name} WHERE {where} {taken};";

            var totalCountRaws = await this.DatabaseContext.Bucket.QuerySlimAsync<CountRaw>(totalCountQuery, true);
            var total = totalCountRaws.First().Count;
            var orderRaws = await this.DatabaseContext.Bucket.QuerySlimAsync<OrderEntity>(rawsQuery, true);
            var accountIds = orderRaws.Where(o => o.AccountId != null).Select(o => o.AccountId).Distinct();
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

            result = new ListModel<OrderRaw>();
            result.Total = total;
            result.Page = page;
            result.Count = orderRaws.Count();
            var raws = new List<OrderRaw>();
            foreach (var o in orderRaws)
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

                raws.Add(raw);
            }

            result.Raws = raws;

            return result;
        }
    }
}