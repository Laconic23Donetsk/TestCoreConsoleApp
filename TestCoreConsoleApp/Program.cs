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
            Range<DateTime> range = new Range<DateTime>();
            Validator validator = new Validator();
            validator.IsValid(range);
        }
    }

    public record Range<T>
        where T : IComparable<T>
    {
        public Range(T? from, T? to)
        {
            From = from;
            To = to;
        }

        public Range()
        { }

        public T? From { get; set; }
        public T? To { get; set; }
    }

    public class Validator
    {
        public bool IsValid(Range<DateTime> dtRange)
        {
            if ((DateTime?)dtRange.From is null || (DateTime?)dtRange.To is null)
                return true;

            return dtRange.From > dtRange.To;
        }
    }
}
