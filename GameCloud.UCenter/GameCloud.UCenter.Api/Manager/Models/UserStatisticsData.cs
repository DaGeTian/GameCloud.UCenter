using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameCloud.UCenter.Api.Manager.Models;

namespace GameCloud.UCenter.Manager.Api.Models
{
    public class UserStatisticsData
    {
        public ChartData RemainRate { get; set; }

        public ChartData LostRate { get; set; }

        public ChartData LifeCycle { get; set; }
    }
}
