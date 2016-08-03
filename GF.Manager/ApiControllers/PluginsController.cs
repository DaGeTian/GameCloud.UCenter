using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using GF.Manager.Models;

namespace GF.Manager.ApiControllers
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class PluginsController : ApiController
    {
        private readonly HttpClient client = new HttpClient();

        public async Task<IReadOnlyList<Plugin>> Get(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task<Plugin> Get(string name)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("getdata")]
        public async Task<HttpResponseMessage> GetData([FromBody]PluginRequest request, CancellationToken token)
        {
            var req = new HttpRequestMessage(new HttpMethod(request.Method), new Uri(request.Url));
            req.Headers.Clear();
            req.Headers.ExpectContinue = false;
            if (!string.IsNullOrEmpty(request.Content))
            {
                req.Content = new StringContent(request.Content);
            }

            var response = await this.client.SendAsync(req);

            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}
