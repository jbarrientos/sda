using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SDA.WebApp.Startup))]
namespace SDA.WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
