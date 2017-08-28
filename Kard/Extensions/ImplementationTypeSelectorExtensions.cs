using Kard.DI;
using Scrutor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Extensions
{
    public static class ImplementationTypeSelectorExtensions
    {

        public static IImplementationTypeSelector AddTransientInterfaces(this IImplementationTypeSelector implementationTypeSelector)
        {
            return
             implementationTypeSelector
            .AddClasses(classes => classes.AssignableTo<ITransientService>())
            .AsImplementedInterfaces()
            .WithTransientLifetime();
        }

        public static IImplementationTypeSelector AddScopedInterfaces(this IImplementationTypeSelector implementationTypeSelector)
        {
            return
             implementationTypeSelector
             .AddClasses(classes => classes.AssignableTo<IScopedService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime();

        }

        public static IImplementationTypeSelector AddSingletonInterfaces(this IImplementationTypeSelector implementationTypeSelector)
        {
            return
             implementationTypeSelector
            .AddClasses(classes => classes.AssignableTo<ISingletonService>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime();
        }

        public static IImplementationTypeSelector AddAttributes(this IImplementationTypeSelector implementationTypeSelector)
        {
            return
             implementationTypeSelector
            .AddClasses()
            .UsingAttributes();
        }

    }
}
