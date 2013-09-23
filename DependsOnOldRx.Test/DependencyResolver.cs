using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DependencyResolver;
using NUnit.Framework;

namespace DependsOnOldRx.Test
{
    [TestFixture]
    public class DependencyResolver
    {
        [Test]
        public void DifferentRxVersions()
        {
            AssemblyName name = new AssemblyName("DependsOnOldRx");
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
                    "System.Reactive.Interfaces",
                    "System.Reactive.Linq",
                    "System.Reactive.Core",
                    "DependsOnOldRx"
                };

            CollectionAssert.AreEqual(actualNames, expectedNames);
        }
    }
}
