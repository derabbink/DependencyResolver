using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DependencyResolver.AppDomainHelper.Data;

namespace DependencyResolver.AppDomainHelper.Util
{
    /// <summary>
    /// Wrapper class for assembly loading instructions
    /// extends MarshalByRefObject to invoke methods in other AppDomain
    /// </summary>
    public class AssemblyLoader : MarshalByRefObject
    {
        private IList<string> _assemblyPaths;

        /// <summary>
        /// constructur is public for reflection
        /// </summary>
        public AssemblyLoader()
        {
            LoadAssemblyPaths();
        }

        private void LoadAssemblyPaths()
        {
            _assemblyPaths = new List<string>();
            string paths = ConfigurationManager.AppSettings.Get("DependencyResolver.assembly-path");
            if (!string.IsNullOrEmpty(paths))
            {
                IEnumerable<string> dirs = paths.Split(Path.PathSeparator).Where(s => !string.IsNullOrEmpty(s));
                foreach (string dir in dirs)
                {
                    if (Directory.Exists(dir))
                        _assemblyPaths.Add(dir);
                }
            }
        }

        public static AssemblyLoader CreateInstanceInAppDomain(AppDomain domain)
        {
            AssemblyName ownAssyName = AssemblyHelper.GetOwnAssemblyName();
            Assembly assy = domain.Load(ownAssyName);
            AssemblyName assyName = assy.GetName(); //same as GetOwnAssemblyName()
            string typename = typeof(AssemblyLoader).FullName;
            return domain.CreateInstanceAndUnwrap(assyName.Name, typename) as AssemblyLoader;
        }

        public AssemblyMetaData ReflectionOnlyLoad(AssemblyName assembly)
        {
            try
            {
                AssemblyMetaData result = AssemblyMetaData.CreateFromAssembly(Assembly.ReflectionOnlyLoad(assembly.FullName));

                if (AssemblyIsInBasePath(result))
                    throw new FileNotFoundException("Not loading anything from implicit assembly path.");
                else
                    return result;
            }
            catch (FileNotFoundException ex)
            {
                //there is no AssemblyResolve or ReflectionOnlyAssemblyResolve event fired
                //if ReflectionOnlyLoad fails, so this is a custom implementation for that
                return ReflectionOnlyLoadFromAssemblyPath(assembly, ex);
            }
        }

        private bool AssemblyIsInBasePath(AssemblyMetaData assembly)
        {
            string basedir = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
            string compareDir = Path.GetFullPath((new Uri(assembly.CodeBase)).LocalPath);
            while (!string.IsNullOrEmpty(compareDir))
            {
                if (compareDir == basedir)
                    return true;
                compareDir = Path.GetDirectoryName(compareDir);
            }
            return false;
        }

        private AssemblyMetaData ReflectionOnlyLoadFromAssemblyPath(AssemblyName assembly, FileNotFoundException originalException)
        {
            foreach (string dir in _assemblyPaths)
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                IEnumerable<FileInfo> files = di.EnumerateFiles().Where(fi => AssemblyExtensions.Contains(fi.Extension));
                foreach (FileInfo fi in files)
                {
                    try
                    {
                        AssemblyName name = AssemblyName.GetAssemblyName(fi.FullName);
                        if (name.FullName == assembly.FullName || name.Name == assembly.Name)
                            return AssemblyMetaData.CreateFromAssembly(Assembly.ReflectionOnlyLoadFrom(fi.FullName));
                    }
                    catch (BadImageFormatException)
                    {
                        //this happens if the assembly does not have a manifest.
                        //We'll assume it's the file we're looking for.
                        if (assembly.Name == Path.GetFileNameWithoutExtension(fi.Name))
                            return AssemblyMetaData.CreateFromPath(assembly.Name, fi.FullName);
                    }
                }
            }

            //if we haven't returned by now, reuse the exception
            throw originalException;
        }
    }
}
