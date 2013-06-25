using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DependencyResolver.Util
{
    internal static class AssemblyHelper
    {
        internal static AssemblyName GetOwnAssemblyName()
        {
            return Assembly.GetExecutingAssembly().GetName();
        }
    }
}
