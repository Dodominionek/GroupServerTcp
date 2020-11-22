using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class UserHandler
    {
        private string path = Directory.GetCurrentDirectory() + "\\users.txt";
        private Dictionary<string, string> credentials;
        private Dictionary<int, User> list;

        public UserHandler() {
            ReadUsersCredentials();
        }

        Registration registration = new Registration();

        public Dictionary<string, string> Credentials { get => credentials; set => credentials = value; }
        internal Dictionary<int, User> List { get => list; set => list = value; }

        // Sprawdza czy dany user jest na liście, jeśli tak to go dodaje (Whitelista)
        public void addNewUser(string log, string pass)
        {
            Credentials.Add(log, pass);
            StreamWriter file = File.AppendText("usersCredentials.txt");
            file.WriteLine(log + ";" + pass);
            file.Close();
        }

        public void ReadUsersCredentials()
        {
            string line;
            var credentials = new Dictionary<string, string>();
            StreamReader file = new StreamReader("usersCredentials.txt");
            while ((line = file.ReadLine()) != null)
            {
                var cred = line.Split(';');
                credentials.Add(cred[0], cred[1]);
            }
            file.Close();
            Credentials = credentials;
        }

        public void showUsers()
        {
            Console.WriteLine("Current users: ");
            foreach (var user in List)
            {
                Console.WriteLine("Login: " + user.Value.getLogin() + " Password: " + user.Value.getPassword());
            }
        }

        public void init()
        {
            //foreach (var user in Credentials)
            //{
            //    registration.addUser(List, user.Key, user.Value);
            //}
            showUsers();
        }


        //Usuwanie usera z listy po id lub loginie (nie kasuje z whitelisty na razie)
        public string removeUser(int id)
        {
            List.Remove(id);
            var temp = List;
            List = new Dictionary<int, User>();
            System.IO.StreamWriter file = new System.IO.StreamWriter("usersCredentials.txt");
            file.Write("");
            file.Close();
            file = File.AppendText("usersCredentials.txt");
            foreach (var user in temp)
            {
                List.Add(user.Key, user.Value);
                file.WriteLine(user.Value.getLogin() + ";" + user.Value.getPassword());
            }
            file.Close();
            return "User with id: " + id + " removed";
        }

        public string removeUser(string log)
        {
            int key = -1;
            foreach (KeyValuePair<int, User> entry in List)
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
                List.Remove(key);
                var temp = List;
                List = new Dictionary<int, User>();
                StreamWriter file = new StreamWriter("usersCredentials.txt");
                file.Write("");
                file.Close();
                file = File.AppendText("usersCredentials.txt");
                foreach (var user in temp)
                {
                    List.Add(user.Key, user.Value);
                    file.WriteLine(user.Value.getLogin() + ";" + user.Value.getPassword());
                }
                file.Close();
                return "User: " + log + " removed";
            }
        }

        public bool login(string log, string pass)
        {
            foreach (KeyValuePair<int, User> entry in List)
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


    }
}
