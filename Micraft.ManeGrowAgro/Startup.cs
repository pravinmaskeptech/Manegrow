using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Micraft.ManeGrowAgro.Startup))]
namespace Micraft.ManeGrowAgro
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
