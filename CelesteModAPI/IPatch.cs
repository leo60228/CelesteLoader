using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CelesteModAPI
{
    public interface IPatch
    { // In compile-time library (statically-linked)
        void patchHelper();
    }
}
