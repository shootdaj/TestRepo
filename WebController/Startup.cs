using Microsoft.Owin;
using Owin;
using WebController;

[assembly: OwinStartup(typeof(Startup))]
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
