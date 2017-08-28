using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard
{
    public interface IModuleBuilder
    {
        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> where essential MVC services are configured.
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// Gets the <see cref="ApplicationPartManager"/> where <see cref="ApplicationPart"/>s
        /// are configured.
        /// </summary>
        //ApplicationPartManager PartManager { get; }
    }
}
