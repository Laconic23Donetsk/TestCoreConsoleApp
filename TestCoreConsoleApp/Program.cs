using Quartz;
using Quartz.Impl;
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
            //var addWithConsoleLogger = AdderWithPluggableLogger(ConsoleLogger);
            //addWithConsoleLogger(1, 2);
            //addWithConsoleLogger(42, 99);

            // Or to do make all the calls together…
            var curried = Curry<int,int,int,int>(Foo);
            var res1 = curried(5)(5);
            Console.WriteLine(res1(10));
            //int result3 = curried(1)(2)(3);
        }


        static Func<T1, Func<T2, Func<T3, TResult>>> Curry<T1, T2, T3, TResult> (Func<T1, T2, T3, TResult> function)
        {
            return a => b => c => function(a, b, c);
        }

        static int Foo(int a, int b, int c)
        {
            Console.WriteLine("a+b+c = " + (a + b + c).ToString());
            return a + b + c;
        }


        delegate int binop(int a, int b);
        //binop AdderWithPluggableLogger(Action<string, int> logger)
        //{
        //    int addWithLogger(int x, int y)
        //    {
        //        logger("x", x);
        //        logger("y", y);
        //        var result = x + y;
        //        logger("x+y", result);
        //        return result;
        //    }

        //    return addWithLogger;
        //}

        //binop AdderWithPluggableLogger(Action<string, int> logger) => (int x, int y) =>
        //{
        //    logger("x", x);
        //    logger("y", y);
        //    var result = x + y;
        //    logger("x+y", result);
        //    return result;
        //};

        //Func<int, int> adderWithPluggableLogger(Action<string, int> logger) => (int x) => (int y) =>
        //{
        //    logger("x", x);
        //    logger("y", y);
        //    var result = x + y;
        //    logger("x+y", result);
        //    return result;
        //};


        void ConsoleLogger(string argName, int argValue) => Console.WriteLine($"{argName}={argValue}");
    }
}
