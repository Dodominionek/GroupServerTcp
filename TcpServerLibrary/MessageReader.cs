using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class MessageReader
    {
        public static Dictionary<string, string> messages = new Dictionary<string, string>();
        public MessageReader()
        {
            string line;
            StreamReader file = new StreamReader("Messages.conf");

            while ((line = file.ReadLine()) != null)
            {
                var cred = line.Split('=');
                messages.Add(cred[0], cred[1]);
            }
        }

        public string getMessage(string key)
        {
            string myValue = messages.FirstOrDefault(x => x.Key == key).Value;
            myValue = myValue + "\r\n";
            return myValue;
        }
    }
}