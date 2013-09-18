using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using DependencyResolver.AppDomainHelper.Data;
using DependencyResolver.AppDomainHelper.Util;

namespace DependencyResolver
{
    public static class Resolver
    {
        /// <summary>
        /// Retrieves all assemblies that <paramref name="start"/> depends on.
        /// Does a recursive traversal, results are produced in loadable order.
        /// Includes <paramref name="start"/> as the final element
        /// </summary>
        /// <param name="start">assembly starting point</param>
        /// <returns></returns>
        public static IObservable<AssemblyName> GetAllDependencies(AssemblyName start)
        {
            AppDomain tempDomain = AppDomainHelper.Util.AppDomainHelper.CreateTempDomain();
            AssemblyLoader loader = AssemblyLoader.CreateInstanceInAppDomain(tempDomain);

            ISet<AssemblyName> assemblies = new HashSet<AssemblyName>(new AssemblyNameEqualityComparer());
            
            Func<AssemblyName, AssemblyMetaData> preProcess = loader.ReflectionOnlyLoad;
            Func<AssemblyName, bool> filter = an => !assemblies.Contains(an);
            Action<AssemblyName> postProcess = an => assemblies.Add(an);

            var result = GetAllDependenciesRecursive(start, filter, preProcess, postProcess).
                Finally(() => AppDomain.Unload(tempDomain));
            return result;
        }

        private static IObservable<AssemblyName> GetAllDependenciesRecursive(AssemblyName start, Func<AssemblyName, bool> filter, Func<AssemblyName, AssemblyMetaData> preProcess, Action<AssemblyName> postProcess)
        {
            AssemblyMetaData loaded = preProcess(start);

            //don't follow leads in the global assembly cache
            if (loaded.GlobalAssemblyCache)
            {
                postProcess(loaded.Name);
                return Observable.Empty<AssemblyName>();
            }

            IEnumerable<AssemblyName> parent = loaded.ReferencedAssemblies;
            IObservable<AssemblyName> result = parent.ToObservable()
                                                     .Where(filter)
                                                     .Select(an => GetAllDependenciesRecursive(an, filter, preProcess, postProcess))
                                                     .Concat()
                                                     .Concat(Observable.Return(loaded.Name)); //loaded.Name contains correct CodeBase
            //this cannot be part of the linq chain,
            //it must be executed before the recursive descend
            postProcess(start);
            return result;
        }
    }
}
