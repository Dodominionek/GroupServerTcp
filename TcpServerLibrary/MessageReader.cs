using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class MessageReader
    {
        public MessageReader()
        {

        }

        public static Dictionary<string, string> GetMessages()
        {
            string line;
            var credentials = new Dictionary<string, string>();
            System.IO.StreamReader file = new System.IO.StreamReader("Messages.conf");
            while ((line = file.ReadLine()) != null)
            {
                var cred = line.Split('=');
                credentials.Add(cred[0], cred[1]);
            }
            return credentials;
        }
    }
}