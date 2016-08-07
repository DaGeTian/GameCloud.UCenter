using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using GF.Manager.Contract.Configuration;

namespace GF.Manager.Contract
{
    public class PluginEntryPoint
    {
        private string plugName;

        public PluginEntryPoint()
        {
            this.Configuration = new PluginConfiguration(this.PluginName);
        }

        public string PluginName
        {
            get
            {
                if (plugName == null)
                {
                    plugName = this.GetType().GetCustomAttribute<PluginMetadataAttribute>().Name;
                }

                return plugName;
            }
        }

        public PluginConfiguration Configuration { get; private set; }
    }
}
