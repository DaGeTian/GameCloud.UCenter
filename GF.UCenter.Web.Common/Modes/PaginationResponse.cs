using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GF.UCenter.Web.Common.Modes
{
    public class PaginationResponse<TRaw>
    {
        public long Total { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public IEnumerable<TRaw> Raws { get; set; }
    }
}
