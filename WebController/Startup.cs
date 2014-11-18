using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebController.Startup))]
namespace WebController
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
