using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebFormsController.Startup))]
namespace WebFormsController
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
