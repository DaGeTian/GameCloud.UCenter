using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameCloud.UCenter.Common.Portable.Models.AppClient;
using Microsoft.AspNetCore.Mvc;

namespace GameCloud.UCenter
{
    public class MockApiController : Controller
    {
        //---------------------------------------------------------------------
        // Mock https://api.weixin.qq.com/sns/oauth2/access_token?appid=APPID&secret=SECRET&code=CODE&grant_type=authorization_code
        // Doc: https://open.weixin.qq.com/cgi-bin/showdocument?action=dir_list&t=resource/res_list&verify=1&id=open1419317853&token=&lang=zh_CN
        [HttpGet]
        [Route("api/mock/sns/oauth2/access_token")]
        public OAuthTokenResponse GetAuthTokenResponse(string appid, string secret, string code, string grant_type)
        {
            string scope = code == "codeforsnsapi_userinfo" ? "snsapi_userinfo" : "snsapi_base";
            var result = new OAuthTokenResponse
            {
                OpenId = "mock_openid",
                AccessToken = "mock_access_token",
                ExpiresIn = 7200,
                RefreshToken = "mock_refresh_token",
                Scope = scope,
                UnionId = "mock_unionid"
            };

            return result;
        }
    }
}
