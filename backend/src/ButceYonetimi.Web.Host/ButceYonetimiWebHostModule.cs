using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ButceYonetimi.EntityFrameworkCore;

namespace ButceYonetimi.Web.Host
{
    [DependsOn(typeof(ButceYonetimiApplicationModule), typeof(ButceYonetimiEntityFrameworkCoreModule), typeof(AbpAspNetCoreModule))]
    public class ButceYonetimiWebHostModule : AbpModule
    {
        public override void PreInitialize()
        {
            // Expose all AppServices as dynamic API controllers
            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(ButceYonetimiApplicationModule).GetAssembly(),
                    moduleName: "app",
                    useConventionalHttpVerbs: true
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ButceYonetimiWebHostModule).GetAssembly());
        }
    }
}
