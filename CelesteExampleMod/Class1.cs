using CelesteModAPI;
using CelestePlaceholder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CelesteExampleMod
{
    public class CelesteExampleMod
    {
        public class ExamplePatch<T> : OriginalClass where T : IPatch, new()
        { // In user mod
            T patch = new T();

            public new void patchedMethod()
            {
                Console.WriteLine("start new patchedMethod");
                base.patchedMethod();
                newMethod();
                Console.WriteLine("end new patchedMethod");

            }

            public void newMethod()
            {
                Console.WriteLine("newMethod");
                unpatchedMethod();
                patch.patchHelper();
            }
        }
    }
}
