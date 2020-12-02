using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class User
    {
        public int id;
        public string login;
        public string password;
        public int permission;
        public int score;

        public static int idCount = 0;

        public User() { }

        public User(string log, string pas, int per)
        {
            this.id = idCount;
            idCount += 1;
            this.login = log;
            this.password = pas;
            this.permission = per;
            this.score = 0;
        }
    }
}
