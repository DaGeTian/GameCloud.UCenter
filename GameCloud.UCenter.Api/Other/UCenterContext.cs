using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GameCloud.Common.MEF;
using GameCloud.Common.Settings;
using GameCloud.UCenter.Common.Portable.Contracts;
using GameCloud.UCenter.Common.Portable.Exceptions;
using GameCloud.UCenter.Database;
using GameCloud.UCenter.Database.Entities;
using GameCloud.UCenter.Web.Common.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Driver;

namespace GameCloud.UCenter
{
    public class UCenterContext
    {
        //---------------------------------------------------------------------
        public static UCenterContext Instance { get; private set; }
        public CacheAppEntity CacheAppEntity { get; private set; }
        public ILogger Logger { get; private set; }

        //---------------------------------------------------------------------
        public UCenterContext(ILoggerFactory logger_factory)
        {
            Instance = this;
            Logger = logger_factory.CreateLogger("Default");

            Logger.LogInformation("UCenterContext()");
        }

        //---------------------------------------------------------------------
        public async Task Setup(UCenterDatabaseContext database, CancellationToken token)
        {
            CacheAppEntity = new CacheAppEntity(database);
            await CacheAppEntity.Setup(token);
        }
    }
}
