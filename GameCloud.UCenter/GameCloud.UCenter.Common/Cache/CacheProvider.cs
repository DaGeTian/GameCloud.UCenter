using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameCloud.UCenter.Common.Cache
{
    public class CacheProvider<TEntity>
    {
        private readonly TimeSpan interval;
        private readonly Func<string, CancellationToken, Task<TEntity>> reteriver;
        private readonly ConcurrentDictionary<string, CacheInfo> map = new ConcurrentDictionary<string, CacheInfo>();

        public CacheProvider(TimeSpan interval, Func<string, CancellationToken, Task<TEntity>> reteriver)
        {
            this.interval = interval;
            this.reteriver = reteriver;
        }

        public async Task<TEntity> Get(string key, CancellationToken token)
        {
            CacheInfo info = null;
            if (this.map.TryGetValue(key, out info) &&
                info.LastUpdateTime.Add(this.interval) > DateTime.Now)
            {
                return info.Entity;
            }

            if (info == null)
            {
                info = new CacheInfo();
            }

            info.LastUpdateTime = DateTime.Now;
            info.Entity = await this.reteriver(key, token);

            this.map.AddOrUpdate(key, info, (k, v) => info);

            return info.Entity;
        }

        private class CacheInfo
        {
            public DateTime LastUpdateTime { get; set; }

            public TEntity Entity { get; set; }
        }
    }
}
