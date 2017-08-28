using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Module
{
    public interface IKardModuleManager
    {
        KardModuleInfo StartupModule { get; }

        IReadOnlyList<KardModuleInfo> Modules { get; }

        void Initialize(Type startupModule);

        void StartModules();

        void ShutdownModules();
    }
}
