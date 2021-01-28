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
            //1)
            //Expression<Func<int, int, int>> adder1 = (x, y) => x + y; //implicit converstion to Expression
            //Console.WriteLine(adder1);

            ////2) same with 1st
            //ParameterExpression xParameter = Expression.Parameter(typeof(int), "x"); 
            //ParameterExpression yParameter = Expression.Parameter(typeof(int), "y"); 
            //Expression body = Expression.Add(xParameter, yParameter); 
            //ParameterExpression[] parameters = new[] { xParameter, yParameter }; 
            //Expression<Func<int, int, int>> adder2 = Expression.Lambda<Func<int, int, int>>(body, parameters); 
            
            //Console.WriteLine(adder2);

            #region restrictions
            ////only  expression-bodied  lambda  expressions  can be converted   to   expression   trees.
            //Expression<Func<int, int, int>> adder = (x, y) => { return x + y; } //ERROR!

            #endregion
        }


    }

}
