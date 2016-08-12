namespace GameCloud.UCenter.Database.Entities
{
    /// <summary>
    /// Provide an Enum for order state.
    /// </summary>
    public enum OrderState
    {
        /// <summary>
        /// Indicating the order is created.
        /// </summary>
        Created,

        /// <summary>
        /// Indicating the order is success.
        /// </summary>
        Success,

        /// <summary>
        /// Indicating the order is failed.
        /// </summary>
        Failed,

        /// <summary>
        /// Indicating the order is expired.
        /// </summary>
        Expired
    }
}
