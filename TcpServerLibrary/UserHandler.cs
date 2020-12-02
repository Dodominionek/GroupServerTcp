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
        private readonly string path = "userCredentials";
        private Dictionary<int, User> userList = new Dictionary<int, User>();

        public UserHandler() {
            ReadUsersCredentials();
        }

        public Dictionary<int, User> UserList { get => userList; set => userList = value; }

        // Sprawdza czy dany user jest na liście, jeśli tak to go dodaje (Whitelista)
        public void AddNewUser(string login, string password, int permission)
        {
            User user = new User(login, password, permission);
            userList.Add(user.id, user);
            StreamWriter file = File.AppendText("usersCredentials");
            file.WriteLine("\r\n" + user.login + ";" + user.password + ";" + user.permission.ToString());
            file.Close();
        }

        public void ReadUsersCredentials()
        {
            string line;
            var userList = new Dictionary<int, User>();
            StreamReader file = new StreamReader("usersCredentials");
            while ((line = file.ReadLine()) != null)
            {
                var cred = line.Split(';');
                User user = new User(cred[0], cred[1], Int32.Parse(cred[2]));
                userList.Add(user.id, user);
            }
            file.Close();
            UserList = userList;
        }

        // Wyświetlenie na konsoli wszystkich użytkowników 
        public void ShowUsers()
        {
            Console.WriteLine("Current users: ");
            foreach (var user in UserList)
            {
                Console.WriteLine("Login: " + user.Value.login + " Password: " + user.Value.password + " Permissions: " + user.Value.permission.ToString());
            }
        }

        //Usuwanie usera z listy 
        public void RemoveUser(string login)
        {
            using (StreamReader sr = File.OpenText("usersCredentials"))
            {
                foreach(var us in UserList)
                {
                    if(us.Value.login == login)
                    {
                        userList.Remove(us.Key);
                    }
                }
                string s;
                List<string> linesToKeep = new List<string> { };
                var tempFile = Path.GetTempFileName();
                while ((s = sr.ReadLine()) != null)
                {
                    string[] credentials_ = s.Split(';');
                    if (credentials_[0] == login)
                    {
                        linesToKeep.Add(s);
                    }
                }
                File.WriteAllLines("usersCredentials", linesToKeep);
            }
        }

        public bool Login(string login, string password)
        {
            foreach (var user in userList)
            {
                if (user.Value.login == login)
                {
                    if (user.Value.password == password)
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }
            return false;
        }
    }
}
