using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DependencyResolver.Data
{
    /// <summary>
    /// Data class containing an Assembly object's meta data.
    /// Immutable
    /// </summary>
    [Serializable]
    internal class AssemblyMetaData
    {
        private AssemblyMetaData() {}

        /// <summary>
        /// Factory method to create an instance from an Assembly object.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        internal static AssemblyMetaData CreateFromAssembly(Assembly assembly)
        {
            AssemblyMetaData result = new AssemblyMetaData();
            result.Name = assembly.GetName();
            result.ReferencedAssemblies = assembly.GetReferencedAssemblies();
            result.CodeBase = assembly.CodeBase;
            result.GlobalAssemblyCache = assembly.GlobalAssemblyCache;
            return result;
        }

        internal AssemblyName Name { get; private set; }

        internal string CodeBase { get; private set; }

        internal IEnumerable<AssemblyName> ReferencedAssemblies { get; private set; }

        internal bool GlobalAssemblyCache { get; private set; }
    }
}
