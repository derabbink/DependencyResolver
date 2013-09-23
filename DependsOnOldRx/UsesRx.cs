using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

namespace DependsOnOldRx
{
    public class UsesRx
    {
        private void SomeMethod()
        {
            char[] alphabet = new[]
                {
                    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                    'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
            IObservable<char> obs = alphabet.ToObservable();
            obs.Subscribe(PrintChar);
        }

        private void PrintChar(char c)
        {
            Debug.WriteLine("{0}", c);
        }
    }
}
