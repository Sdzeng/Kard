using Kard.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Kard.Core;

namespace Kard.Web
{
    [DependsOn(typeof(CoreModule))]
    public class WebModule : KardModule
    {
     
    }
}
