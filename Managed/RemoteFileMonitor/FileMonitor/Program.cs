﻿// RemoteFileMonitor (File: FileMonitor\Program.cs)
//
// Copyright (c) 2017 Justin Stenning
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// Please visit https://easyhook.github.io for more information
// about the project, latest updates and other tutorials.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            Int32 targetPID = 0;
            string targetExe = null;

            // Will contain the name of the IPC server channel
            string channelName = null;

            // Process command line arguments or print instructions and retrieve argument value
            //ProcessArgs(args, out targetPID, out targetExe);

            //if (targetPID <= 0 && string.IsNullOrEmpty(targetExe))
            //    return;

            // Create the IPC server using the FileMonitorIPC.ServiceInterface class as a singleton
            EasyHook.RemoteHooking.IpcCreateServer<FileMonitorHook.ServerInterface>(ref channelName, System.Runtime.Remoting.WellKnownObjectMode.Singleton);

            // Get the full path to the assembly we want to inject into the target process
            string injectionLibrary = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "FileMonitorHook.dll");
            //string ar = "\"-Xbootclasspath/a:C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\\Core\" \"-Dcom.fortify.InstallRoot=C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\" -jar \"C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\\Core\\private-bin\\awb\\eclipse\\awb-startup.jar\" -os win32 -ws win32 -arch x86 -launcher \"C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\\Core\\private-bin\\awb\\eclipse\\launch2.exe\" -name Launch2 -showsplash 600 -exitdata 22d0_1c8 -application com.fortify.awb.Main -vm \"C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\\jre\\bin\\javaw.exe\" -vmargs -Xmx500M -XX:MaxPermSize=96m \"-Xbootclasspath/a:C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\\Core\" \"-Dcom.fortify.InstallRoot=C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\" -jar awb-startup.jar";
            string ar = "";

            try
            {
                //string ar = "\"C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\\Core\\private-bin\\awb\\eclipse\\launch2.exe\" -name Launch2 -showsplash 600 \"c:\\Program Files(x86)\\Fortify Software\\Fortify SCA 5.1\\Core\\private-bin\\awb\\eclipse\\plugins\\org.eclipse.platform_3.2.0.v20060601\\splash.bmp\"";


                Process p = new Process();
                p.StartInfo.FileName = @"C:\Users\sfx\AppData\Local\Google\Chrome\Application\chrome.exe";
                p.StartInfo.Arguments = ar;
                p.StartInfo.UseShellExecute = true;
                p.Start();
                targetPID = p.Id;
                //Thread.Sleep(500);
                
                // Injecting into existing process by Id
                if (targetPID > 0)
                {
                    Console.WriteLine("Attempting to inject into process {0}", targetPID);

                    // inject into existing process
                    EasyHook.RemoteHooking.Inject(
                        targetPID,          // ID of process to inject into
                        injectionLibrary,   // 32-bit library to inject (if target is 32-bit)
                        injectionLibrary,   // 64-bit library to inject (if target is 64-bit)
                        channelName         // the parameters to pass into injected library
                                            // ...
                    );
                }
                // Create a new process and then inject into it
                else if (!string.IsNullOrEmpty(targetExe))
                {
                    Console.WriteLine("Attempting to create and inject into {0}", targetExe);
                    // start and inject into a new process

                    //string ar = "\" - Xbootclasspath / a:C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\\Core\" \" - Dcom.fortify.InstallRoot = C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\" -jar \"C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\\Core\\private-bin\\awb\\eclipse\awb-startup.jar\" -os win32 -ws win32 -arch x86 -launcher \"C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\\Core\\private-bin\\awb\\eclipse\\launch2.exe\" -name Launch2 -showsplash 600 -exitdata 22d0_1c8 -application com.fortify.awb.Main -vm \"C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\\jre\\bin\\javaw.exe\" -vmargs -Xmx500M -XX:MaxPermSize=96m \"-Xbootclasspath/a:C:\\Program Files(x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\\Core\" \"-Dcom.fortify.InstallRoot=C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\" -jar awb-startup.jar";
                    //string ar = "\"-Xbootclasspath/a:C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\\Core\" \"-Dcom.fortify.InstallRoot=C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\" - jar awb - startup.jar - os win32 - ws win32 - arch x86 - launcher launch2.exe - name Launch2 - showsplash 600 - exitdata 22d0_1c8 - application com.fortify.awb.Main - vm \"C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\\jre\\bin\\javaw.exe\" - vmargs - Xmx500M - XX:MaxPermSize = 96m \"-Xbootclasspath/a:C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\\Core\\\" \"-Dcom.fortify.InstallRoot=C:\\Program Files (x86)\\Fortify Software\\Fortify SCA 5.1\\bin\\..\" - jar awb - startup.jar";
                    EasyHook.RemoteHooking.CreateAndInject(
                        targetExe,          // executable to run
                        ar,                 // command line arguments for target
                        0,                  // additional process creation flags to pass to CreateProcess
                        EasyHook.InjectionOptions.DoNotRequireStrongName, // allow injectionLibrary to be unsigned
                        injectionLibrary,   // 32-bit library to inject (if target is 32-bit)
                        injectionLibrary,   // 64-bit library to inject (if target is 64-bit)
                        out targetPID,      // retrieve the newly created process ID
                        channelName         // the parameters to pass into injected library
                                            // ...
                    );
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("There was an error while injecting into target:");
                Console.ResetColor();
                Console.WriteLine(e.ToString());
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("<Press any key to exit>");
            Console.ResetColor();
            Console.ReadKey();
        }

        static void ProcessArgs(string[] args, out int targetPID, out string targetExe)
        {
            targetPID = 0;
            targetExe = null;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(@"______ _ _     ___  ___            _ _             
|  ___(_) |    |  \/  |           (_) |            
| |_   _| | ___| .  . | ___  _ __  _| |_ ___  _ __ 
|  _| | | |/ _ \ |\/| |/ _ \| '_ \| | __/ _ \| '__|
| |   | | |  __/ |  | | (_) | | | | | || (_) | |   
\_|   |_|_|\___\_|  |_/\___/|_| |_|_|\__\___/|_|");

            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("    monitor file operations in the target process");
            Console.WriteLine();
            Console.ResetColor();
            

            // Load any parameters
            while ((args.Length != 1) || !Int32.TryParse(args[0], out targetPID) || !File.Exists(args[0]))
            {
                if (targetPID > 0)
                {
                    break;
                }
                if (args.Length != 1 || !File.Exists(args[0]))
                {
                    Console.WriteLine("Usage: FileMonitor ProcessID");
                    Console.WriteLine("   or: FileMonitor PathToExecutable");
                    Console.WriteLine("");
                    Console.WriteLine("e.g. : FileMonitor 1234");
                    Console.WriteLine("          to monitor an existing process with PID 1234");
                    Console.WriteLine(@"  or : FileMonitor ""C:\Windows\Notepad.exe""");
                    Console.WriteLine("          create new notepad.exe process using RemoteHooking.CreateAndInject");
                    Console.WriteLine();
                    Console.WriteLine("Enter a process Id or path to executable");
                    Console.Write("> ");

                    args = new string[] { Console.ReadLine() };

                    Console.WriteLine();

                    if (String.IsNullOrEmpty(args[0])) return;
                }
                else
                {
                    targetExe = args[0];
                    break;
                }
            }
        }
    }
}
