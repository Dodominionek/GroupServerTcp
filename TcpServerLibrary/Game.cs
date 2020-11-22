using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpServerLibrary
{
    class Game
    {

        public int numberValue;

        public Game()
        {
            this.numberValue = randomInt();
        }

        private int randomInt()
        {
            Random random = new Random();
            return random.Next(100);
        }

        public string hotOrNot(int guessedValue)
        {
            string hotOrNot = "";
            if (guessedValue < numberValue + 10 && guessedValue > numberValue - 10)
            {
                 hotOrNot = " Goroco \r\n";
            }
            else if (guessedValue < numberValue + 20 && guessedValue > numberValue - 20)
            {
                 hotOrNot = " Cieplo \r\n";
            }
            else if (guessedValue < numberValue + 35 && guessedValue > numberValue - 35)
            {
                 hotOrNot = " Zimno \r\n";
            }
            else 
            {
                 hotOrNot = " Mrozno \r\n";
            }

            return hotOrNot;
        }
    }
}
