using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kard.Module
{
  
    /// <summary>
    /// Kernel (core) module of the ABP system.
    /// No need to depend on this, it's automatically the first module always.
    /// </summary>
    public sealed class KardKernelModule : KardModule
    {
         

        public override void PreInitialize()
        {
            //IocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());

            //IocManager.Register<IScopedIocResolver, ScopedIocResolver>(DependencyLifeStyle.Transient);
            //IocManager.Register(typeof(IAmbientScopeProvider<>), typeof(DataContextAmbientScopeProvider<>), DependencyLifeStyle.Transient);

           
            //AddSettingProviders();
           
            //ConfigureCaches();
            //AddIgnoredTypes();
        }

        public override void Initialize()
        {
            //foreach (var replaceAction in ((KardStartupConfiguration)Configuration).ServiceReplaceActions.Values)
            //{
            //    replaceAction();
            //}

            //IocManager.IocContainer.Install(new EventBusInstaller(IocManager));

            //IocManager.RegisterAssemblyByConvention(typeof(KardKernelModule).GetAssembly(),
            //    new ConventionalRegistrationConfig
            //    {
            //        InstallInstallers = false
            //    });
        }

        public override void PostInitialize()
        {
            //RegisterMissingComponents();

            //IocManager.Resolve<SettingDefinitionManager>().Initialize();
            //IocManager.Resolve<FeatureManager>().Initialize();
      
            //IocManager.Resolve<NotificationDefinitionManager>().Initialize();
            //IocManager.Resolve<NavigationManager>().Initialize();

         
        }

        public override void Shutdown()
        {
           
        }
 
        private void AddSettingProviders()
        {
        
            //Configuration.Settings.Providers.Add<NotificationSettingProvider>();
           
        }

      
    

        private void ConfigureCaches()
        {
            //Configuration.Caching.Configure(KardCacheNames.ApplicationSettings, cache =>
            //{
            //    cache.DefaultSlidingExpireTime = TimeSpan.FromHours(8);
            //});

            //Configuration.Caching.Configure(KardCacheNames.TenantSettings, cache =>
            //{
            //    cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(60);
            //});

            //Configuration.Caching.Configure(KardCacheNames.UserSettings, cache =>
            //{
            //    cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(20);
            //});
        }

        private void AddIgnoredTypes()
        {
            //var commonIgnoredTypes = new[]
            //{
            //    typeof(Stream),
            //    typeof(Expression)
            //};

            //foreach (var ignoredType in commonIgnoredTypes)
            //{
            //    Configuration.Auditing.IgnoredTypes.AddIfNotContains(ignoredType);
            //    Configuration.Validation.IgnoredTypes.AddIfNotContains(ignoredType);
            //}

            //var validationIgnoredTypes = new[] { typeof(Type) };
            //foreach (var ignoredType in validationIgnoredTypes)
            //{
            //    Configuration.Validation.IgnoredTypes.AddIfNotContains(ignoredType);
            //}
        }

        //private void RegisterMissingComponents()
        //{
        //    if (!IocManager.IsRegistered<IGuidGenerator>())
        //    {
        //        IocManager.IocContainer.Register(
        //            Component
        //                .For<IGuidGenerator, SequentialGuidGenerator>()
        //                .Instance(SequentialGuidGenerator.Instance)
        //        );
        //    }

            
        //    IocManager.RegisterIfNot<INotificationStore, NullNotificationStore>(DependencyLifeStyle.Singleton);
        
        //}
    }
}
