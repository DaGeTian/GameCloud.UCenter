using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using GF.Manager.Contract;
using GF.Manager.Handlers;
using GF.Manager.Models;
using GF.UCenter.Web.Common.Logger;

namespace GF.Manager.ApiControllers
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class PluginsController : ApiController
    {
        private readonly HttpClient client = new HttpClient();
        private readonly PluginManager manager;

        [ImportingConstructor]
        public PluginsController(PluginManager manager)
        {
            this.manager = manager;
        }

        public IReadOnlyList<Plugin> Get(CancellationToken token)
        {
            return this.manager.Plugins;
        }

        [HttpPost]
        [Route("pluginrequest")]
        public async Task<object> DoWork([FromBody]PluginRequest request, CancellationToken token)
        {
            var plugin = this.manager.Plugins.Where(p => p.Name == request.PluginName).First();
            var item = plugin.GetItem(request.Item);

            var parameters = request.Content
                .Select(kv => new PluginRequestParameter() { Name = kv.Key, Value = kv.Value })
                .ToList();
            var requestInfo = new PluginRequestInfo(request.Method, parameters);
            try
            {
                var result = item.EntryMethod.Invoke(plugin.EntryPoint, new object[] { requestInfo });
                if (result != null && result.GetType().IsSubclassOf(typeof(Task)))
                {
                    var task = result as Task;
                    return await (dynamic)task;
                }

                return result;
            }
            catch (Exception ex)
            {
                CustomTrace.TraceError(ex, "Execute plugin action error: Plugin:{0}, Item: {2}", plugin.Name, item.Name);
                return null;
            }
        }
    }
}
