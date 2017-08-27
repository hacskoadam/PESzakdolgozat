using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(szaki.Startup))]
namespace szaki
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
