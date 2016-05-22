using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GF.UCenter.CouchBase.Models
{
    public class PaginationResponse<TRaw>
    {
        public int Total { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
        
        public IEnumerable<TRaw> Raws { get; set; }
    }
}
