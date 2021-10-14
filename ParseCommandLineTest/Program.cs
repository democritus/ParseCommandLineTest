using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ParseCommandLineTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /* Test embedding args within args.
             *
             * Technique:
             *   Outer args: Use double quotes as usual for values that contain spaces.
             *   Inner args: Use single quotes for values that contain spaces.
             *   
             * Example:
             *   ParseCommandLineTest.exe /DIR1="C:\Program Files" /GRAPI="/DIR2='C:\Program Files (x86)'"
             */

            List<string> outerArgs = new List<string>();
            string sInnerArgs = null;

            Console.WriteLine("All args:");
            for (int i = 0; i < args.Length; ++i)
            {
                string thisArg = args[i];

                // Show each element in Main's args array 
                Console.WriteLine($"{i}\t{thisArg}");

                // Check for the "inner" args...
                string needle = "/GRAPI=";
                int ixStart = needle.Length;
                if (thisArg.StartsWith(needle, StringComparison.OrdinalIgnoreCase))
                {
                    // ...skipping if they're an empty string...
                    if (thisArg.Length < ixStart)
                    {
                        continue;
                    }

                    // ...otherwise grabbing the arg's value..
                    sInnerArgs = thisArg.Substring(ixStart);
                }
                else
                {
                    // Re-collect all args other than the "inner args".
                    outerArgs.Add(thisArg);
                }
            }

            // Replace single quotes with double quotes before passing to external process.
            if (sInnerArgs != null)
            {
                sInnerArgs = sInnerArgs.Replace('\'', '"');
            }

            Console.WriteLine("\nOuter args:");
            Console.WriteLine(string.Join(" ", outerArgs));

            Console.WriteLine("\nInner args:");
            Console.WriteLine(sInnerArgs);

            // Pass inner args to new instance of this program to test how they are parsed.
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            string currentExePath = entryAssembly.Location;
            string workingDir = Path.GetDirectoryName(currentExePath);
            string cppExePath = Path.Combine(workingDir, "CppConsole.exe");
            string assemblyName = entryAssembly.GetName().Name;
            Process[] runningProcesses = Process.GetProcessesByName(assemblyName);
            if (sInnerArgs != null
                && runningProcesses.Length < 10) //prevent infinite recursion madness!
            {
                Process.Start(currentExePath, sInnerArgs);
                Process.Start(cppExePath, sInnerArgs);
            }
            Console.WriteLine("\nPress ENTER to exit...\n");
            do
            {
            } while (Console.ReadKey(true).Key != ConsoleKey.Enter);
        }
    }
}
