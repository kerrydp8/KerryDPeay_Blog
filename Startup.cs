using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KerryDPeay_Blog.Startup))]
namespace KerryDPeay_Blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
