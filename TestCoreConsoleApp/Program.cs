using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace TestCoreConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Type type = typeof(Newtonsoft.Json.IJsonLineInfo);
            Console.WriteLine(type);


            Type type2 = Type.GetType("Newtonsoft.Json.IJsonLineInfo, newtonsoft.json");
            Console.WriteLine(type2);
        }
    }
}
