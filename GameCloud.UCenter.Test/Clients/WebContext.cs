using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Web.Http;
using System.Web.Http.SelfHost;
using GameCloud.UCenter;

namespace GameCloud.UCenter.Test.Clients
{
    [Export]
    public class WebContext : DisposableObjectSlim
    {
        private readonly HttpSelfHostConfiguration configuration;
        private readonly HttpSelfHostServer server;
        private readonly Settings settings;

        [ImportingConstructor]
        public WebContext(ExportProvider exportProvider, Settings settings)
        {
            this.settings = settings;
            this.BaseAddress = $"http://{this.settings.ServerHost}:{this.settings.ServerPort}";

            if (UseSelfHost())
            {
                try
                {
                    this.configuration = new HttpSelfHostConfiguration(this.BaseAddress);
                    this.configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
                    this.configuration.MapHttpAttributeRoutes();
                    WebApplicationManager.InitializeApplication(configuration, exportProvider);

                    this.server = new HttpSelfHostServer(configuration);
                    this.server.OpenAsync().Wait();
                }
                catch (Exception ex)
                {
                    CustomTrace.TraceError(ex);
                    throw;
                }
            }
        }

        public string BaseAddress { get; }

        protected override void DisposeInternal()
        {
            if (this.configuration != null)
            {
                this.configuration.Dispose();
            }

            if (this.server != null)
            {
                this.server.Dispose();
            }
        }

        private bool UseSelfHost()
        {
            return BaseAddress.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   BaseAddress.Contains("127.0.0.1");
        }
    }
}