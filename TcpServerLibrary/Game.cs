using ServerLibrary;
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
        private int tmpValue = 0;
        public int numberValue;
        private int counter = 0;

        public Game()
        {
            this.numberValue = randomInt();
        }

        private int randomInt()
        {
            Random random = new Random();
            return random.Next(100);
        }

        public static void guessingGame(NetworkStream stream, MessageReader messageReader, ClientComunicator comunicator)
        {
            int nextGame = 1;
            Game game = new Game();
            Console.WriteLine("Number to guess: " + game.numberValue);
            while (nextGame == 1)
            {
                try
                {
                    comunicator.SendMessage(stream, messageReader.getMessage("guessMessage"));
         
                    var guessedVal = comunicator.ReadResponse(stream);

                    String time = DateTime.Now.ToString("h:mm:ss");
                    Console.WriteLine(time + " -> " + guessedVal);
                    int guessedValInt;
                    try
                    {
                        guessedValInt = Int32.Parse(guessedVal);
                    }
                    catch (FormatException e)
                    {
                        guessedValInt = 102;
                    }
                    if (guessedValInt > 100 || guessedValInt < 0)
                    {
                        comunicator.SendMessage(stream, messageReader.getMessage("badValueMessage"));
                    }

                    string hotOrNotMessage = game.hotOrNot(guessedValInt);
                    comunicator.SendMessage(stream, hotOrNotMessage);

                    if (game.numberValue.Equals(guessedValInt))
                    {
                        comunicator.SendMessage(stream, messageReader.getMessage("winningMessage"));
                        Console.WriteLine("Client guessed the number");

                        comunicator.SendMessage(stream, messageReader.getMessage("continueMessage"));

                        var continueGame = comunicator.ReadResponse(stream); 

                        nextGame = Int32.Parse(continueGame);
                        if (nextGame == 0)
                        {
                            comunicator.SendMessage(stream, messageReader.getMessage("endMessage"));
                        }
                        else
                        {
                            game = new Game();
                            Console.WriteLine("Number to guess: " + game.numberValue);
                        }
                    }
                }
                catch (Exception ex)
                {
                    nextGame = 0;
                    Console.WriteLine("Zaden klient nie jest polonczony z serwerem");
                }
            }
        }

        public string hotOrNot(int guessedValue)
        {   
            string hotOrNot = "";
            if (guessedValue < numberValue + 10 && guessedValue > numberValue - 10)
            {
                 hotOrNot = " Goroco \r\n";
                 counter = 0;
            }
            else if (guessedValue < numberValue + 25 && guessedValue > numberValue - 25)
            {
                hotOrNot = " Cieplo \r\n";
                if (guessedValue > numberValue && counter == 1)
                {
                    if (guessedValue - numberValue < tmpValue - numberValue)
                    {
                        hotOrNot = " Cieplej \r\n";
                    }
                }
                else if(guessedValue < numberValue && counter == 1)
                {
                    if (guessedValue - numberValue > tmpValue - numberValue)
                    {
                        hotOrNot = " Cieplej \r\n";
                    }
                }
                    counter = 1;
            }

            else if (guessedValue < numberValue + 40 && guessedValue > numberValue - 40)
            {
                 hotOrNot = " Zimno \r\n";
                 counter = 0;
            }
            else 
            {
                 hotOrNot = " Mrozno \r\n";
                 counter = 0;
            }
            tmpValue = guessedValue;
            return hotOrNot;
        }
    }
}
