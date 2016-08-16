using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.UCenter.Database;
using GameCloud.UCenter.Database.Entities;
using GameCloud.UCenter.Web.Common.Logger;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameCloud.UCenter.Test.MongoDB
{
    [TestClass]
    public class EventTraceTest : UCenterTestBase
    {
        [TestMethod]
        public async Task BulkTraceEventTest()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var adapter = ExportProvider.GetExportedValue<UCenterDatabaseContext>()
                .AccountEvents;
            var eventTrace = ExportProvider.GetExportedValue<EventTrace>();
            int bufferSize = EventTrace.BufferSize;
            var entities = ParallelEnumerable.Range(0, bufferSize - 1)
                .Select(idx => this.GenerateTestRecord(idx))
                .ToList();
            var tasks = entities.AsParallel()
                .Select(entity =>
                {
                    return eventTrace.TraceEvent<AccountEventEntity>(entity, token);
                }).ToArray();

            await Task.WhenAll(tasks);
            var record = await adapter.GetSingleAsync(e => e.Id == entities.First().Id, token);
            Assert.IsNull(record);

            await eventTrace.TraceEvent<AccountEventEntity>(this.GenerateTestRecord(bufferSize), token);

            record = await adapter.GetSingleAsync(e => e.Id == entities.Last().Id, token);
            Assert.IsNotNull(record);
            Assert.IsNotNull(record.AccountName);
        }

        private AccountEventEntity GenerateTestRecord(int index)
        {
            var entity = new AccountEventEntity()
            {
                Id = DateTime.UtcNow.ToString("MMddyyyyHHmmss") + "-" + index.ToString(),
                AccountId = "testaccount",
                AccountName = "testaccountname",
                ClientIp = "127.0.0.1",
                DeviceId = "device-" + index.ToString(),
                EventName = "test event",
                Message = "test message",
                CreatedTime = DateTime.UtcNow,
                LoginArea = "ShangHai",
                UpdatedTime = DateTime.UtcNow,
                UserAgent = "user agent"
            };

            return entity;
        }
    }
}
