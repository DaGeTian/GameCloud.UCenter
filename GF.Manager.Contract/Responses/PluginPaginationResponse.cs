using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GF.Manager.Contract.Responses
{
    /// <summary>
    /// Provide a class for pagination response.
    /// </summary>
    /// <typeparam name="TRaw">The type of object to paginate</typeparam>
    [DataContract]
    public class PluginPaginationResponse<TRaw>
    {
        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        [DataMember(Name = "total")]
        public long Total { get; set; }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>

        [DataMember(Name = "page")]
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>

        [DataMember(Name = "pageSize")]
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the raw data.
        /// </summary>

        [DataMember(Name = "raws")]
        public IEnumerable<TRaw> Raws { get; set; }
    }
}
