namespace GF.UCenter.Web.Common.Modes
{
    using System.Collections.Generic;

    public class PaginationResponse<TRaw>
    {
        public long Total { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public IEnumerable<TRaw> Raws { get; set; }
    }
}
