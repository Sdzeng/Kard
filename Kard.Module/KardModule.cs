using Kard.Module.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Kard.Module
{
    public abstract class KardModule
    {

        /// <summary>
        /// Gets a reference to the IOC manager.
        /// </summary>
        protected internal IServiceCollection Services { get; internal set; }

        /// <summary>
        /// Gets a reference to the ABP configuration.
        /// </summary>
        //protected internal IKardStartupConfiguration Configuration { get; internal set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        protected KardModule()
        {
         
        }

        //protected KardModule(IServiceCollection services, ILogger logger)
        //{
        //    Services = services;
        //    Logger = logger;
        //}

        /// <summary>
        /// This is the first event called on application startup. 
        /// Codes can be placed here to run before dependency injection registrations.
        /// </summary>
        public virtual void PreInitialize()
        {

        }

        /// <summary>
        /// This method is used to register dependencies for this module.
        /// </summary>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// This method is called lastly on application startup.
        /// </summary>
        public virtual void PostInitialize()
        {

        }

        /// <summary>
        /// This method is called when the application is being shutdown.
        /// </summary>
        public virtual void Shutdown()
        {

        }

        public virtual Assembly[] GetAdditionalAssemblies()
        {
            return new Assembly[0];
        }

        /// <summary>
        /// Checks if given type is an Kard module class.
        /// </summary>
        /// <param name="type">Type to check</param>
        public static bool IsKardModule(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return
                typeInfo.IsClass &&
                !typeInfo.IsAbstract &&
                !typeInfo.IsGenericType &&
                typeof(KardModule).IsAssignableFrom(type);
        }

        /// <summary>
        /// Finds direct depended modules of a module (excluding given module).
        /// </summary>
        public static List<Type> FindDependedModuleTypes(Type moduleType)
        {
            if (!IsKardModule(moduleType))
            {
                throw new KardInitializationException("This type is not an ABP module: " + moduleType.AssemblyQualifiedName);
            }

            var list = new List<Type>();

            if (moduleType.GetTypeInfo().IsDefined(typeof(DependsOnAttribute), true))
            {
                var dependsOnAttributes = moduleType.GetTypeInfo().GetCustomAttributes(typeof(DependsOnAttribute), true).Cast<DependsOnAttribute>();
                foreach (var dependsOnAttribute in dependsOnAttributes)
                {
                    foreach (var dependedModuleType in dependsOnAttribute.DependedModuleTypes)
                    {
                        list.Add(dependedModuleType);
                    }
                }
            }

            return list;
        }

        public static List<Type> FindDependedModuleTypesRecursivelyIncludingGivenModule(Type moduleType)
        {
            var list = new List<Type>();
            AddModuleAndDependenciesRecursively(list, moduleType);
            list.AddIfNotContains(typeof(KardKernelModule));
            return list;
        }

        private static void AddModuleAndDependenciesRecursively(List<Type> modules, Type module)
        {
            if (!IsKardModule(module))
            {
                throw new KardInitializationException("This type is not an ABP module: " + module.AssemblyQualifiedName);
            }

            if (modules.Contains(module))
            {
                return;
            }

            modules.Add(module);

            var dependedModules = FindDependedModuleTypes(module);
            foreach (var dependedModule in dependedModules)
            {
                AddModuleAndDependenciesRecursively(modules, dependedModule);
            }
        }
    }
}
