using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    class UserHandler
    {
        public UserHandler() { }

        Registration registration = new Registration();

        private string path = Directory.GetCurrentDirectory() + "\\users.txt";

        // Sprawdza czy dany user jest na liście, jeśli tak to go dodaje (Whitelista)
        

        public Dictionary<string, string> ReadUsersCredentials()
        {
            string line;
            var credentials = new Dictionary<string, string>();
            System.IO.StreamReader file = new System.IO.StreamReader("usersCredentials.txt");
            while ((line = file.ReadLine()) != null)
            {
                var cred = line.Split(';');
                credentials.Add(cred[0], cred[1]);
            }
            file.Close();
            return credentials;
        }

        public void init(Dictionary<int, User> list)
        {
            foreach (var user in ReadUsersCredentials())
            {
                registration.addUser(list, user.Key, user.Value);
            }
            showUsers(list);
        }

        //Usuwanie usera z listy po id lub loginie (nie kasuje z whitelisty na razie)
        public string removeUser(Dictionary<int, User> list, int id)
        {
            list.Remove(id);
            var temp = list;
            list = new Dictionary<int, User>();
            System.IO.StreamWriter file = new System.IO.StreamWriter("usersCredentials.txt");
            file.Write("");
            file.Close();
            file = File.AppendText("usersCredentials.txt");
            foreach (var user in temp)
            {
                list.Add(user.Key, user.Value);
                file.WriteLine(user.Value.getLogin() + ";" + user.Value.getPassword());
            }
            file.Close();
            return "User with id: " + id + " removed";
        }

        public string removeUser(Dictionary<int, User> list, string log)
        {
            int key = -1;
            foreach (KeyValuePair<int, User> entry in list)
            {
                if (entry.Value.getLogin() == log)
                {
                    key = entry.Key;
                }
            }
            if (key == -1)
            {
                return "There is no user with login: " + log;
            }
            else
            {
                list.Remove(key);
                var temp = list;
                list = new Dictionary<int, User>();
                System.IO.StreamWriter file = new System.IO.StreamWriter("usersCredentials.txt");
                file.Write("");
                file.Close();
                file = File.AppendText("usersCredentials.txt");
                foreach (var user in temp)
                {
                    list.Add(user.Key, user.Value);
                    file.WriteLine(user.Value.getLogin() + ";" + user.Value.getPassword());
                }
                file.Close();
                return "User: " + log + " removed";
            }
        }

        public bool login(Dictionary<int, User> list, string log, string pass)
        {
            foreach (KeyValuePair<int, User> entry in list)
            {
                if (entry.Value.getLogin() == log)
                {
                    if (entry.Value.getPassword() == pass)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void showUsers(Dictionary<int, User> list)
        {
            Console.WriteLine("Current users: ");
            foreach (var user in list)
            {
                Console.WriteLine("Login: " + user.Value.getLogin() + " Password: " + user.Value.getPassword());
            }
        }
    }
}
