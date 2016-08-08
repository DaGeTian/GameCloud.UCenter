using System.Collections.Generic;

namespace GameCloud.UCenter.Web.Common.Modes
{
    /// <summary>
    /// Provide a class for pagination response.
    /// </summary>
    /// <typeparam name="TRaw">The type of object to paginate</typeparam>
    public class PaginationResponse<TRaw>
    {
        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the raw data.
        /// </summary>
        public IEnumerable<TRaw> Raws { get; set; }
    }
}
