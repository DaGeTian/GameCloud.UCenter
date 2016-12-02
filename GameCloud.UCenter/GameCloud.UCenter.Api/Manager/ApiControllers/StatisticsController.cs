using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.Database.Adapters;
using GameCloud.Manager.PluginContract.Requests;
using GameCloud.Manager.PluginContract.Responses;
using GameCloud.UCenter.Api.Manager.Models;
using GameCloud.UCenter.Common.Settings;
using GameCloud.UCenter.Database;
using GameCloud.UCenter.Database.Entities;
using GameCloud.UCenter.Manager.Api.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace GameCloud.UCenter.Api.Manager.ApiControllers
{
    /// <summary>
    /// Provide a controller for users.
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StatisticsController : ManagerApiControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountEventsController" /> class.
        /// </summary>
        /// <param name="ucenterDb">Indicating the database context.</param>
        /// <param name="ucenterventDb">Indicating the database context.</param>
        /// <param name="settings">Indicating the settings.</param>
        [ImportingConstructor]
        public StatisticsController(
            UCenterDatabaseContext ucenterDb,
            UCenterEventDatabaseContext ucenterventDb,
            Settings settings)
            : base(ucenterDb, ucenterventDb, settings)
        {
        }

        [HttpPost, Route("api/manager/newusers")]
        public async Task<NewUserStatisticsData> NewUsers([FromBody]PluginRequestInfo request, CancellationToken token)
        {
            var startTime = request.GetParameterValue<DateTime>("startDate", DateTime.UtcNow.AddYears(-1)).ToUniversalTime();
            var endTime = request.GetParameterValue<DateTime>("endDate", DateTime.UtcNow).ToUniversalTime();

            var result = new NewUserStatisticsData();
            result.HourlyNewUsers = await this.GetHourlyNewUserChartData(startTime, endTime, token);

            return result;
        }

        [HttpPost, Route("api/manager/userstatistics")]
        public async Task<UserStatisticsData> UserStatistics([FromBody] PluginRequestInfo request, CancellationToken token)
        {
            var startDate = request.GetParameterValue<DateTime>("startDate", DateTime.UtcNow.AddYears(-1)).ToUniversalTime();
            var endDate = request.GetParameterValue<DateTime>("endDate", DateTime.UtcNow).ToUniversalTime();
            var startTime = request.GetParameterValue<DateTime>("startTime", DateTime.UtcNow.AddYears(-1)).ToUniversalTime();
            var endTime = request.GetParameterValue<DateTime>("endTime", DateTime.UtcNow).ToUniversalTime();
            string type = request.GetParameterValue<string>("type", "day");

            var startDateTime = startDate.Date + startTime.TimeOfDay;
            var endDateTime = endDate.Date + endTime.TimeOfDay;
            var loginRecords = await this.UCenterEventDatabase.AccountEvents.GetListAsync(
                e => e.EventName == "Login" && e.CreatedTime >= startDateTime && e.CreatedTime <= endDateTime,
                token);

            IEnumerable<IGrouping<DateTime, AccountEventEntity>> groups = null;
            if (type == "day")
            {
                groups = loginRecords.GroupBy(e => e.CreatedTime.Date);
            }
            else if (type == "week")
            {
                groups = loginRecords.GroupBy(e => e.CreatedTime.Date.AddDays(-1 * (int)(e.CreatedTime.Date.DayOfWeek)));
            }
            else if (type == "month")
            {
                groups = loginRecords.GroupBy(e => e.CreatedTime.Date.AddDays(-1 * e.CreatedTime.Date.Day + 1));
            }

            groups = groups.OrderBy(g => g.Key);

            List<string> previousUsers = null;
            float ratio = 1;
            var remainRate = new ChartData();
            var remainDatas = new List<float>();

            foreach (var group in groups)
            {
                if (previousUsers == null || previousUsers.Count == 0)
                {
                    ratio = 1;
                    previousUsers = group.Select(g => g.AccountId).Distinct().ToList();
                }
                else
                {
                    var currentUsers = group.Select(g => g.AccountId).Distinct().ToList();
                    var remainUserCount = currentUsers.Count - currentUsers.Except(previousUsers).Count();
                    ratio = 100 * (float)Math.Round((double)remainUserCount / previousUsers.Count, 2);
                    previousUsers = currentUsers;
                }

                remainDatas.Add(ratio);
                remainRate.Labels.Add(group.Key.ToString("yyyy/MM/dd"));
            }

            remainRate.Data.Add(remainDatas);
            remainRate.Series.Add("留存率");

            var lostRate = new ChartData()
            {
                Data = new List<List<float>>() { remainDatas.Select(d => 100 - d).ToList() },
                Labels = remainRate.Labels,
                Series = new List<string>() { "流失率" }
            };

            var lifeCycleRate = new ChartData()
            {
                Data = lostRate.Data.Select(raw => raw.Select(d => d == 0 ? 0 : 1 / d).ToList()).ToList(),
                Labels = remainRate.Labels,
                Series = new List<string>() { "生命周期" }
            };

            return new UserStatisticsData()
            {
                RemainRate = await this.GetStayLostStatisticsData(startTime, endTime, type, true, token),
                LostRate = lostRate,
                LifeCycle = lifeCycleRate
            };
        }

        [HttpPost, Route("api/manager/userstatistics2")]
        public async Task<ChartData> UserStatistics2([FromBody] PluginRequestInfo request, CancellationToken token)
        {
            var startTime = request.GetParameterValue<DateTime>("startDate", DateTime.UtcNow.AddYears(-1)).ToUniversalTime();
            var endTime = request.GetParameterValue<DateTime>("endDate", DateTime.UtcNow).ToUniversalTime();
            string type = request.GetParameterValue<string>("type", "day");
            bool isStay = request.GetParameterValue<bool>("isStay");

            return await this.GetStayLostStatisticsData(startTime, endTime, type, isStay, token);
        }

        [HttpPost, Route("api/manager/hourlystatistics")]
        public async Task<ChartData> NewUserStatistics([FromBody] PluginRequestInfo request, CancellationToken token)
        {
            var today = DateTime.UtcNow.Date;
            var yesterDay = today.AddDays(-1);
            var last7Day = today.AddDays(-7);
            var latest30Days = today.AddDays(-30);
            var dates = request.GetParameterValue<DateTime[]>("dates", new DateTime[] { });
            dates = dates.Select(d => d.ToUniversalTime()).ToArray();
            var labels = ParallelEnumerable.Range(0, 24).Select(i => i > 9 ? $"{i}:00" : $"0{i}:00").ToList();
            var series = new List<string>();
            var chartData = new ChartData();
            chartData.Labels = labels;

            chartData.Data.Add(await this.GetHourlyUserStatisticsData(today, today.AddDays(1), token));
            chartData.Series.Add("today");

            chartData.Data.Add(await this.GetHourlyUserStatisticsData(yesterDay, today, token));
            chartData.Series.Add("yesterday");

            chartData.Data.Add(await this.GetHourlyUserStatisticsData(last7Day, today, token));
            chartData.Series.Add("last7 days");

            chartData.Data.Add(await this.GetHourlyUserStatisticsData(latest30Days, today, token));
            chartData.Series.Add("latest30 days");

            foreach (var date in dates)
            {
                chartData.Data.Add(await this.GetHourlyUserStatisticsData(date, date.AddDays(1), token));
                chartData.Series.Add(date.ToString("YYYY/MM/dd"));
            }

            return chartData;
        }

        #region <<<<<<<<<<<<<<<<<<<<<<<<< The final version.>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        [HttpPost, Route("api/manager/newusersanddevices")]
        public async Task<ChartData> NewUserAndDeviceStatistics([FromBody] PluginRequestInfo request, CancellationToken token)
        {
            var startTime = request.GetParameterValue<DateTime>("startDate", DateTime.UtcNow.Date.AddDays(-7)).ToUniversalTime();
            var endTime = request.GetParameterValue<DateTime>("endDate", DateTime.UtcNow.Date).ToUniversalTime();

            // should always query to the day next to endTime.
            var queryEndTime = endTime.AddDays(1);
            var users = await this.UCenterDatabase.Accounts.GetListAsync(u => u.CreatedTime >= startTime && u.CreatedTime < queryEndTime, token);
            var devices = await this.UCenterDatabase.Devices.GetListAsync(d => d.CreatedTime >= startTime && d.CreatedTime < queryEndTime, token);
            var userGroups = users.GroupBy(u => u.CreatedTime.ToLocalTime().Date).ToList();
            var deviceGroups = devices.GroupBy(d => d.CreatedTime.ToLocalTime().Date).ToList();

            startTime = startTime.ToLocalTime();
            endTime = endTime.ToLocalTime();
            var result = new ChartData();
            var userData = new List<float>();
            var deviceData = new List<float>();
            for (var date = startTime.Date; date <= endTime.Date; date = date.AddDays(1))
            {
                var userGroup = userGroups.FirstOrDefault(g => g.Key == date);
                var deviceGroup = deviceGroups.FirstOrDefault(d => d.Key == date);
                result.Labels.Add(date.ToString("yyyy/MM/dd"));
                userData.Add(userGroup == null ? 0 : userGroup.Count());
                deviceData.Add(deviceGroup == null ? 0 : deviceGroup.Count());
            }

            result.Data.Add(userData);
            result.Data.Add(deviceData);

            result.Series.Add("新增用户");
            result.Series.Add("新增设备");

            return result;
        }

        [HttpPost, Route("api/manager/staystatistics")]
        public async Task<ChartData> StayStatistics([FromBody] PluginRequestInfo request, CancellationToken token)
        {
            var startTime = request.GetParameterValue<DateTime>("startDate", DateTime.UtcNow.Date.AddDays(-7)).ToUniversalTime();
            var endTime = request.GetParameterValue<DateTime>("endDate", DateTime.UtcNow.Date).ToUniversalTime();

            return await this.GetStayLostStatisticsData2(startTime, endTime, true, token);
        }

        [HttpPost, Route("api/manager/loststatistics")]
        public async Task<ChartData> LostStatistics([FromBody] PluginRequestInfo request, CancellationToken token)
        {
            var startTime = request.GetParameterValue<DateTime>("startDate", DateTime.UtcNow.Date.AddDays(-7)).ToUniversalTime();
            var endTime = request.GetParameterValue<DateTime>("endDate", DateTime.UtcNow.Date).ToUniversalTime();

            return await this.GetStayLostStatisticsData2(startTime, endTime, false, token);
        }

        [HttpPost, Route("api/manager/activeusers")]
        public async Task<ChartData> ActiveUserStatistics([FromBody] PluginRequestInfo request, CancellationToken token)
        {
            var startTime = request.GetParameterValue<DateTime>("startDate", DateTime.UtcNow.Date.AddDays(-7)).ToUniversalTime();
            var endTime = request.GetParameterValue<DateTime>("endDate", DateTime.UtcNow.Date).ToUniversalTime();

            // for the end time, we should always add 1 day.
            var queryEndTime = endTime.AddDays(1);

            var loginRecords = await this.UCenterEventDatabase.AccountEvents.GetListAsync(
                e => (e.EventName == "Login" || e.EventName == "GuestLogin") && (e.CreatedTime >= startTime && e.CreatedTime < queryEndTime),
                token);

            var groups = loginRecords.GroupBy(e => e.CreatedTime.ToLocalTime().Date).ToList();

            var result = new ChartData();
            var datas = new List<float>();
            startTime = startTime.ToLocalTime();
            endTime = endTime.ToLocalTime();
            for (var date = startTime.Date; date <= endTime.Date; date = date.AddDays(1))
            {
                var group = groups.FirstOrDefault(g => g.Key == date);
                result.Labels.Add(date.ToString("yyyy/MM/dd"));
                datas.Add(group == null ? 0 : group.Select(g => g.AccountId).Distinct().Count());
            }

            result.Data.Add(datas);
            result.Series.Add("活跃用户");

            return result;
        }

        #endregion

        private async Task<List<float>> GetHourlyUserStatisticsData(DateTime startTime, DateTime endTime, CancellationToken token)
        {
            var users = await this.UCenterDatabase.Accounts.GetListAsync(u => u.CreatedTime >= startTime && u.CreatedTime <= endTime, token);
            var groups = users.GroupBy(u =>
            {
                var localTime = u.CreatedTime.ToLocalTime();
                return localTime.Hour;
            })
            .OrderBy(g => g.Key)
            .ToList();
            var result = new List<float>();
            var start = 0;
            var end = 24;
            for (var idx = start; idx < end; idx++)
            {
                if (groups.Any(g => g.Key == idx))
                {
                    result.Add(groups.First(g => g.Key == idx).Count());
                }
                else
                {
                    result.Add(0);
                }
            }

            return result;
        }

        private async Task<ChartData> GetHourlyNewUserChartData(DateTime startTime, DateTime endTime, CancellationToken token)
        {
            var users = await this.UCenterDatabase.Accounts.GetListAsync(
                u => u.CreatedTime >= startTime && u.CreatedTime < endTime,
                token);

            var groups = users
                .GroupBy(u =>
                {
                    var localTime = u.CreatedTime.ToLocalTime();
                    return localTime.Date.AddHours(localTime.Hour);
                })
                .OrderBy(g => g.Key)
                .ToList();

            var data = groups.Select(g => (float)(g.Distinct().Count())).ToList();
            var labels = groups.Select(g => g.Key.ToString("yyyy-MM-dd HH:00")).ToList();
            var series = new List<string>() { "小时新增用户" };

            return new ChartData()
            {
                Data = new List<List<float>>() { data },
                Labels = labels,
                Series = series
            };
        }

        private async Task<ChartData> GetStayLostStatisticsData(DateTime startTime, DateTime endTime, string type, bool isStay, CancellationToken token)
        {
            var loginRecords = await this.UCenterEventDatabase.AccountEvents.GetListAsync(
                e => e.EventName == "Login" && e.CreatedTime >= startTime && e.CreatedTime <= endTime,
                token);

            IEnumerable<IGrouping<DateTime, AccountEventEntity>> groups = null;
            if (type == "day")
            {
                groups = loginRecords.GroupBy(e => e.CreatedTime.Date);
            }
            else if (type == "week")
            {
                groups = loginRecords.GroupBy(e => e.CreatedTime.Date.AddDays(-1 * (int)(e.CreatedTime.Date.DayOfWeek)));
            }
            else if (type == "month")
            {
                groups = loginRecords.GroupBy(e => e.CreatedTime.Date.AddDays(-1 * e.CreatedTime.Date.Day + 1));
            }

            groups = groups.OrderBy(g => g.Key);

            List<string> previousUsers = null;
            float percent = 0;
            float count = 0;
            var chart = new ChartData();
            var percentDatas = new List<float>();
            var countDatas = new List<float>();

            foreach (var group in groups)
            {
                if (previousUsers == null || previousUsers.Count == 0)
                {
                    percent = 0;
                    count = 0;
                    previousUsers = group.Select(g => g.AccountId).Distinct().ToList();
                }
                else
                {
                    var currentUsers = group.Select(g => g.AccountId).Distinct().ToList();
                    count = currentUsers.Count - currentUsers.Except(previousUsers).Count();
                    percent = (float)Math.Round((double)count / previousUsers.Count, 4) * 100;

                    if (!isStay)
                    {
                        count = previousUsers.Count - count;
                        percent = (float)Math.Round((double)(100 - percent), 2);
                    }

                    previousUsers = currentUsers;
                }

                percentDatas.Add(percent);
                countDatas.Add(count);
                chart.Labels.Add(group.Key.ToString("yyyy/MM/dd"));
            }

            chart.Data.Add(percentDatas);
            chart.Data.Add(countDatas);
            return chart;
        }

        private async Task<ChartData> GetStayLostStatisticsData2(DateTime startTime, DateTime endTime, bool isStay, CancellationToken token)
        {
            var lastDay = endTime.AddDays(30);
            var loginRecords = await this.UCenterEventDatabase.AccountEvents.GetListAsync(
                e => e.EventName == "Login" && e.CreatedTime >= startTime && e.CreatedTime <= lastDay,
                token);

            var groups = loginRecords.GroupBy(e => e.CreatedTime.ToLocalTime().Date).ToList();
            var dayDatas = new List<float>();
            var last7Datas = new List<float>();
            var last30Datas = new List<float>();
            var labels = new List<string>();

            var startDate = startTime.ToLocalTime().Date;
            var endDate = endTime.ToLocalTime().Date;

            Func<DateTime, int, float> getStay = (dateTime, days) =>
             {
                 var next = groups.FirstOrDefault(g => g.Key == dateTime.AddDays(days));
                 var group = groups.FirstOrDefault(g => g.Key == dateTime);
                 if (group == null)
                 {
                     return 0;
                 }
                 else if (next == null)
                 {
                     return 0;
                 }
                 else
                 {
                     var nextUsers = next.Select(u => u.AccountId).Distinct().ToList();
                     var currentUsers = group.Select(u => u.AccountId).Distinct().ToList();

                     return (float)Math.Round((double)(nextUsers.Count() - nextUsers.Except(currentUsers).Count()) / currentUsers.Count() * 100, 2);
                 }
             };

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                dayDatas.Add(getStay(date, 1));
                last7Datas.Add(getStay(date, 7));
                last30Datas.Add(getStay(date, 30));
                labels.Add(date.ToString("yyyy/MM/dd"));
            }

            var result = new ChartData();
            result.Data.Add(dayDatas);
            result.Data.Add(last7Datas);
            result.Data.Add(last30Datas);
            result.Labels = labels;

            var tag = "留存";
            if (!isStay)
            {
                result.Data = result.Data.Select(list => list.Select(d => 100 - d).ToList()).ToList();
                tag = "流失";
            }

            result.Series = new List<string>() { $"次日{tag}率", $"7日{tag}率", $"30日{tag}率" };

            return result;
        }
    }
}
