using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kard.Module.Reflection
{
    public class KardAssemblyFinder : IAssemblyFinder
    {
        private readonly IKardModuleManager _moduleManager;

        public KardAssemblyFinder(IKardModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
        }

        public List<Assembly> GetAllAssemblies()
        {
            var assemblies = new List<Assembly>();

            foreach (var module in _moduleManager.Modules)
            {
                assemblies.Add(module.Assembly);
                assemblies.AddRange(module.Instance.GetAdditionalAssemblies());
            }

            return assemblies.Distinct().ToList();
        }
    }
}
