using Scrutor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Module.Extensions.DI
{
    public static class ImplementationTypeSelectorExtensions
    {

        public static IImplementationTypeSelector AddLifetime(this IImplementationTypeSelector implementationTypeSelector)
        {
            return
             implementationTypeSelector
            .AddClasses(classes => classes.AssignableTo<ITransientService>())
            .AsImplementedInterfaces()
            .WithTransientLifetime()
             .AddClasses(classes => classes.AssignableTo<IScopedService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ISingletonService>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime();
        }

    }
}
