﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DependencyResolver.AppDomainHelper.Data
{
    /// <summary>
    /// Data class containing an Assembly object's meta data.
    /// Immutable
    /// </summary>
    [Serializable]
    public class AssemblyMetaData
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

        /// <summary>
        /// Factory method to create an instance from just a path (and assembly name).
        /// Does not look into the file to try and analyse any manifests.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static AssemblyMetaData CreateFromPath(string name, string path)
        {
            AssemblyMetaData result = new AssemblyMetaData();
            result.Name = new AssemblyName(name);
            result.ReferencedAssemblies = new AssemblyName[0];
            result.CodeBase = (new Uri(path)).ToString();
            result.GlobalAssemblyCache = false;
            return result;
        }

        public AssemblyName Name { get; private set; }

        internal string CodeBase { get; private set; }

        public IEnumerable<AssemblyName> ReferencedAssemblies { get; private set; }

        public bool GlobalAssemblyCache { get; private set; }
    }
}
