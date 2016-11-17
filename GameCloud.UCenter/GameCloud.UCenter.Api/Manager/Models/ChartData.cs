using System.Collections.Generic;

namespace GameCloud.UCenter.Api.Manager.Models
{
    public class ChartData
    {
        public ChartData()
        {
            this.Labels = new List<string>();
            this.Data = new List<List<float>>();
            this.Series = new List<string>();
        }

        public List<string> Labels { get; set; }

        public List<string> Series { get; set; }

        public List<List<float>> Data { get; set; }

        public List<ChartOverride> Overrides { get; set; }
    }
}
