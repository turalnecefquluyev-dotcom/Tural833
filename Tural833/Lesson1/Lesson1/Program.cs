using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SystemProgrammingTask
{
    internal class Program
    {
        static bool isRunning = true;

        static void ShowProcesses()
        {
            while (isRunning)
            {
                Console.Clear();

                Process[] processes = Process.GetProcesses()
                    .OrderByDescending(p =>
                    {
                        try
                        {
                            return p.StartTime;
                        }
                        catch
                        {
                            return DateTime.MinValue;
                        }
                    })
                    .Take(20)
                    .ToArray();

                Console.WriteLine("Son yaranmis 20 Process:");
                Console.WriteLine("----------------------------");

                foreach (var process in processes)
                {
                    try
                    {
                        Console.WriteLine($"{process.Id} - {process.ProcessName}");
                    }
                    catch
                    {
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Komanda daxil edin:");
                Console.WriteLine("start chrome.exe");
                Console.WriteLine("kill chrome.exe");

                Thread.Sleep(500);
            }
        }

        static void CommandHandler()
        {
            while (isRunning)
            {
                string command = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(command))
                    continue;

                string[] parts = command.Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries
                );

                if (parts.Length != 2)
                    continue;

                string action = parts[0].ToLower();
                string processName = parts[1];

                if (action == "start")
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = processName,
                            UseShellExecute = true
                        });
                    }
                    catch
                    {
                        Console.WriteLine("Process baslatmaq mumkun olmadi.");
                    }
                }
                else if (action == "kill")
                {
                    string name = processName.EndsWith(".exe")
                        ? processName[..^4]
                        : processName;

                    Process[] processes = Process.GetProcessesByName(name);

                    foreach (var process in processes)
                    {
                        try
                        {
                            process.Kill();
                            process.WaitForExit();
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            Thread thread1 = new Thread(ShowProcesses);
            Thread thread2 = new Thread(CommandHandler);

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();
        }
    }
}