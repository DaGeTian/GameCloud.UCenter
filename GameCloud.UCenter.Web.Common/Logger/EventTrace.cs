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
using GameCloud.UCenter.Database;

namespace GameCloud.UCenter.Web.Common.Logger
{
    [Export]
    public class EventTrace
    {
        internal const int BufferSize = 100;
        private readonly DatabaseContext context;
        private readonly ConcurrentStack<EntityBase> buffer = new ConcurrentStack<EntityBase>();
        private readonly ConcurrentDictionary<string, ICollectionAdapter<EntityBase>> adapters = new ConcurrentDictionary<string, ICollectionAdapter<EntityBase>>();

        [ImportingConstructor]
        public EventTrace(UCenterEventDatabaseContext context)
        {
            this.context = context;
        }

        public async Task TraceEvent<TEvent>(TEvent ev, CancellationToken token) where TEvent : EntityBase
        {
            this.buffer.Push(ev);

            if (this.buffer.Count >= BufferSize)
            {
                await this.Flush(token);
            }
        }

        public async Task Flush(CancellationToken token)
        {
            var count = this.buffer.Count;
            if (count > 0)
            {
                EntityBase[] tmp = new EntityBase[count];
                count = buffer.TryPopRange(tmp, 0, count);
                if (count > 0)
                {
                    try
                    {
                        buffer.Clear();
                        var tasks = tmp.Where(t => t != null)
                            .GroupBy(t => t.GetType())
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

        public long EventsInBuffer
        {
            get
            {
                return this.buffer.Count;
            }
        }
    }
}
