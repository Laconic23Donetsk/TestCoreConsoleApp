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
            var class1 = new Class1<int>(3);
            var class2 = new Class1<int, int>(1, 2);

            class1.Foo();
            class2.Foo();

        }
    }

    class Class1<T> 
    {
        public Class1(T arg1)
        {

        }

        public void Foo()
        {
            Console.WriteLine("version 1");
        }
    }

    class Class1<T1, T2>
    {
        public Class1(T1 arg1, T2 arg2)
        {

        }

        public void Foo()
        {
            Console.WriteLine("version 2");
        }
    }
}
