using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EnglishSchool.Startup))]
namespace EnglishSchool
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
