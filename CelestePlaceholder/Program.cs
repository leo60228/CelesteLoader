using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CelestePlaceholder
{
    public class OriginalClass
    { // Placeholder for what's being patched
        public void unpatchedMethod()
        {
            Console.WriteLine("unpatchedMethod");
        }

        public void patchedMethod()
        {
            Console.WriteLine("old patchedMethod");
        }
    }

    public static class OriginalClass2
    { // Placeholder for what's being patched
        public static void unpatchedMethod()
        {
            Console.WriteLine("unpatchedMethod");
        }

        public static void patchedMethod()
        {
            Console.WriteLine("old patchedMethod");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var class1 = new OriginalClass();

            class1.patchedMethod();

            OriginalClass2.patchedMethod();
        }
    }
}
