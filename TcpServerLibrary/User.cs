using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    class User
    {
        public static int id = 0;
        private string login;
        private string password;
        private int score;

        // U¿ytkownik ma takie atrybuty jak id (do identyfikacji w s³owniku), login, has³ow i wynik w grze
        public User(string login, string password)
        {
            this.login = login;
            this.password = password;
            this.score = 0;
        }

        public string getLogin()
        {
            return login;
        }

        public string getPassword()
        {
            return password;
        }
    }
}