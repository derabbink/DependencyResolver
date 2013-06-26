using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DependencyResolver.Data;

namespace DependencyResolver.Util
{
    /// <summary>
    /// Wrapper class for assembly loading instructions
    /// extends MarshalByRefObject to invoke methods in other AppDomain
    /// </summary>
    internal class AssemblyLoader : MarshalByRefObject
    {
        private readonly string _assemblyPath;

        /// <summary>
        /// constructur is public for reflection
        /// </summary>
        public AssemblyLoader()
        {
            _assemblyPath = ConfigurationManager.AppSettings.Get("DependencyResolver.assembly-path");
        }

        internal static AssemblyLoader CreateInstanceInAppDomain(AppDomain domain)
        {
            AssemblyName ownAssyName = AssemblyHelper.GetOwnAssemblyName();
            Assembly assy = domain.Load(ownAssyName);
            AssemblyName assyName = assy.GetName(); //same as GetOwnAssemblyName()
            string typename = typeof(AssemblyLoader).FullName;
            return domain.CreateInstanceAndUnwrap(assyName.Name, typename) as AssemblyLoader;
        }

        internal AssemblyMetaData ReflectionOnlyLoad(AssemblyName assembly)
        {
            try
            {
                return AssemblyMetaData.CreateFromAssembly(Assembly.ReflectionOnlyLoad(assembly.FullName));
            }
            catch (FileNotFoundException ex)
            {
                //there is no AssemblyResolve or ReflectionOnlyAssemblyResolve event fired
                //if ReflectionOnlyLoad fails, so this is a custom implementation for that
                return ReflectionOnlyLoadFromAssemblyPath(assembly, ex);
            }
        }

        private AssemblyMetaData ReflectionOnlyLoadFromAssemblyPath(AssemblyName assembly, FileNotFoundException originalException)
        {
            IEnumerable<string> paths = _assemblyPath.Split(Path.PathSeparator);
            foreach (string p in paths)
            {
                DirectoryInfo dir = new DirectoryInfo(p);
                IEnumerable<FileInfo> files = dir.EnumerateFiles();
                foreach (FileInfo f in files)
                {
                    if (!AssemblyExtensions.Contains(f.Extension))
                        continue;

                    AssemblyName name = AssemblyName.GetAssemblyName(f.FullName);
                    if (name.FullName == assembly.FullName || name.Name == assembly.Name)
                        return AssemblyMetaData.CreateFromAssembly(Assembly.ReflectionOnlyLoadFrom(f.FullName));
                }
            }

            //if we haven't returned by now, reuse the exception
            throw originalException;
        }
    }
}
