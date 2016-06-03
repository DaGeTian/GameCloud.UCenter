using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GF.UCenter.MongoDB.Entity
{
    public enum OrderState
    {
        Created,
        Success,
        Failed,
        Expired
    }
}
