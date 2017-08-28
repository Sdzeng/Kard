using JetBrains.Annotations;
using Kard.Module.Extensions.DI;
using Kard.Module.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace Kard.Module
{

    public class ModuleBootstrapper : IDisposable
    {
        public Type StartupModule { get; }

        public IServiceCollection Services { get; }

        private KardModuleManager _moduleManager;
        private ILogger _logger;

        private ModuleBootstrapper([NotNull] Type startupModule, [NotNull] IServiceCollection services)
        {
            Check.NotNull(startupModule, nameof(startupModule));
            Check.NotNull(services, nameof(services));

            if (!typeof(KardModule).GetTypeInfo().IsAssignableFrom(startupModule))
            {
                throw new ArgumentException($"{nameof(startupModule)} should be derived from {nameof(KardModule)}.");
            }

            StartupModule = startupModule;
          
            Services = services;
        }



        public static ModuleBootstrapper Create<TStartupModule>(IServiceCollection services)
         where TStartupModule : KardModule
        {
            return new ModuleBootstrapper(typeof(TStartupModule), services);
        }


        public virtual void Initialize()
        {
            ResolveLogger();

            try
            {
                RegisterBootstrapper();

               
                Services.AddSingleton<ITypeFinder, TypeFinder>();
                Services.AddSingleton<IKardModuleManager, KardModuleManager>();
                Services.AddSingleton<IAssemblyFinder, KardAssemblyFinder>();

                _moduleManager = Services.Resolve<KardModuleManager>();
                _moduleManager.Initialize(StartupModule);
                _moduleManager.StartModules();
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex.ToString(), ex);
                throw;
            }
        }

        private void ResolveLogger()
        {
            var loggerFactory = Services.Resolve<ILoggerFactory>();
            _logger= loggerFactory.CreateLogger(typeof(ModuleBootstrapper));
        }

   

        private void RegisterBootstrapper()
        {
            Services.AddSingleton(this);
        }

        public virtual void Dispose()
        {
            //if (IsDisposed)
            //{
            //    return;
            //}

            //IsDisposed = true;

            //_moduleManager?.ShutdownModules();
        }
    }
}
