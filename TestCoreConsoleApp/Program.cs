using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TestCoreConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Type type = typeof(Test);
            ConstructorInfo ctorInfo = type.GetConstructor(new[] { typeof(string) });
            ParameterInfo[] parameters = ctorInfo.GetParameters();

            foreach (ParameterInfo parameter in parameters)
            {
                if (parameter.ParameterType == typeof(string))
                Console.WriteLine(parameter.Name);
            }
        }
    }

    public class Test
    {
        public Test(string paramName)
        {

        }
    }
}
