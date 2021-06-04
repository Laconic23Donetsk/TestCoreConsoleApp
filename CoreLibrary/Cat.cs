using System;

namespace CoreLibrary
{
    public class Cat
    {
        public int Age { get; set; }
        public string Name { get; set; }


        public override string ToString()
        {
            return $"{this.Age, -10}   {this.Name}";
        }
    }
}
