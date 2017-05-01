using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.Manager.App.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace GameCloud.Manager.App.Manager
{
    public class PluginClient
    {
        private readonly Plugin plugin;
        private readonly HttpClient client;

        public PluginClient(Plugin plugin)
        {
            this.plugin = plugin;
            this.client = new HttpClient();
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<TResponse> SendAsync<TRequest, TResponse>(PluginItem item, TRequest request, CancellationToken token)
        {
#if DEBUG
            var uri = $"{this.plugin.DebugUrl ?? this.plugin.Url}/{item.Route}";
#else
            var uri = $"{this.plugin.Url}/{item.Route}";
#endif

            try
            {

                using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, uri))
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                    var response = await this.client.SendAsync(message, token);
                    response.EnsureSuccessStatusCode();

                    var responseString = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<TResponse>(responseString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to invoke api {uri}, Exception message: {ex.Message}");
                Console.WriteLine($"Exception stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
