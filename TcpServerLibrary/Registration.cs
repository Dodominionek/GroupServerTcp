using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    class Registration
    {
        public Registration() { }

        public void addUser(Dictionary<int, User> list, string log, string pass)
        {
            User user = new User(log, pass);
            User.id += 1;
            list.Add(User.id, user);
        }

        public void addNewUser(Dictionary<int, User> list, string log, string pass)
        {
            addUser(list, log, pass);
            System.IO.StreamWriter file = File.AppendText("usersCredentials.txt");
            file.WriteLine(log + ";" + pass);
            file.Close();
        }
    }
}
