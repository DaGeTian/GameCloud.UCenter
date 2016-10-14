using System.Runtime.Serialization;

namespace GameCloud.UCenter.Common.Portable.Models.AppClient
{
    public class WeChatAccessTokenResponse
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }

        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        [DataMember(Name = "openid")]
        public string OpenId { get; set; }

        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        [DataMember(Name = "unionid")]
        public string UnionId { get; set; }
    }
}
