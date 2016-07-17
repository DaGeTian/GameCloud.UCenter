using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GF.Manager.UCenter.Startup))]
namespace GF.Manager.UCenter
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
