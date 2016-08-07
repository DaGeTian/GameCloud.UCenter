using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GF.Manager.Contract;
using GF.Manager.Contract.Responses;

namespace GF.Manager.PluginDemo
{
    [PluginMetadata(Name = "demoplugin", DisplayName = "Demo插件", Description = "示例插件，包括列表，报表，更新数据")]
    [PluginCategoryMetadata(Name = "list", DisplayName = "分类-LIST", Description = "分类示例1")]
    [PluginCategoryMetadata(Name = "report", DisplayName = "分类-REPORT", Description = "分类示例2")]
    [PluginCategoryMetadata(Name = "update", DisplayName = "分类-UPDATE", Description = "分类示例3")]
    public class DemoPluginEntryPoint : PluginEntryPoint
    {
        private static DemoPluginSettings GlobalSettings;

        [PluginItemMetadata(Name = "demo-list", Category = "list", DisplayName = "列表示例", Description = "Demo for list data.", Type = PluginItemType.List)]
        public PluginPaginationResponse<DemoPluginRawData> GetDataForDemoList(PluginRequestInfo request)
        {
            var list = ParallelEnumerable.Range(0, 1000)
                .Select(i => new DemoPluginRawData() { Id = i, Name = "demo data " + i.ToString(), Type = (DemoEnumType)(i % 3) })
                .ToList();

            var keyword = request.GetParameterValue<string>("keyword");
            var page = request.GetParameterValue<int>("page", 1);
            var pageSize = request.GetParameterValue<int>("pageSize", 10);

            IEnumerable<DemoPluginRawData> raws = list;
            if (!string.IsNullOrEmpty(keyword))
            {
                raws = raws.Where(r => r.Name.Contains(keyword));
            }

            var total = raws.Count();

            raws = raws.Skip((page - 1) * pageSize).Take(pageSize);

            return new PluginPaginationResponse<DemoPluginRawData>()
            {
                Page = page,
                PageSize = pageSize,
                Raws = raws.ToList(),
                Total = total
            };
        }

        [PluginItemMetadata(Name = "demo-update", Category = "update", DisplayName = "更新数据示例", Description = "Demo for update data", Type = PluginItemType.Update)]
        public DemoPluginSettings GetDataForDemoUpdate(PluginRequestInfo request)
        {
            if (GlobalSettings == null)
            {
                GlobalSettings = new DemoPluginSettings();
                GlobalSettings.ConnectionString = this.Configuration.GetSettingValue<string>("ConnectionString");
                GlobalSettings.DatabaseName = this.Configuration.GetSettingValue<string>("DatabaseName");
                GlobalSettings.UpdateTime = DateTime.UtcNow;
            }

            if (request.Method == PluginRequestMethod.Update)
            {
                var data = request.GetParameterValue<DemoPluginSettings>("data");
                GlobalSettings.ConnectionString = data.ConnectionString;
                GlobalSettings.DatabaseName = data.DatabaseName;
                GlobalSettings.UpdateTime = DateTime.UtcNow;
            }

            return GlobalSettings;
        }

        [PluginItemMetadata(Name = "demo-report", Category = "report", DisplayName = "报表数据示例", Description = "Demo for report data", Type = PluginItemType.Report)]
        public IReadOnlyList<object> GetDataForDemoReport(PluginRequestInfo request)
        {
            var pieData = new
            {
                Labels = ParallelEnumerable.Range(0, 24).Select(i => i.ToString()).ToArray(),
                Series = new string[] { "小时设备激活" },
                Datas = this.RandomNumbers(24),
            };
            var lineData = new
            {
                Labels = new string[] { "男", "女" },
                Datas = this.RandomNumbers(2),
            };

            return new object[] { pieData, lineData };
        }

        private double[] RandomNumbers(int length)
        {
            Random random = new Random();
            return new int[length].Select(i => Math.Floor(random.NextDouble() * 10000) / 100).ToArray();
        }

    }
}
