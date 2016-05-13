using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GF.UCenter.Common.IP
{
    public static class IPHelper
    {
        private const string HostUrl = "http://ip.taobao.com/service/getIpInfo.php";
        public static async Task<IPInfoResponse> GetIPInfoAsync(string ipAddress, CancellationToken token)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(1);
                    string url = string.Format(CultureInfo.InvariantCulture, "{0}?ip={1}", HostUrl, ipAddress);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                    // request.Properties.conte
                    var response = await httpClient.SendAsync(request, token);
                    string content = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<IPInfoResponse>(content);
                }
            }
            catch (Exception)
            {
                return new IPInfoResponse() { Code = IPInfoResponseCode.Failed };
            }
        }
    }
}
