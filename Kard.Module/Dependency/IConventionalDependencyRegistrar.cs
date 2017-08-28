using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Module.Dependency
{
    public interface IConventionalDependencyRegistrar
    {
        /// <summary>
        /// Registers types of given assembly by convention.
        /// </summary>
        /// <param name="context">Registration context</param>
        void RegisterAssembly(IConventionalRegistrationContext context);
    }
}
