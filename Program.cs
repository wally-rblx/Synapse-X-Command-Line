using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using sxlib;
using sxlib.Specialized;

namespace SynapseCommandLine
{
    class SynapseLibrary
    {
        public static AppDomain currentDomain = AppDomain.CurrentDomain;
        public static SxLibOffscreen Synapse;

        public static string directory = Directory.GetCurrentDirectory();

        public static bool attached = false;
        public static bool isReady = false;
        public static bool autoAttach = false;

        public static List<SxLibBase.SynHubEntry> Scripts;

        public static void print(string txt)
        {
            Console.WriteLine(txt);
        }

        public static void Init()
        {
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ErrHandler);
            Synapse = SxLib.InitializeOffscreen(directory);

            Synapse.LoadEvent += Loader;
            Synapse.AttachEvent += Attacher;
            Synapse.ScriptHubEvent += ScriptHub;

            Synapse.Load();
        }

        public static void Kill(string msg)
        {
            Console.WriteLine(msg);
            Thread.Sleep(5000);
            Environment.Exit(0);
        }

        public static bool FindWindow(string name)
        {
            Process[] processes = Process.GetProcesses();
            foreach (var proc in processes)
            {
                if (proc.ProcessName == name)
                {
                    return true;
                }
            }
            return false;
        }

        private static void ScriptHub(List<SxLibBase.SynHubEntry> SynScripts)
        {
            Scripts = SynScripts;
        }

        private static void Attacher(SxLibBase.SynAttachEvents Event, object bla)
        {
            switch (Event)
            {
                case SxLibBase.SynAttachEvents.READY:
                    {
                        attached = true;
                        Console.WriteLine("[S^X] Synapse X has attached, you may now start executing!");
                        break;
                    }

                case SxLibBase.SynAttachEvents.FAILED_TO_FIND:
                    {
                        attached = false;
                        Console.WriteLine("[S^X] Synapse X has failed to find Roblox!");
                        break;
                    }

                case SxLibBase.SynAttachEvents.SCANNING:
                    {
                        Console.WriteLine("[S^X] Scanning...");
                        break;
                    }

                case SxLibBase.SynAttachEvents.INJECTING:
                    {
                        Console.WriteLine("[S^X] Injecting...");
                        break;
                    }

                case SxLibBase.SynAttachEvents.CHECKING_WHITELIST:
                    {
                        Console.WriteLine("[S^X] Checking whitelist...");
                        break;
                    }

                case SxLibBase.SynAttachEvents.CHECKING:
                    {
                        Console.WriteLine("[S^X] Checking...");
                        break;
                    }

                case SxLibBase.SynAttachEvents.PROC_DELETION:
                    {
                        attached = false;
                        Console.WriteLine("[S^X] Synapse X has disconnected from Roblox!");
                        break;
                    }
                case SxLibBase.SynAttachEvents.PROC_CREATION:
                    {
                        if (autoAttach == true)
                        {
                            Console.WriteLine("[S^X] Starting auto attach process...");
                        }
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        private static void Loader(SxLibBase.SynLoadEvents Event, object yuck)
        {
            switch (Event)
            {
                case SxLibBase.SynLoadEvents.READY:
                    {
                        isReady = true;
                        Console.WriteLine("[S^X] Synapse X has loaded! Type 'attach' to attach!");
                        if (Synapse.GetOptions().AutoAttach)
                        {
                            autoAttach = true;
                            Console.WriteLine("[S^X] Auto Attach is on!");
                        }
                        Synapse.ScriptHub();
                        break;
                    }

                case SxLibBase.SynLoadEvents.NOT_UPDATED:
                    {
                        Kill("[S^X] Synapse X is not updated!");
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        public static void ErrHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;

            if (e.Message == "Access to the path is denied.")
            {
                Console.WriteLine("Error: Access to the path is denied. Please close all lingering UI processes (such as RC7 ui, Synapse ui, etc) and try again.");
            }
            else
            {
                Console.WriteLine("Something bad happened: " + e.Message + "\r\n" + e.StackTrace);
            };

            Thread.Sleep(50000);
        }

        private static string GetScriptHubName(string nm) { 
            switch(nm)
            {
                case "jbhaxx": {
                    return "JailbreakHaxx";
                }

                case "dex": {
                    return "Dark Dex";
                }

                case "esp": {
                    return "Unnamed ESP";
                }

                case "madcity": {
                    return "MadcityHaxx";
                }

                case "pfhaxx": {
                    return "PFHaxx";
                }

                case "streamsnipe": {
                    return "Stream Sniper";
                }

                case "scriptdump": {
                    return "Script Dumper";
                }

                case "remotespy": {
                    return "Remote Spy";
                }

                default:  {
                    return "bad input";
                }
            }
        }

        public static string GetScriptHubNames()
        {
            string list = "";
            foreach (var Script in Scripts)
            {
                list += Script.Name + "\n";
            }

            return list;
        }

        public static void ExecuteScriptHub(string name)
        {
            foreach(var Script in Scripts)
            {
                if (Script.Name == name)
                {
                    Console.WriteLine("[S^X] Successfully executed " + name + "!");
                    Script.Execute();
                    return;
                }
            }
        }

        public static void ExecuteFile(string file) {
            string newPath = (directory + "\\scripts\\"  + file);
            if (File.Exists(newPath))
            {
                Synapse.Execute(File.ReadAllText(newPath));
                Console.WriteLine("[S^X] Executed file: " + file);
            }
        }
    }

    class SynCommandLine
    {
        private static string helpText = @"[S^X COMMANDS]
execute <code> - executes a script. example: execute print'hi'
shub <name> - executes a script hub script. example: shub esp (would execute Unnamed ESP)
script <file> - executes a script from your scripts folder. example: script memes.lua
attach - attaches Synapse X to Roblox if it is not already attached.
scrlist - gives u all the script hub names
";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Title = "Synapse X Command Line - Created by wally";
            Console.WriteLine("[S^X] Welcome to the Synapse X Command Line!");

            if (!File.Exists("Synapse X.exe"))
            {
                Console.WriteLine("[S^X] Unable to find 'Synapse X.exe'");
                Thread.Sleep(5000);
                Environment.Exit(0);
            };

            SynapseLibrary.Init();
            while (!SynapseLibrary.isReady)
            {
                /* do not accept input until Synapse X is loaded :) */
                Thread.Sleep(100);
            }

            while (true)
            {
                string input = Console.ReadLine();
                if (SynapseLibrary.attached == true)
                {                    
                    if (input.Length >= 7 && input.Substring(0, 7) == "execute")
                    {
                        SynapseLibrary.Synapse.Execute(input.Substring(8));
                        Console.WriteLine("[S^X] Executed successfully!");
                    } else if (input.Length >= 4 && input.Substring(0, 4) == "shub") {
                        if (input.Length > 4)
                        {
                            SynapseLibrary.ExecuteScriptHub(input.Substring(5));
                        }
                    } else if (input.Length >= 6 && input.Substring(0, 6) == "script") {
                        if (input.Length > 6)
                        {
                            SynapseLibrary.ExecuteFile(input.Substring(7));
                        }
                    } else if (input.Substring(0, 4) == "help")
                    {
                        Console.WriteLine(helpText);
                    } else if (input.Substring(0, 7) == "scrlist")
                    {
                        Console.Write(SynapseLibrary.GetScriptHubNames());
                    }
                } else
                {
                    if (input == "attach")
                    {
                        Console.WriteLine("[S^X] Attaching to roblox...");
                        SynapseLibrary.Synapse.Attach();
                    }
                }

                Thread.Sleep(100);
            }
        }
    }
}
