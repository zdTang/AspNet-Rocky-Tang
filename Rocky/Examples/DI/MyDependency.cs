using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Examples.DI
{
    public class MyDependency:IMyDependency
    {
        public void WriteMessage(string message)
        {
            Console.WriteLine($"MyDependency.WriteMessage Message: {message}");
        }
    }
}
