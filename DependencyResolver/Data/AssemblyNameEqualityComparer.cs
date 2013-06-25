using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DependencyResolver.Data
{
    internal class AssemblyNameEqualityComparer : IEqualityComparer<AssemblyName>
    {
        public bool Equals(AssemblyName x, AssemblyName y)
        {
            return x.FullName.Equals(y.FullName);
        }

        public int GetHashCode(AssemblyName obj)
        {
            return obj.FullName.GetHashCode();
        }
    }
}
