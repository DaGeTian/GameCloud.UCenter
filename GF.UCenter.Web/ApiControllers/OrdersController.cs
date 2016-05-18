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
            string queryString = @"SELECT o.id as OrderId, o.accountId, o.appId, o.orderStatus, o.orderData,o.createdTime, o.finishTime, a.accountName as AccountName 
 From ucenter o INNER JOIN ucenter a
ON KEYS o.accountId 
WHERE o.type='Order' AND a.type='Account'";

            if (!string.IsNullOrEmpty(accountId))
            {
                queryString += $" AND o.accountId='{accountId}'";
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                queryString += $" AND (o.id like $1 OR o.accountId like '$1 OR o.appId like $1 OR o.orderData like $1 OR o.status like $1 )";
            }

            var request = new QueryRequest();
            request.Statement(queryString);
            request.AddNamedParameter("$1", $"%{keyword}%");
            var result = await this.DatabaseContext.Bucket.QueryAsync<OrderRaw>(request);
            List<OrderRaw> raws = result.Success ? result.Rows : new List<OrderRaw>();

            var total = raws.Count();
            raws = raws.Skip((page - 1) * count).Take(count).ToList();

            return new ListModel<OrderRaw>()
            {
                Total = total,
                Count = raws.Count,
                Page = page,
                Raws = raws
            };
        }

    }
}