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
        private Dictionary<string, string> credentials;

        public UserHandler() {
            ReadUsersCredentials();
        }

        public Dictionary<string, string> Credentials { get => credentials; set => credentials = value; }

        // Sprawdza czy dany user jest na liście, jeśli tak to go dodaje (Whitelista)
        public void AddNewUser(string login, string password)
        {
            Credentials.Add(login, password);
            StreamWriter file = File.AppendText("usersCredentials");
            file.WriteLine("\r\n" + login + ";" + password);
            file.Close();
        }

        public void ReadUsersCredentials()
        {
            string line;
            var credentials = new Dictionary<string, string>();
            StreamReader file = new StreamReader("usersCredentials");
            while ((line = file.ReadLine()) != null)
            {
                var cred = line.Split(';');
                credentials.Add(cred[0], cred[1]);
            }
            file.Close();
            Credentials = credentials;
        }

        // Wyświetlenie na konsoli wszystkich użytkowników 
        public void ShowUsers()
        {
            Console.WriteLine("Current users: ");
            foreach (var user in Credentials)
            {
                Console.WriteLine("Login: " + user.Key + " Password: " + user.Value);
            }
        }

        //Usuwanie usera z listy 
        public void RemoveUser(string login)
        {
            using (StreamReader sr = File.OpenText("usersCredentials"))
            {
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
            foreach (var user in Credentials)
            {
                if (user.Key == login)
                {
                    if (user.Value == password)
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
