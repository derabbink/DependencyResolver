using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SomeOtherAssembly
{
    //creates an actual dependency to SomeOtherAssembly.Reference
    public class Class1 : SomeOtherAssembly.Reference.Class1
    {
    }
}
