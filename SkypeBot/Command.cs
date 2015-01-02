using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using SKYPE4COMLib;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Threading;

namespace SkypeBot
{
    class CommandParser
    {
        private Dictionary<string, Command> lookup;
        public CommandParser()
        {
            lookup = new Dictionary<string, Command>();
            Command random = new RandomCommand();
            lookup.Add(random.ID, random);
        }

        public void HandleCommand(string command, IChat target)
        {
            var temp = new UserCommand(command);
            try
            {
                lookup[temp.ID].Execute(target, temp);
            }
            catch (Exception)
            {
                //Lookup failed, don't handle.
            }
            
        }

        public bool GetCommandFile(string path)
        {
            if (!File.Exists(path))
                return false;
            XmlSerializer serializer = new XmlSerializer(typeof(CommandList));

            FileStream fs = new FileStream(path, FileMode.Open);
            CommandList parsedFile;
            try
            {
                parsedFile = (CommandList)serializer.Deserialize(fs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            foreach (var command in parsedFile.Items)
            {
                Command temp = new Command(command);
                lookup.Add(temp.ID, temp);
            }
            return true;
        }

        public void DumpCommands()
        {
            foreach (var cmd in lookup.Values)
            {
                Console.WriteLine(String.Format("{0}, {1}",cmd.ID, cmd.Message));
            }
        }

        public static bool IsCommand(string command)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class CommandBase : IEqualityComparer<CommandBase>
    {
        public string ID { protected set; get; }

        protected CommandBase(string command)
        {
            ID = command.Substring(0, command.IndexOf('{'));
        }
        protected CommandBase()
        {

        }

        protected void SetID(string id)
        {
            ID = id;
        }

        public bool Equals(CommandBase x, CommandBase y)
        {
            return x.ID.Equals(y.ID);
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public int GetHashCode(CommandBase obj)
        {
            int hash = 1;
            var id = obj.ID.ToLower();
            for (int i = 0; i < 3 && i < obj.ID.Length; i++)
            {
                int tmp = (obj.ID[i] - 97) % primes.Length;
                hash *= primes[tmp];
            }
            return hash;
        }

        protected static int[] primes = primes = new int[]
	    {
	        3, 7, 11, 17, 23, 29, 37,
	        47, 59, 71, 89, 107, 131,
	        163, 197, 239, 293, 353,
	        431, 521, 631, 761, 919,
	        1103
	    };
    }
    public class Command : CommandBase
    {
        private int timer = 0;

        private string filePath = null;

        protected CommandType type;
        
        public string Message { private set; get; }


        public Command(CommandListCommand command)
        {
            // TODO: Complete member initialization
            this.SetID(command.trigger);
            this.timer = command.timer ?? 0;
            this.Message = command.message;
            this.filePath = command.file;
            SetType();
        }
        protected Command()
        {

        }
        private void SetType()
        {
            if (!String.IsNullOrWhiteSpace(Message) && !String.IsNullOrWhiteSpace(filePath))
                type = CommandType.Mixed;
            else if (!String.IsNullOrWhiteSpace(Message))
                type = CommandType.Message;
            else if (!String.IsNullOrWhiteSpace(filePath))
                type = CommandType.File;
            else
                type = CommandType.None;
        }

        public virtual void Execute(IChat place, UserCommand cmd)
        {
            if (type == CommandType.Mixed || type == CommandType.Message)
            {
                place.SendMessage(Message);
            }
            if (File.Exists(filePath) && (type == CommandType.Mixed || type == CommandType.File))
            {
                Task t = new Task( () => 
                {
                    foreach (var line in File.ReadAllLines(filePath))
                    {
                        place.SendMessage(line);
                        Thread.Sleep(timer * 1000);
                    }
                });
                t.Start();
            }
        }
        protected enum CommandType{
            File,
            Message,
            Mixed,
            None
        }
        
    }
    public class RandomCommand : Command
    {
        public RandomCommand()
        {
            ID = Strings.RandomCommandID;
        }

        public override void Execute(IChat place, UserCommand cmd)
        {
            if (cmd.ArgList.Count != 2)
                return;
            try
            {
                int min = Int32.Parse(cmd.ArgList[0]);
                int max = Int32.Parse(cmd.ArgList[1]);
                Random rand = new Random();
               place.SendMessage("Randomed: " + rand.Next(min, max));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
    public class UserCommand : CommandBase
    {
        public List<string> ArgList { get; private set; }

        public UserCommand(string command)
        {
            ID = command.Split(' ')[0].Substring(1);
            ArgList = command.Split(' ').ToList();
            ArgList.RemoveAt(0); //Remove the !Command part
        }


        
    }
}
