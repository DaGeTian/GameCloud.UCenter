using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Manager.TexasPoker.Startup))]
namespace Manager.TexasPoker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
