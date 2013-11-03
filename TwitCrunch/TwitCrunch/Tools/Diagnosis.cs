using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TwitCrunch.Tools
{
    public class Diagnosis
    {
        private static Process _currentProcess = System.Diagnostics.Process.GetCurrentProcess();

        public static string GetVersionNumber()
        {
            return Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        public static string GetMemoryUsage(double memory=0, byte format=0)
        {
            if(memory == 0)memory = _currentProcess.WorkingSet64;
            if (memory > 999)
            {
                memory = memory / 1024;
                format++;
                return GetMemoryUsage(memory, format);
            }
            string mem = string.Empty;
            switch (format)
            {
                case 1:
                    mem = string.Format("{0}Kb", Math.Round(memory,2));
                    break;
                case 2:
                    mem = string.Format("{0}Mb",  Math.Round(memory,2));
                    break;
                case 3:
                    mem = string.Format("{0}Gb",  Math.Round(memory,2));
                    break;
            }
            return mem;
        }

        public static string GetThreadCount()
        {
            return _currentProcess.Threads.Count.ToString();
        }
    }
}
