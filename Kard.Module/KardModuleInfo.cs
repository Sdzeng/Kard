using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Kard.Module
{
    public class KardModuleInfo
    {
        /// <summary>
        /// The assembly which contains the module definition.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Type of the module.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Instance of the module.
        /// </summary>
        public KardModule Instance { get; }

        /// <summary>
        /// Is this module loaded as a plugin.
        /// </summary>
        public bool IsLoadedAsPlugIn { get; }

        /// <summary>
        /// All dependent modules of this module.
        /// </summary>
        public List<KardModuleInfo> Dependencies { get; }

        /// <summary>
        /// Creates a new AbpModuleInfo object.
        /// </summary>
        public KardModuleInfo([NotNull] Type type, [NotNull] KardModule instance, bool isLoadedAsPlugIn)
        {
            Check.NotNull(type, nameof(type));
            Check.NotNull(instance, nameof(instance));

            Type = type;
            Instance = instance;
            IsLoadedAsPlugIn = isLoadedAsPlugIn;
            Assembly = Type.GetTypeInfo().Assembly;

            Dependencies = new List<KardModuleInfo>();
        }

        public override string ToString()
        {
            return Type.AssemblyQualifiedName ??
                   Type.FullName;
        }
    }
}
