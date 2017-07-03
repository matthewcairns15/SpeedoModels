using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SpeedoModels.Startup))]
namespace SpeedoModels
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
 
}
