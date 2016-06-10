namespace GF.UCenter.Common.IP
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.ServiceModel.Channels;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Newtonsoft.Json;
    using Portable.Models.Ip;

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
                    var response = await httpClient.SendAsync(request, token);
                    string content = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<IPInfoResponse>(content);
                }
            }
            catch (Exception)
            {
                return new IPInfoResponse { Code = IPInfoResponseCode.Failed };
            }
        }

        public static string GetClientIpAddress(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }

            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                var prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }

            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }

            return null;
        }
    }
}
