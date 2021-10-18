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

            Console.WriteLine(".NET app test\n");
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

            Console.WriteLine("\nOuter args:");
            Console.WriteLine(string.Join(" ", outerArgs));

            Console.WriteLine("\nInner args:");
            Console.WriteLine(sInnerArgs);

            // Replace single quotes with double quotes before passing to external process.
            if (sInnerArgs != null)
            {
                sInnerArgs = sInnerArgs.Replace('\'', '"');
            }

            Console.WriteLine("\nInner args sanitized:");
            Console.WriteLine(sInnerArgs);

            // Pass the inner args to a .NET and C++ program to test how they are handled.
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            string dotNetAssemblyName = entryAssembly.GetName().Name;
            string cppAssemblyName = "CppConsole";
            string dotNetExePath = entryAssembly.Location;
            string workingDir = Path.GetDirectoryName(dotNetExePath);
            string cppExePath = Path.Combine(workingDir, $"{cppAssemblyName}.exe");
            Process[] runningProcesses = Process.GetProcessesByName(dotNetAssemblyName);
            if (sInnerArgs != null
                && runningProcesses.Length < 10) //prevent infinite recursion madness!
            {
                Process.Start(dotNetExePath, sInnerArgs); //.NET program
                Process.Start(cppExePath, sInnerArgs); // C++ program
            }
            Console.WriteLine("\nPress ENTER to exit...\n");
            do
            {
            } while (Console.ReadKey(true).Key != ConsoleKey.Enter);

            // Close process related to this test.
            List<Process> processesToClose = new List<Process>();
            foreach (Process p in Process.GetProcessesByName(dotNetAssemblyName))
            {
                if (p.Id != Process.GetCurrentProcess().Id)
                {
                    processesToClose.Add(p);
                }
            }
            processesToClose.AddRange(Process.GetProcessesByName(cppAssemblyName));
            try
            {
                processesToClose.ForEach(x => x.CloseMainWindow());
            }
            catch
            {
            }
        }
    }
}