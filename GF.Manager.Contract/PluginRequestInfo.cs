using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GF.Manager.Contract
{
    public class PluginRequestInfo<T>
    {
        public PluginRequestInfo(List<PluginQueryParameter> parameters, T content)
        {
            this.Parameters = parameters;
            this.Content = content;
        }

        public List<PluginQueryParameter> Parameters { get; private set; }

        public T Content { get; private set; }
    }
}
