using Kard.Core.IRepositories;
using Kard.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Kard.Core.AppServices.Test
{
    public class TestAppService : ITestAppService
    {

        public TestAppService(IServiceProvider serviceProvider)
        {
            //scope.Dispose()-->childProvider.Dispose()删除对Service实例的引用
            using (var scope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
            {
                var childProvider = scope.ServiceProvider;

                var r1 = serviceProvider.GetService<ITestRepository>();
                var r2 = childProvider.GetService<ITestRepository>();
                bool result = (r1.GetHashCode().Equals(r2.GetHashCode()));
                string ee = result.ToString();
            }

        }

        public string Hello()
        {
            return "hello world";
        }
    }
}
