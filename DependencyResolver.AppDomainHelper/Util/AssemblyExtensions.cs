using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DependencyResolver.AppDomainHelper.Util
{
    internal static class AssemblyExtensions
    {
        private static List<string> _extensions = new List<string>() {".dll", ".exe"};
        
        internal static bool Contains(string item)
        {
            return _extensions.Contains(item.ToLowerInvariant());
        }
    }
}
