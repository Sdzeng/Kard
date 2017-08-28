﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Module
{
    public class ModuleBuilder: IModuleBuilder
    {

        public ModuleBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public  IServiceCollection Services { get; }
    }
}
