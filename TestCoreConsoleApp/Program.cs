using Quartz;
using Quartz.Impl;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestCoreConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Frog aFrog = new Frog();
            Frog bFrog = new Frog();
            Animal aAnimal = aFrog;
            Animal bAnimal = bFrog;
            // not necessarily equal...
            bool areEqualFrogs = aFrog.Equals(bFrog);
            bool areEqualAnimals = aAnimal.Equals(bAnimal);
            Console.WriteLine(areEqualFrogs);
            Console.WriteLine(areEqualAnimals);

            Console.WriteLine(aFrog == bFrog);
            Console.WriteLine(aAnimal == bAnimal);
            Console.WriteLine(aFrog == bAnimal);
            //string str1 = null;
            //string str2 = null;

            //Console.WriteLine(Object.ReferenceEquals(str1, str2));
        }

      
    }

    internal class Frog : Animal
    {
        public override bool Equals(object obj)
        {
            Console.WriteLine("Equals works");
            return true;
        }

        public static bool operator ==(Frog a, Frog b)
        {
            Console.WriteLine("== works");
            return true;
        }

        public static bool operator !=(Frog a, Frog b)
        {
            Console.WriteLine("!= works");
            return true;
        }

    }

    internal class Animal
    {
        public static bool operator ==(Animal a, Animal b)
        {
            Console.WriteLine("== works");
            return false;
        }

        public static bool operator !=(Animal a, Animal b)
        {
            Console.WriteLine("!= works");
            return false;
        }
    }
}
