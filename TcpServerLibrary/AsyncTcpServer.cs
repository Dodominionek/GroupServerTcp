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
        MessageReader messageReader;
        ClientComunicator comunicator;
        public delegate void TransmissionDataDelegate(NetworkStream stream);

        public AsyncTcpServer(IPAddress IP, int port) : base(IP, port)
        {
            this.messageReader = new MessageReader();
            this.comunicator = new ClientComunicator();
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
            AcceptClient();
        }


        public bool Menu(NetworkStream stream, UserHandler userHandler)
        {
            comunicator.SendMessage(stream, messageReader.getMessage("menu"));
            var msg = comunicator.ReadResponse(stream);

            // rejestracja nowego uzytkownika
            if (msg == "2")
            {
                comunicator.SendMessage(stream, messageReader.getMessage("loginMessage"));
                string login = comunicator.ReadResponse(stream);

                comunicator.SendMessage(stream, messageReader.getMessage("passwordMessage"));
                string password = comunicator.ReadResponse(stream);

                comunicator.SendMessage(stream, messageReader.getMessage("passwordMessage"));
                int permission = Int32.Parse(comunicator.ReadResponse(stream));
                try
                {
                    userHandler.AddNewUser(login, password, permission);
                }
                catch
                {
                    comunicator.SendMessage(stream, messageReader.getMessage("wrongLoginMessage"));
                    return false;
                }
            }
            return true;
        }

        protected override void BeginDataTransmission(NetworkStream stream)
        {
            UserHandler userHandler = new UserHandler();
            //userHandler.ShowUsers();
            var credentials = userHandler.UserList;

            while (true)
            {
                try
                {
                    byte[] msg = new byte[256];

                    bool go = false;
                    do
                    {
                        go = Menu(stream, userHandler);
                    } while (go == false);


                    comunicator.SendMessage(stream, messageReader.getMessage("loginMessage"));
                    string login = comunicator.ReadResponse(stream);

                    comunicator.SendMessage(stream, messageReader.getMessage("passwordMessage"));
                    string password = comunicator.ReadResponse(stream);

                    Console.WriteLine("|" + login + "|" + password + "|");
                    userHandler.ShowUsers();

                    try
                    {
                        if (userHandler.Login(login, password))
                        {

                            comunicator.SendMessage(stream, messageReader.getMessage("welcomeMessage"));

                            int length = stream.Read(msg, 0, msg.Length);
                            string result = Encoding.UTF8.GetString(msg).ToUpper();
                            msg = Encoding.ASCII.GetBytes(result);
                            stream.Write(msg, 0, length);

                            Game.guessingGame(stream, messageReader, comunicator);

                        }
                        else
                        {
                            comunicator.SendMessage(stream, messageReader.getMessage("refuseMessage"));
                        }
                    }
                    catch
                    {
                        comunicator.SendMessage(stream, messageReader.getMessage("refuseMessage"));
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
