using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Kard.DI
{
    public static class KardIoc
    {

        public static IServiceProvider ServiceProvider { private get; set; }

        public static T GetService<T>()
        {
            return  ServiceProvider.GetService<T>();
        }

        public static IServiceScope CreateScope()
        {
            return ServiceProvider.GetService<IServiceScopeFactory>().CreateScope();
        }
    }
}
