using Kard.DI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Kard.Extensions
{

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 根据应用名称注入模块
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IModuleBuilder AddModule(this IServiceCollection services)
        {
            //Check.NotNull(services, nameof(services));

            //IHostingEnvironment hostingEnvironment = services.GetHostingEnvironment();
            //string startsWithFileName = hostingEnvironment?.ApplicationName?.Split(".")?.First();
            //Check.NotNullOrEmpty(startsWithFileName, nameof(startsWithFileName));

            //return services.AddModuleStartsWith(startsWithFileName);

            Check.NotNull(services, nameof(services));
            return services.AddDependenciesModule();
        }


        /// <summary>
        /// 根据名称开头注入模块
        /// </summary>
        /// <param name="services"></param>
        /// <param name="startsWithFileName"></param>
        /// <returns></returns>
        public static IModuleBuilder AddModuleStartsWith(this IServiceCollection services, string startsWithFileName)
        {
            Check.NotNull(services, nameof(services));


#if DEBUG
            string rootPtah = Path.Combine(Environment.CurrentDirectory, "bin/Debug/netcoreapp2.0");
#else

            string rootPtah = Environment.CurrentDirectory;
#endif


            var searchFiles = Directory.GetFiles(rootPtah, $"{startsWithFileName}*.*", SearchOption.TopDirectoryOnly)
                                          .Where(s => s.EndsWith(".dll") || s.EndsWith(".exe"))
                                          .ToArray();
            //var searchFiles = new string[] { "Kard.dll", "Kard.Dapper.dll", "Kard.Core.dll", "Kard.Web.dll" };
            //searchFiles=searchFiles.Select(s => $"D:\\GitRepository\\Kard\\Kard.Web\\bin/Debug/netcoreapp2.0\\{s}").ToArray();

            Func<List<Assembly>> loadAssemblies = () => (searchFiles ?? new string[0])
                                                                                        .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
                                                                                        .ToList();
            var lazyAssemblies = new Lazy<List<Assembly>>(loadAssemblies, true);
            var assemblies = lazyAssemblies.Value;

            services.ScanModule(scan => scan.FromAssemblies(assemblies));

            return new ModuleBuilder(services);
        }


        /// <summary>
        /// 注入引用模块
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IModuleBuilder AddDependenciesModule(this IServiceCollection services, Action<ModuleOptions> setupAction = null)
        {
            Check.NotNull(services, nameof(services));

            var options = new ModuleOptions { };
            setupAction?.Invoke(options);


            services.ScanModule(scan => scan.FromApplicationDependencies());

            return new ModuleBuilder(services);

        }

        private static void ScanModule(this IServiceCollection services, Func<ITypeSourceSelector,IImplementationTypeSelector> typeSelector)
        {
            services.Scan(scan => typeSelector(scan).AddTransientInterfaces());
            services.Scan(scan => typeSelector(scan).AddScopedInterfaces());
            services.Scan(scan => typeSelector(scan).AddSingletonInterfaces());
            services.Scan(scan => typeSelector(scan).AddAttributes());
        }

        public static IHostingEnvironment GetHostingEnvironment(this IServiceCollection services)
        {
            return services.GetService<IHostingEnvironment>();
        }


        public static T GetService<T>(this IServiceCollection services)
        {
            return (T)services.GetService(typeof(T));
        }

        public static object GetService(this IServiceCollection services, Type t)
        {
            return services
                .LastOrDefault(d => d.ServiceType == t)
                ?.ImplementationInstance;
        }



    }
}
