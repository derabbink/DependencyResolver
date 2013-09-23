using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace DependencyResolver.Test
{
    [TestFixture]
    public class DependencyResolver
    {
        /// <summary>
        /// tests whether the assembly resolver works on this test assembly
        /// </summary>
        [Test]
        public void GetAllTestDependencies()
        {
            Assembly self = Assembly.GetExecutingAssembly();
            AssemblyName name = self.GetName();

            List<string> actualNames = new List<string>();

            Resolver.GetAllDependencies(name).
                     Subscribe(an =>
                         {
                             actualNames.Add(an.Name);
                             Console.WriteLine("    {0} ({1})", an.Name, an.CodeBase);
                         },
                         ex =>
                         {
                             Console.WriteLine(ex.ToString());
                             throw ex;
                         });

            List<string> expectedNames = new List<string>()
                {
                    "nunit.framework",
                    "DependencyResolver.AppDomainHelper",
                    "System.Reactive.Interfaces",
                    "System.Reactive.Linq",
                    "DependencyResolver",
                    "System.Reactive.Core",
                    "DependencyResolver.Test"
                };
            
            CollectionAssert.AreEqual(actualNames, expectedNames);
        }

        [Test]
        public void GetPathAssemblies()
        {
            AssemblyName name = new AssemblyName("SomeOtherAssembly");
            List<string> actualNames = new List<string>();

            Resolver.GetAllDependencies(name).
                     Subscribe(an =>
                         {
                             actualNames.Add(an.Name);
                             Console.WriteLine("    {0} ({1})", an.Name, an.CodeBase);
                         },
                         ex =>
                             {
                                 Console.WriteLine(ex.ToString());
                                 throw ex;
                             });
            List<string> expectedNames = new List<string>()
                {
                    "SomeOtherAssembly.Reference",
                    "SomeOtherAssembly"
                };

            CollectionAssert.AreEqual(actualNames, expectedNames);
        }
    }
}
