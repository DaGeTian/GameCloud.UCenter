using System;
using System.Collections.Generic;
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
    public class CacheAppEntity
    {
        //---------------------------------------------------------------------
        UCenterDatabaseContext Db { get; set; }
        Dictionary<string, AppEntity> MapAppId { get; set; }// key=AppId
        Dictionary<string, AppEntity> MapWechatAppId { get; set; }// key=WechatAppId

        //---------------------------------------------------------------------
        public CacheAppEntity(UCenterDatabaseContext db)
        {
            Db = db;
            MapAppId = new Dictionary<string, AppEntity>();
            MapWechatAppId = new Dictionary<string, AppEntity>();
        }

        //---------------------------------------------------------------------
        public async Task Setup(CancellationToken token)
        {
            var logger = UCenterContext.Instance.Logger;

            var apps = await Db.Apps.GetListAsync(e => !string.IsNullOrEmpty(e.Id), null, token);
            foreach (var i in apps)
            {
                MapAppId[i.Id] = i;
                MapWechatAppId[i.WechatAppId] = i;

                logger.LogInformation("AppId={0}, WechatAppId={1}", i.Id, i.WechatAppId);
            }
        }

        //---------------------------------------------------------------------
        public AppEntity GetAppEntityByAppId(string app_id)
        {
            MapAppId.TryGetValue(app_id, out var app_entity);
            return app_entity;
        }

        //---------------------------------------------------------------------
        public AppEntity GetAppEntityByWechatAppId(string wechat_app_id)
        {
            MapWechatAppId.TryGetValue(wechat_app_id, out var app_entity);
            return app_entity;
        }
    }
}
