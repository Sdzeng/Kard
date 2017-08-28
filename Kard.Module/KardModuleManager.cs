using Kard.Module.Extensions.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Kard.Module
{

    /// <summary>
    /// This class is used to manage modules.
    /// </summary>
    public class KardModuleManager : IKardModuleManager
    {
        public KardModuleInfo StartupModule { get; private set; }

        public IReadOnlyList<KardModuleInfo> Modules => _modules.ToImmutableList();

        public ILogger Logger { get; set; }

        private KardModuleCollection _modules;

        private readonly IServiceCollection _services;
 

        public KardModuleManager(IServiceCollection services,ILogger logger)
        {
            _services = services;
          
            Logger = logger;
        }

        public virtual void Initialize(Type startupModule)
        {
            _modules = new KardModuleCollection(startupModule);
            LoadAllModules();
        }

        public virtual void StartModules()
        {
            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.ForEach(module => module.Instance.PreInitialize());
            sortedModules.ForEach(module => module.Instance.Initialize());
            sortedModules.ForEach(module => module.Instance.PostInitialize());
        }

        public virtual void ShutdownModules()
        {
            Logger.LogDebug("Shutting down has been started");

            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Shutdown());

            Logger.LogDebug("Shutting down completed.");
        }

        private void LoadAllModules()
        {
            Logger.LogDebug("Loading Kard modules...");

            List<Type> plugInModuleTypes;
            var moduleTypes = FindAllModuleTypes(out plugInModuleTypes).Distinct().ToList();

            Logger.LogDebug("Found " + moduleTypes.Count + " ABP modules in total.");

            RegisterModules(moduleTypes);
            CreateModules(moduleTypes, plugInModuleTypes);

            _modules.EnsureKernelModuleToBeFirst();
            _modules.EnsureStartupModuleToBeLast();

            SetDependencies();

            Logger.LogDebug($"{_modules.Count} modules loaded.");
        }

        private List<Type> FindAllModuleTypes(out List<Type> plugInModuleTypes)
        {
            plugInModuleTypes = new List<Type>();

            var modules = KardModule.FindDependedModuleTypesRecursivelyIncludingGivenModule(_modules.StartupModuleType);

            return modules;
        }

        private void CreateModules(ICollection<Type> moduleTypes, List<Type> plugInModuleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                var moduleObject = _services.Resolve(moduleType) as KardModule;
                if (moduleObject == null)
                {
                    throw new KardInitializationException("This type is not an ABP module: " + moduleType.AssemblyQualifiedName);
                }

                moduleObject.Services = _services;
                //moduleObject.Configuration = _iocManager.Resolve<IKardStartupConfiguration>();

                var moduleInfo = new KardModuleInfo(moduleType, moduleObject, plugInModuleTypes.Contains(moduleType));

                _modules.Add(moduleInfo);

                if (moduleType == _modules.StartupModuleType)
                {
                    StartupModule = moduleInfo;
                }

                Logger.LogDebug($"Loaded module: {moduleType.AssemblyQualifiedName}");
            }
        }

        private void RegisterModules(ICollection<Type> moduleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                _services.AddSingleton(moduleType);
            }
        }

        private void SetDependencies()
        {
            foreach (var moduleInfo in _modules)
            {
                moduleInfo.Dependencies.Clear();

                //Set dependencies for defined DependsOnAttribute attribute(s).
                foreach (var dependedModuleType in KardModule.FindDependedModuleTypes(moduleInfo.Type))
                {
                    var dependedModuleInfo = _modules.FirstOrDefault(m => m.Type == dependedModuleType);
                    if (dependedModuleInfo == null)
                    {
                        throw new KardInitializationException("Could not find a depended module " + dependedModuleType.AssemblyQualifiedName + " for " + moduleInfo.Type.AssemblyQualifiedName);
                    }

                    if ((moduleInfo.Dependencies.FirstOrDefault(dm => dm.Type == dependedModuleType) == null))
                    {
                        moduleInfo.Dependencies.Add(dependedModuleInfo);
                    }
                }
            }
        }
    }
}
