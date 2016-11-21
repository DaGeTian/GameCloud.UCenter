using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace GameCloud.UCenter.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetClientIpAddress(this HttpContext request)
        {
            try
            {
                var remoteIpAddress = request.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress;

                if (remoteIpAddress == null)
                {
                    return "Unknown";
                }
                return remoteIpAddress.ToString();
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
