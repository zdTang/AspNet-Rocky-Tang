using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Examples.DI
{
    public interface IMyDependency
    {
        public void WriteMessage(string message);
    }
}
