using Kard.Module.Extensions.DI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Module.Dependency
{
    public class BasicConventionalRegistrar : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            //Transient
            context.Services.Scan(scan => scan.FromAssemblies(context.Assembly)
            
                        .AddClasses(classes => classes.AssignableTo<ITransientService>())
                        .AsImplementedInterfaces()
                       .WithTransientLifetime());




            ////Singleton
            //context.IocManager.IocContainer.Register(
            //    Classes.FromAssembly(context.Assembly)
            //        .IncludeNonPublicTypes()
            //        .BasedOn<ISingletonDependency>()
            //        .If(type => !type.GetTypeInfo().IsGenericTypeDefinition)
            //        .WithService.Self()
            //        .WithService.DefaultInterfaces()
            //        .LifestyleSingleton()
            //    );

            context.Services.Scan(scan => scan.FromAssemblies(context.Assembly)

                   .AddClasses(classes => classes.AssignableTo<ISingletonService>())
                   .AsImplementedInterfaces()
                  .WithSingletonLifetime());

            ////Windsor Interceptors
            //context.IocManager.IocContainer.Register(
            //    Classes.FromAssembly(context.Assembly)
            //        .IncludeNonPublicTypes()
            //        .BasedOn<IInterceptor>()
            //        .If(type => !type.GetTypeInfo().IsGenericTypeDefinition)
            //        .WithService.Self()
            //        .LifestyleTransient()
            //    );

            context.Services.Scan(scan => scan.FromAssemblies(context.Assembly)

               .AddClasses(classes => classes.AssignableTo<IScopedService>())
               .AsImplementedInterfaces()
              .WithScopedLifetime());


            //ServiceDescriptorAttribute
            context.Services.Scan(scan => scan.FromAssemblies(context.Assembly)
                      .AddClasses()
                      .UsingAttributes());
        }
    }
}
