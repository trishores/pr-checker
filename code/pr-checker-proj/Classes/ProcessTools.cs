/*
 * Copyright (C) 2021 Tris Shores
 * Open source software. Licensed under the MIT license: https://opensource.org/licenses/MIT
*/

using System;
using System.Diagnostics;
using System.Text;

namespace PrChecker
{
    internal static class ProcessTools
    {
        internal static (int exitCode, string outData, string errData) RunProcess(string filename, string args, string workingDir = null, bool waitForExit = true, int timeoutMs = -1)
        {
            StringBuilder sbStd = new();
            StringBuilder sbError = new();

            using var proc = new Process();
            proc.OutputDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                sbStd.AppendLine(e.Data + Environment.NewLine);
            });
            proc.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                sbError.AppendLine(e.Data + Environment.NewLine);
            });

            proc.EnableRaisingEvents = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;

            proc.StartInfo.FileName = filename;
            proc.StartInfo.Arguments = args;
            proc.StartInfo.WorkingDirectory = workingDir;
            //proc.StartInfo.Verb = "runas";
            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            if (waitForExit)
            {
                try
                {
                    proc.WaitForExit(milliseconds: timeoutMs);
                    return (proc.ExitCode, sbStd.ToString(), sbError.ToString());
                }
                catch
                {
                    return (-1, sbStd.ToString(), sbError.ToString());
                }
            }
            return (0, null, null);
        }

        internal static (int res, string stdout, string stderr) RunProcessOutput(string filename, string args, string workingDir = null, bool waitForExit = true, bool hideConsole = true)
        {
            ConsoleWriteLine($"\r\nStarting {filename} {args}\r\n", ConsoleColor.Green);

            var sbOut = new StringBuilder();
            var sbErr = new StringBuilder();

            using (var proc = new Process())
            {
                proc.OutputDataReceived += new DataReceivedEventHandler((s, e) =>
                {
                    ConsoleWriteLine(e.Data + Environment.NewLine);
                    sbOut.Append(e.Data);
                });
                proc.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
                {
                    ConsoleWriteLine(e.Data + Environment.NewLine);
                    sbErr.Append(e.Data);
                });
                if (hideConsole)
                {
                    proc.EnableRaisingEvents = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                }
                proc.StartInfo.FileName = filename;
                proc.StartInfo.Arguments = args;
                proc.StartInfo.WorkingDirectory = workingDir;
                //proc.StartInfo.Verb = "runas";
                proc.Start();
                if (hideConsole) proc.BeginOutputReadLine();
                if (hideConsole) proc.BeginErrorReadLine();
                if (waitForExit)
                {
                    proc.WaitForExit();
                    return (proc.ExitCode, sbOut.ToString(), sbErr.ToString());
                }
                return (0, null, null);
            }
        }

        internal static int RunDotNet(string args, string workingDir = null)
        {
            ConsoleWriteLine($"\r\nStarting dotnet {args}\r\n", ConsoleColor.Green);

            // Initialize new process:
            using (var proc = new Process())
            {
                //var sbOut = new StringBuilder();
                //var sbErr = new StringBuilder();

                proc.OutputDataReceived += new DataReceivedEventHandler((s, e) => 
                {
                    ConsoleWriteLine(e.Data + Environment.NewLine);
                    //sbOut.AppendLine(e.Data);
                });
                proc.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
                {
                    ConsoleWriteLine(e.Data + Environment.NewLine);
                    //sbErr.AppendLine(e.Data);
                });
                proc.EnableRaisingEvents = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.FileName = @"C:\Program Files\dotnet\dotnet.exe";
                proc.StartInfo.Arguments = args + (args.StartsWith("publish") || args.StartsWith("build") ? " -v quiet" : "");
                if (workingDir != null) proc.StartInfo.WorkingDirectory = workingDir;
                proc.Start();
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
                proc.WaitForExit();
                if (proc.ExitCode != 0)
                {
                    //File.WriteAllText(@"C:\Users\someone\Desktop\dotnet_publish_out.txt", sbOut.ToString());
                    //File.WriteAllText(@"C:\Users\someone\Desktop\dotnet_publish_err.txt", sbErr.ToString());
                }
                return proc.ExitCode;
            }
        }

        internal static void ConsoleWriteLine(string str, ConsoleColor consoleColor = ConsoleColor.White, bool onlyColorLines = true)
        {
            if (str.ToLower().Contains("error") || str.ToLower().Contains("failed")) consoleColor = ConsoleColor.Red;
            else if (str.ToLower().Contains("warning")) consoleColor = ConsoleColor.Yellow;
            else if (consoleColor == ConsoleColor.White)
            {
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(str);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
