using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UmbracoUnitTesting.TestWeb.Startup))]
namespace UmbracoUnitTesting.TestWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
