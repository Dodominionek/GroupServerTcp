using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpServerLibrary;

namespace ServerLibrary
{
    public class AsyncTcpServer : AbstractServer
    {
        static UserHandler userHandler = new UserHandler();
        static Dictionary<int, User> list = new Dictionary<int, User>();
        MessageReader messageReader;
        public delegate void TransmissionDataDelegate(NetworkStream stream);

        public AsyncTcpServer(IPAddress IP, int port) : base(IP, port)
        {
            this.messageReader = new MessageReader();
        }

        protected override void AcceptClient()
        {
            while (true)
            {
                tcpClient = TcpListener.AcceptTcpClient();
                networkStream = tcpClient.GetStream();
                TransmissionDataDelegate transmissionDelegate = new TransmissionDataDelegate(BeginDataTransmission);
                transmissionDelegate.BeginInvoke(networkStream, TransmissionCallback, tcpClient);
                Console.WriteLine("Client connected");
            }
        }

        private void TransmissionCallback(IAsyncResult ar)
        {
            tcpClient.Close();
        }

        public override void Start()
        {
            StartListening();
            userHandler.init(list);
            AcceptClient();
        }

        protected override void BeginDataTransmission(NetworkStream stream)
        {
            var credentials = userHandler.ReadUsersCredentials();
         
            while (true)
            {
                try
                {
                    byte[] msg = new byte[256];
                    byte[] login = new byte[256];
                    byte[] password = new byte[256];

                    byte[] loginMessageByte = new ASCIIEncoding().GetBytes(messageReader.getMessage("loginMessage"));
                    stream.Write(loginMessageByte, 0, loginMessageByte.Length);

                    do
                    {
                        stream.Read(login, 0, login.Length);
                    } while (Encoding.UTF8.GetString(login).Replace("\0", "") == "\r\n");
                    string login_s = Encoding.UTF8.GetString(login).Replace("\0", "");

                    string passwordMessage = "Podaj haslo: \r\n";
                    byte[] passwordMessageByte = new ASCIIEncoding().GetBytes(passwordMessage);
                    stream.Write(passwordMessageByte, 0, passwordMessageByte.Length);
                    Console.WriteLine(passwordMessage);

                    do
                    {
                        stream.Read(password, 0, password.Length);
                    } while (Encoding.UTF8.GetString(password).Replace("\0", "") == "\r\n");
                    string password_s = Encoding.UTF8.GetString(password).Replace("\0", "");

                    try
                    {
                        if (credentials[login_s] == password_s)
                        {
                            Game game = new Game();
                           
                            byte[] welcomeMessage = new ASCIIEncoding().GetBytes(messageReader.getMessage("welcomeMessage"));
                            stream.Write(welcomeMessage, 0, welcomeMessage.Length);

                            int nextGame = 1;

                            int length = stream.Read(msg, 0, msg.Length);
                            string result = Encoding.UTF8.GetString(msg).ToUpper();
                            msg = Encoding.ASCII.GetBytes(result);
                            stream.Write(msg, 0, length);

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

                                            tcpClient.GetStream().Close();
                                            tcpClient.Close();
                                            return;
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
                        else
                        {
                            byte[] refuseMessageByte = new ASCIIEncoding().GetBytes(messageReader.getMessage("refuseMessage"));
                            stream.Write(refuseMessageByte, 0, refuseMessageByte.Length);
                        }
                    }
                    catch
                    {
                        byte[] refuseMessageByte = new ASCIIEncoding().GetBytes(messageReader.getMessage("refuseMessage"));
                        stream.Write(refuseMessageByte, 0, refuseMessageByte.Length);
                    }
                }
                catch (IOException)
                {
                    break;
                }
            }


        }
    }
}
