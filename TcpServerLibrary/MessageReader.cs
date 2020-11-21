using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class MessageReader
    {
        public static Dictionary<string, string> credentials = new Dictionary<string, string>();
        public MessageReader()
        {
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader("Messages.conf");
            while ((line = file.ReadLine()) != null)
            {
                var cred = line.Split('=');
                credentials.Add(cred[0], cred[1]);
            }
        }

        public string getMessage(string key)
        {
            string myValue = credentials.FirstOrDefault(x => x.Key == key).Value;
            myValue = myValue + "\r\n";
            return myValue;
        }
    }
}