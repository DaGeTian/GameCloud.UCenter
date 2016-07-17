using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GF.Manager.TexasPoker.Startup))]
namespace GF.Manager.TexasPoker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
