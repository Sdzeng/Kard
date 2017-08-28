using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Kard.Module.Extensions.DI
{

    public static class ServiceCollectionExtensions
    {
       

        public static IModuleBuilder AddModule<TStartupModule>(this IServiceCollection services,Action<ModuleOptions> setupAction = null) where TStartupModule : KardModule
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var options = new ModuleOptions { };
            setupAction?.Invoke(options);

            var moduleBootstrapper = ModuleBootstrapper.Create<TStartupModule>(services);
            moduleBootstrapper.Initialize();
            //if (null == serviceProvider)
            //{
            //    IServiceProvider rootServiceProvider = services.BuildServiceProvider();
            //    serviceProvider = rootServiceProvider
            //        .GetService<IServiceScopeFactory>()
            //        .CreateScope()
            //        .ServiceProvider;

            //}

            return new ModuleBuilder(services);

        }


        public static T Resolve<T>(this IServiceCollection services)
        {
            return (T)services.Resolve(typeof(T));
        }

        public static object Resolve(this IServiceCollection services,Type t)
        {
            return services
                .LastOrDefault(d => d.ServiceType == t)
                ?.ImplementationInstance;
        }


        //public static IServiceCollection Scan(this IServiceCollection services, Action<IAssemblySelector> action)
        //{
        //    if (services == null)
        //    {
        //        throw new ArgumentNullException(nameof(services));
        //    }

        //    if (action == null)
        //    {
        //        throw new ArgumentNullException(nameof(action));
        //    }

        //    var selector = new AssemblySelector();

        //    action(selector);

        //    ((ISelector)selector).Populate(services);

        //    return services;
        //}


    }
}
