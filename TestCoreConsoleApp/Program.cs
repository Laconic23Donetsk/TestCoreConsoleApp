using Quartz;
using Quartz.Impl;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace TestCoreConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Test test = new Test();
            Console.WriteLine(test.Foo(1, 2));
        }
    }

    static class StaticClass
    {
        public static int Foo(this Test test, int x, int y)
        {
            Console.WriteLine("extension method");
            return x + y;
        }
    }


    internal class Test
    {
        public int X { get; set; }
        public int Y { get; set; }


        public double Foo(double x, double y)
        {
            return x + y;
        }

    }

    //First, there’s a matter of priority: if there’s a regular instance method that’s validfor  the method  invocation,  the compiler  will always  prefer that  over an  extensionmethod.It doesn’t matter whether the extension method has “better” parameters; ifthe compiler can use an instance method, it won’t even look for extension methods.
}
