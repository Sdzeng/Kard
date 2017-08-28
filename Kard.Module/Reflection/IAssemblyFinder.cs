using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Kard.Module.Reflection
{
 
    public interface IAssemblyFinder
    {
        /// <summary>
        /// Gets all assemblies.
        /// </summary>
        /// <returns>List of assemblies</returns>
        List<Assembly> GetAllAssemblies();
    }
}
