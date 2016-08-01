using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GF.UCenter.Manager.Startup))]
namespace GF.UCenter.Manager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
