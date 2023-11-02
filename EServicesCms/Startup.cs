using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(EServicesCms.Startup))]

namespace EServicesCms
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}