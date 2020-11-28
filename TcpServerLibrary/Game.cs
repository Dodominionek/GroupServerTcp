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

        public static void guessingGame(NetworkStream stream, MessageReader messageReader)
        {
            int nextGame = 1;
            Game game = new Game();
            byte[] buffer = new byte[256];
            Console.WriteLine("Number to guess: " + game.numberValue);
            while (nextGame == 1)
            {
                try
                {
                    byte[] guessMessageByte = new ASCIIEncoding().GetBytes(messageReader.getMessage("guessMessage"));
                    stream.Write(guessMessageByte, 0, guessMessageByte.Length);

                    int responseLength = stream.Read(buffer, 0, buffer.Length);
                    if (Encoding.UTF8.GetString(buffer, 0, responseLength) == "\r\n")
                    {
                        responseLength = stream.Read(buffer, 0, buffer.Length);
                    }
                    var guessedVal = Encoding.UTF8.GetString(buffer, 0, responseLength);

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
                        byte[] badValueMessageByte = new ASCIIEncoding().GetBytes(messageReader.getMessage("badValueMessage"));
                        stream.Write(badValueMessageByte, 0, badValueMessageByte.Length);
                    }

                    string hotOrNotMessage = game.hotOrNot(guessedValInt);
                    byte[] hotOrNot = new ASCIIEncoding().GetBytes(hotOrNotMessage);
                    stream.Write(hotOrNot, 0, hotOrNot.Length);

                    if (game.numberValue.Equals(guessedValInt))
                    {
                        byte[] winningMessageByte = new ASCIIEncoding().GetBytes(messageReader.getMessage("winningMessage"));
                        stream.Write(winningMessageByte, 0, winningMessageByte.Length);
                        Console.WriteLine("Client guessed the number");

                        byte[] continueMessageByte = new ASCIIEncoding().GetBytes(messageReader.getMessage("continueMessage"));
                        stream.Write(continueMessageByte, 0, continueMessageByte.Length);

                        buffer = new byte[256];
                        responseLength = stream.Read(buffer, 0, buffer.Length);
                        if (Encoding.UTF8.GetString(buffer, 0, responseLength) == "\r\n")
                        {
                            responseLength = stream.Read(buffer, 0, buffer.Length);
                        }
                        var continueGame = Encoding.UTF8.GetString(buffer, 0, responseLength);

                        nextGame = Int32.Parse(continueGame);
                        if (nextGame == 0)
                        {
                            byte[] endMessageByte = new ASCIIEncoding().GetBytes(messageReader.getMessage("endMessage"));
                            stream.Write(endMessageByte, 0, endMessageByte.Length);
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
                    Console.WriteLine(ex);
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
