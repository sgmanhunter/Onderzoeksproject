using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterDataInterpretor
{
    class Program
    {
        static void Main(string[] args)
        {
            Report report = new Report(new DateTime(2013, 12, 22), new DateTime(2014, 01, 5));
            Console.WriteLine(report);
            Console.ReadKey(true);
        }
    }
}
