using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GF.Database;
using GF.Database.Adapters;
using GF.Manager.Contract;
using GF.Manager.Contract.Responses;
using GF.Manager.UCenter.Models;
using GF.UCenter.Common;
using GF.UCenter.Database;
using MongoDB.Driver;

namespace GF.Manager.UCenter
{
    [PluginMetadata(Name = "ucenter", DisplayName = "UCenter管理平台", Description = "This is a demo plugin.")]
    [PluginCategoryMetadata(Name = "player-analytics", DisplayName = "玩家分析", Description = "This is demo collection 1")]
    [PluginCategoryMetadata(Name = "online-analytics", DisplayName = "在线分析", Description = "This is demo collection 2")]
    public class UCenterPluginEntryPoint : PluginEntryPoint
    {
        private readonly ExportProvider exportProvider;
        private readonly UCenterDatabaseContext database;

        public UCenterPluginEntryPoint()
        {
            this.exportProvider = CompositionContainerFactory.Create();
            var dbSettings = this.exportProvider.GetExportedValue<DatabaseContextSettings>();
            dbSettings.ConnectionString = this.Configuration.GetSettingValue<string>("ConnectionString");
            dbSettings.DatabaseName = this.Configuration.GetSettingValue<string>("DatabaseName");

            this.database = this.exportProvider.GetExportedValue<UCenterDatabaseContext>();
        }

        [PluginItemMetadata(Name = "app-search", DisplayName = "App管理", Type = PluginItemType.List)]
        public async Task<PluginPaginationResponse<AppEntity>> GetApps(PluginRequestInfo request)
        {
            Expression<Func<AppEntity, bool>> filter = null;
            string keyword = request.GetParameterValue<string>("keyword");
            int page = request.GetParameterValue<int>("page", 1);
            int count = request.GetParameterValue<int>("pageSize", 10);
            if (!string.IsNullOrEmpty(keyword))
            {
                filter = a => a.Name.Contains(keyword);
            }

            var total = await this.database.Apps.CountAsync(filter, CancellationToken.None);

            IQueryable<AppEntity> queryable = this.database.Apps.Collection.AsQueryable();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }

            var result = queryable.Skip((page - 1) * count).Take(count).ToList();

            // todo: add orderby support.
            var model = new PluginPaginationResponse<AppEntity>
            {
                Page = page,
                PageSize = count,
                Raws = result,
                Total = total
            };

            return model;
        }

        [PluginItemMetadata(Name = "account-search", DisplayName = "账号管理", Type = PluginItemType.List)]
        public async Task<PluginPaginationResponse<AccountEntity>> GetAccounts(PluginRequestInfo request)
        {
            var accountId = request.GetParameterValue<string>("accountId");
            var keyword = request.GetParameterValue<string>("keyword");
            var page = request.GetParameterValue<int>("page", 1);
            var count = request.GetParameterValue<int>("pageSize", 10);

            Expression<Func<AccountEntity, bool>> filter = null;

            if (!string.IsNullOrEmpty(keyword))
            {
                filter = a => a.AccountName.Contains(keyword)
                    || a.Email.Contains(keyword)
                    || a.PhoneNum.Contains(keyword);
            }

            var total = await this.database.Accounts.CountAsync(filter, CancellationToken.None);

            IQueryable<AccountEntity> queryable = this.database.Accounts.Collection.AsQueryable();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }

            var result = queryable.Skip((page - 1) * count).Take(count).ToList();

            // todo: add orderby support.
            var model = new PluginPaginationResponse<AccountEntity>
            {
                Page = page,
                PageSize = count,
                Raws = result,
                Total = total
            };

            return model;
        }

        [PluginItemMetadata(Name = "order-search", DisplayName = "订单管理", Type = PluginItemType.List)]
        public PluginPaginationResponse<OrderEntity> GetOrders(PluginRequestInfo request)
        {
            var accountId = request.GetParameterValue<string>("accountId");
            var keyword = request.GetParameterValue<string>("keyword");
            var page = request.GetParameterValue<int>("page", 1);
            var count = request.GetParameterValue<int>("pageSize", 10);

            IQueryable<OrderEntity> querable = this.database.Orders.Collection.AsQueryable();
            if (!string.IsNullOrEmpty(accountId))
            {
                querable = querable.Where(a => a.AccountId == accountId);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                querable = querable.Where(a => a.Id.Contains(keyword)
                    || a.AccountName.Contains(keyword)
                    || a.AppName.Contains(keyword)
                    || a.Content.Contains(keyword));
            }

            var orders = querable.Skip((page - 1) * count).Take(count).ToList();

            var total = querable.LongCount();

            var model = new PluginPaginationResponse<OrderEntity>
            {
                Page = page,
                PageSize = count,
                Total = total,
                Raws = orders
            };

            return model;
        }

        [PluginItemMetadata(Name = "realtime-glance", DisplayName = "实时状况", Type = PluginItemType.Report)]
        public Task<UCenterStatisticsInfo> GetRealtimeGlance(PluginRequestInfo request)
        {
            return Task.FromResult<UCenterStatisticsInfo>(this.CreateSampleStatisticsInfo());
        }

        [PluginItemMetadata(Name = "new-users", Category = "player-analytics", DisplayName = "新增玩家", Type = PluginItemType.Report)]
        public Task<UCenterStatisticsInfo> GetNewUsers(PluginRequestInfo request)
        {
            return Task.FromResult<UCenterStatisticsInfo>(this.CreateSampleStatisticsInfo());
        }

        [PluginItemMetadata(Name = "active-users", Category = "player-analytics", DisplayName = "活跃玩家", Type = PluginItemType.Report)]
        public Task<UCenterStatisticsInfo> GetActiveUsers(PluginRequestInfo request)
        {
            return Task.FromResult<UCenterStatisticsInfo>(this.CreateSampleStatisticsInfo());
        }

        [PluginItemMetadata(Name = "stay-statistics", Category = "player-analytics", DisplayName = "留存统计", Type = PluginItemType.Report)]
        public Task<UCenterStatisticsInfo> GetStayStatistics(PluginRequestInfo request)
        {
            return Task.FromResult<UCenterStatisticsInfo>(this.CreateSampleStatisticsInfo());
        }

        [PluginItemMetadata(Name = "lost-statistics", Category = "player-analytics", DisplayName = "流失统计", Type = PluginItemType.Report)]
        public Task<UCenterStatisticsInfo> GetLostStatistics(PluginRequestInfo request)
        {
            return Task.FromResult<UCenterStatisticsInfo>(this.CreateSampleStatisticsInfo());
        }

        [PluginItemMetadata(Name = "online-analytics", Category = "online-analytics", DisplayName = "在线分析", Type = PluginItemType.Report)]
        public Task<UCenterStatisticsInfo> GetOnlineAnalytics(PluginRequestInfo request)
        {
            return Task.FromResult<UCenterStatisticsInfo>(this.CreateSampleStatisticsInfo());
        }

        [PluginItemMetadata(Name = "online-behaviour", Category = "online-behaviour", DisplayName = "在线习惯", Type = PluginItemType.Report)]
        public Task<UCenterStatisticsInfo> GetOnlineBehaviour(PluginRequestInfo request)
        {
            return Task.FromResult<UCenterStatisticsInfo>(this.CreateSampleStatisticsInfo());
        }

        private UCenterStatisticsInfo CreateSampleStatisticsInfo()
        {
            UCenterStatisticsInfo info = new UCenterStatisticsInfo();
            info.FirstPlay = new StatisticsData()
            {
                Labels = new string[] { "6PM", "7PM", "8PM", "9PM", "10PM", "11PM", "12PM" },
                Datas = this.RandomNumbers(7),
            };

            info.NewUserSex = new StatisticsData()
            {
                Labels = new string[] { "男", "女" },
                Datas = this.RandomNumbers(2),
            };

            info.ActiveUserSex = new StatisticsData()
            {
                Labels = new string[] { "男", "女" },
                Datas = this.RandomNumbers(2),
            };

            info.NewUserAge = new StatisticsData()
            {
                Labels = new string[] { "20-30", "30-40", "40-50", "50-60" },
                Datas = new double[] { 55, 5, 15, 15, 5 }
            };

            info.ActiveUserAge = new StatisticsData()
            {
                Labels = new string[] { "20-30", "30-40", "40-50", "50-60" },
                Datas = new double[] { 55, 5, 15, 15, 5 }
            };

            info.HourActiveDevices = new StatisticsData()
            {
                Labels = ParallelEnumerable.Range(0, 24).Select(i => i.ToString()).ToArray(),
                Series = new string[] { "小时设备激活" },
                Datas = this.RandomNumbers(24),
            };

            info.HourNewUsers = new StatisticsData()
            {
                Labels = ParallelEnumerable.Range(0, 24).Select(i => i.ToString()).ToArray(),
                Series = new string[] { "小时新增用户" },
                Datas = this.RandomNumbers(24),
            };

            info.HourNewDevices = new StatisticsData()
            {
                Labels = ParallelEnumerable.Range(0, 24).Select(i => i.ToString()).ToArray(),
                Series = new string[] { "小时新增设备" },
                Datas = this.RandomNumbers(24),
            };

            info.HourDAU = new StatisticsData()
            {
                Labels = ParallelEnumerable.Range(0, 24).Select(i => i.ToString()).ToArray(),
                Series = new string[] { "小时DAU" },
                Datas = this.RandomNumbers(24),
            };

            info.HourWAU = new StatisticsData()
            {
                Labels = ParallelEnumerable.Range(0, 24).Select(i => i.ToString()).ToArray(),
                Series = new string[] { "小时WAU" },
                Datas = this.RandomNumbers(24),
            };

            info.HourMAU = new StatisticsData()
            {
                Labels = ParallelEnumerable.Range(0, 24).Select(i => i.ToString()).ToArray(),
                Series = new string[] { "小时MAU" },
                Datas = this.RandomNumbers(24),
            };

            return info;
        }

        private double[] RandomNumbers(int length)
        {
            Random random = new Random();
            return new int[length].Select(i => Math.Floor(random.NextDouble() * 10000) / 100).ToArray();
        }
    }
}
