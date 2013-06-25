using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;

namespace DependencyResolver.Util
{
    internal static class AppDomainHelper
    {
        internal static AppDomain CreateTempDomain()
        {
            string name = GenerateDomainName();
            //need to reuse config, in order to get access to THIS assembly and dependencies
            //before others can be loaded into reflection context
            Evidence evidence = AppDomain.CurrentDomain.Evidence;
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;

            AppDomain domain = AppDomain.CreateDomain(name, evidence, setup);
            return domain;
        }

        private static string GenerateDomainName()
        {
            Guid guid = Guid.NewGuid();
            return string.Format("{0}-tmp.{1}", AssemblyHelper.GetOwnAssemblyName().Name, guid);
        }
    }
}
