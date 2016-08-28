using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCloud.Database;

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
    }
}
