namespace GameCloud.UCenter.Common.Portable.Models.AppClient
{
    /// <summary>
    /// provide a Enum for the key placeholder type.
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// Indicating account is normal.
        /// </summary>
        NormalAccount = 0,

        /// <summary>
        /// Indicating account is guest
        /// </summary>
        Guest,
    }

    // 登陆源
    public enum LoginFrom
    {
        Default = 0,
        Phone = 10,
        Wechat = 20,
        QQ = 30
    }
}
