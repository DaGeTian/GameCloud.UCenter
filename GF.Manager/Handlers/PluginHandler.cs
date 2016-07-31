using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using GF.Manager.Models;

namespace GF.Manager.Handlers
{
    public class PluginHandler
    {
        private readonly HttpClient client = new HttpClient();

        private readonly Plugin plugin;
        private readonly PluginItem pluginItem;

        public PluginHandler(Plugin plugin, PluginItem pluginItem)
        {
            this.plugin = plugin;
            this.pluginItem = pluginItem;
        }

        public Task<object> SendAsync<TContent>(string parameters, TContent content)
        {
            HttpContent httpContent = null;
            if (content is HttpContent)
            {
                httpContent = content as HttpContent;
            }
            else
            {
                httpContent = new ObjectContent<TContent>(content, new JsonMediaTypeFormatter());
            }

            var url = $"{this.plugin.ServerUrl}/{this.pluginItem.ControllerName}/{this.pluginItem.ActionName}/?{parameters}";

            return this.SendAsync<object>(this.pluginItem.Method, url, httpContent);
        }

        private async Task<TResponse> SendAsync<TResponse>(HttpMethod method, string url, HttpContent content)
        {
            var request = new HttpRequestMessage(method, new Uri(url));
            request.Headers.Clear();
            request.Headers.ExpectContinue = false;
            request.Content = content;

            var response = await this.client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<TResponse>();
        }
    }
}