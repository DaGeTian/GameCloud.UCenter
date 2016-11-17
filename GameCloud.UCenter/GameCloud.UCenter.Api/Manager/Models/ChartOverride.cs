using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace GameCloud.UCenter.Api.Manager.Models
{
    [DataContract]
    public class ChartOverride
    {
        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember(Name = "yAxisID")]
        public string YAxisID { get; set; }
    }
}
