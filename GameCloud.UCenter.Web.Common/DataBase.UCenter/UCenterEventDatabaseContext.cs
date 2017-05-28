using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCloud.Database;
using GameCloud.Database.Adapters;
using GameCloud.UCenter.Database.Entities;

namespace GameCloud.UCenter.Database
{
    [Export]
    public class UCenterEventDatabaseContext : DatabaseContext
    {
        [ImportingConstructor]
        public UCenterEventDatabaseContext(ExportProvider exportProvider, UCenterEventDatabaseContextSettings settings)
            : base(exportProvider, settings)
        {
        }

        /// <summary>
        /// Gets the account event adapter.
        /// </summary>
        public ICollectionAdapter<AccountEventEntity> AccountEvents
        {
            get
            {
                return this.GetAdapter<AccountEventEntity>();
            }
        }

        /// <summary>
        /// Gets the account error event adapter.
        /// </summary>
        public ICollectionAdapter<AccountErrorEventEntity> AccountErrorEvents
        {
            get
            {
                return this.GetAdapter<AccountErrorEventEntity>();
            }
        }

        /// <summary>
        /// Gets the exception event adapter.
        /// </summary>
        public ICollectionAdapter<ExceptionEventEntity> ExceptionEvents
        {
            get
            {
                return this.GetAdapter<ExceptionEventEntity>();
            }
        }
    }
}
