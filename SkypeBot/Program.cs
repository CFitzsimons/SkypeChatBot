using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SkypeBot
{
    class Program
    {
        static void Main(string[] args)
        {
            //Just a sample program that will grab 
            SkypeChats sc = new SkypeChats();
            CommandParser cp = new CommandParser();
            if (!cp.GetCommandFile(Path.Combine(Environment.CurrentDirectory, "Command.xml")))
            {
                Console.WriteLine("Failed to parse the file.");
                Console.ReadKey();
                Environment.Exit(1);
            }
            Console.WriteLine("Dumping all active commands.");
            cp.DumpCommands();
            Console.WriteLine("Dump complete");
            while (true)
            {
                foreach (var test in sc.GetAllCommandMessages())
                {
                    if (!String.IsNullOrEmpty(test.Key))
                        cp.HandleCommand(test.Key, test.Value.Chat);
                }
            }
        }
    }
}
