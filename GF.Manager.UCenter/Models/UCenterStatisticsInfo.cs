using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GF.Manager.UCenter.Models
{
    [DataContract]
    public class UCenterStatisticsInfo
    {
        [DataMember(Name = "playTime")]
        public StatisticsData PlayTime { get; set; }

        [DataMember(Name = "sex")]
        public StatisticsData Sex { get; set; }

        [DataMember(Name = "age")]
        public StatisticsData Age { get; set; }

        [DataMember(Name = "hourNewDevices")]
        public StatisticsData HourNewDevices { get; set; }

        [DataMember(Name = "hourActiveDevices")]
        public StatisticsData HourActiveDevices { get; set; }

        [DataMember(Name = "hourNewUsers")]
        public StatisticsData HourNewUsers { get; set; }

        [DataMember(Name = "firstPlay")]
        public StatisticsData FirstPlay { get; set; }

        [DataMember(Name = "newUserSex")]
        public StatisticsData NewUserSex { get; set; }

        [DataMember(Name = "activeUserSex")]
        public StatisticsData ActiveUserSex { get; set; }

        [DataMember(Name = "newUserAge")]
        public StatisticsData NewUserAge { get; set; }

        [DataMember(Name = "activeUserAge")]
        public StatisticsData ActiveUserAge { get; set; }

        [DataMember(Name = "hourDAU")]
        public StatisticsData HourDAU { get; set; }

        [DataMember(Name = "hourWAU")]
        public StatisticsData HourWAU { get; set; }

        [DataMember(Name = "hourMAU")]
        public StatisticsData HourMAU { get; set; }
    }
}
