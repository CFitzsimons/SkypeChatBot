using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SKYPE4COMLib;

namespace SkypeBot
{
    class SkypeChats
    {
        List<ChatWrapper> activeChats;
        Skype skype;

        public SkypeChats()
        {
            skype = new Skype();
            activeChats = new List<ChatWrapper>();
            RefreshChats();
        }

        public void RefreshChats()
        {
            foreach (Chat chat in skype.Chats)
            {
                if (chat.Topic.Contains(Strings.TopicIdent))
                {
                    ChatWrapper wrap = new ChatWrapper(skype, chat);
                    if (!activeChats.Contains(wrap))
                        activeChats.Add(wrap);
                }
            }
        }

        public Dictionary<String, ChatWrapper> GetAllCommandMessages()
        {
            Dictionary<String, ChatWrapper> list = new Dictionary<string, ChatWrapper>();
            foreach (var wrapper in activeChats)
            {
               list.Add(wrapper.GetRecentCommandMessage(), wrapper);
            }
            return list;
        }
    }

    class ChatWrapper : IEquatable<ChatWrapper>
    {
        public String ChatName { private set; get; }
        public Chat Chat { private set; get; }
        public DateTime Timestamp { private set; get; }
        private Skype SkypeClient;
        
        public ChatWrapper(Skype mainObj, Chat chat)
        {
            SkypeClient = mainObj;
            ChatName = chat.Name;
            
            UpdateChat();
            foreach (IChatMessage message in Chat.Messages)
            {
                Timestamp = message.Timestamp;
                break;
            }
        }
        private void UpdateChat() 
        {
            //For some reason creating a new Skype object has a tendency to throw a COM exception.
            //TODO: Figure out why?
            try
            {
                SkypeClient = new Skype();
                Chat = SkypeClient.get_Chat(ChatName);
            }
            catch (Exception e)
            {
                //Log or something.
            }
        }
        public string[] GetRecentMessages()
        {
            UpdateChat();
            List<string> messages = new List<string>();
            DateTime temp = Timestamp;
            
            foreach (IChatMessage message in Chat.Messages)
            {
                if (temp < message.Timestamp)
                    temp = message.Timestamp;

                if (Timestamp < message.Timestamp)
                    messages.Add(message.Body);
                else
                    break;
            }
            this.Timestamp = temp;
            return messages.ToArray();
        }
        public string GetRecentCommandMessage()
        {
            foreach (var message in GetRecentMessages())
            {
                if (message.StartsWith("!"))
                    return message;
            }
            return String.Empty;
        }

        public bool Equals(ChatWrapper other)
        {
            return this.Chat.Topic.Equals(other.Chat.Topic);
        }

    }
}
