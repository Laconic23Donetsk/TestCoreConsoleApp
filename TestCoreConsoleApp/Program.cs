using CoreLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace TestCoreConsoleApp
{
    class Program
    {
        static Cat[] Cats = new Cat[]
        {
            new Cat { Age = 1,   Name = "Cat 1" },
            new Cat { Age = 10,  Name = "Cat 10" },
            new Cat { Age = 100, Name = "Cat 100" },
        };

        class CatFilter 
        {
            public int? Age { get; set; }
            public string Name { get; set; }
        }

        static async Task Main(string[] args)
        {
            #region SimpleExample
            //var oldCats = Cats.Where(cat => cat.Age > 50).ToList();
            //var dynamicOldCats = Cats.AsQueryable().Where("Age > 5").ToList();
            //var dynamicNamedCats = Cats.AsQueryable().Where("Name.Contains(\"10\")").ToList();


            //foreach (var cat in dynamicNamedCats)
            //{
            //    Console.WriteLine(cat);
            //}

            #endregion

            #region recursion example

            CatFilter catFilter = new CatFilter { Age = null, Name = "Cat 10" };

            var props = typeof(CatFilter).GetProperties();

            var query = Cats.AsQueryable();
            foreach (var prop in props)
            {
                var value = prop.GetValue(catFilter);
                if (value is null)
                    continue;

                query = query.Where($"{prop.Name} == \"{value}\"");
            }

            var resultColl = query.ToList();
            foreach (Cat cat in resultColl)
            {
                Console.WriteLine(cat);
            }

            #endregion
        }
    }
}
