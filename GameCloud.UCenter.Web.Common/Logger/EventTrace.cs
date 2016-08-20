using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.Database;
using GameCloud.Database.Adapters;
using GameCloud.Database.Attributes;

namespace GameCloud.UCenter.Web.Common.Logger
{
    [Export]
    public class EventTrace
    {
        internal const int BufferSize = 100;

        private readonly DatabaseContext context;
        private readonly ConcurrentBag<EntityBase> buffer = new ConcurrentBag<EntityBase>();
        private readonly ConcurrentDictionary<string, ICollectionAdapter<EntityBase>> adapters = new ConcurrentDictionary<string, ICollectionAdapter<EntityBase>>();
        
        [ImportingConstructor]
        public EventTrace(DatabaseContext context)
        {
            this.context = context;
        }

        public async Task TraceEvent<TEvent>(TEvent ev, CancellationToken token) where TEvent : EntityBase
        {
            this.buffer.Add(ev);

            if (this.buffer.Count >= BufferSize)
            {
                await this.Flush(token);
            }
        }

        public async Task Flush(CancellationToken token)
        {
            var tmp = buffer.ToList();
            if (tmp.Count > 0)
            {
                try
                {
                    var tasks = tmp.GroupBy(t => t.GetType())
                        .AsParallel()
                        .Select(async g =>
                        {
                            var collectionName = g.Key.GetCustomAttribute<CollectionNameAttribute>().CollectionName;
                            var adapter = this.adapters.GetOrAdd(collectionName, key => new CollectionAdapter<EntityBase>(this.context, key));
                            await adapter.InsertManyAsync(g.ToList(), token);
                        });

                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    CustomTrace.TraceError(ex);
                }
            }
        }
    }
}
