using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GroceryWebApp.Startup))]
namespace GroceryWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
