using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GF.Manager.UCenter.Models
{
    [DataContract]
    public class StatisticsData
    {
        [DataMember(Name = "series")]
        public IReadOnlyList<string> Series { get; set; }

        [DataMember(Name = "labels")]
        public IReadOnlyList<string> Labels { get; set; }

        [DataMember(Name = "datas")]
        public IReadOnlyList<double> Datas { get; set; }
    }
}
