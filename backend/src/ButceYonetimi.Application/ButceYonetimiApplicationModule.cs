using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace ButceYonetimi
{
    [DependsOn(typeof(ButceYonetimiCoreModule), typeof(AbpAutoMapperModule))]
    public class ButceYonetimiApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg =>
            {
                cfg.AddMaps(typeof(ButceYonetimiApplicationModule).GetAssembly());
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ButceYonetimiApplicationModule).GetAssembly());
        }
    }
}
