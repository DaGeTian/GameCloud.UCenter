using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameCloud.Manager.PluginContract.Requests
{
    public class UpdateRequestInfo<TBody> : PluginRequestInfo
    {
        public UpdateRequestInfo(PluginRequestMethod method, List<PluginRequestParameter> parameters)
            : base(method, parameters)
        {
        }

        public TBody Body
        {
            get
            {
                return this.GetParameterValue<TBody>("data");
            }
        }
    }
}
