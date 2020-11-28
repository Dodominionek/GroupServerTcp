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
            AcceptClient();
        }

        private string ReadResponse(NetworkStream stream)
        {
            byte[] response = new byte[256];
            do
            {
                stream.Read(response, 0, response.Length);
            } while (Encoding.UTF8.GetString(response).Replace("\0", "") == "\r\n");
            return Encoding.UTF8.GetString(response).Replace("\0", "");
        }

        private void SendMessage(NetworkStream stream, string message)
        {
            byte[] messageBytes = new ASCIIEncoding().GetBytes(message);
            stream.Write(messageBytes, 0, messageBytes.Length);
        }

        public bool Menu(NetworkStream stream, UserHandler userHandler)
        {
            SendMessage(stream, messageReader.getMessage("menu"));
            var msg = ReadResponse(stream);

            // rejestracja nowego uzytkownika
            if(msg == "2")
            {
                SendMessage(stream, messageReader.getMessage("loginMessage"));
                string login = ReadResponse(stream);

                SendMessage(stream, messageReader.getMessage("passwordMessage"));
                string password = ReadResponse(stream);
                try
                {
                    userHandler.AddNewUser(login, password);
                }
                catch{
                    SendMessage(stream, messageReader.getMessage("wrongLoginMessage"));
                    return false;
                }
            }
            return true;
        }

        protected override void BeginDataTransmission(NetworkStream stream)
        {
            UserHandler userHandler = new UserHandler();
            //userHandler.ShowUsers();
            var credentials = userHandler.Credentials;
         
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


                    SendMessage(stream, messageReader.getMessage("loginMessage"));
                    string login = ReadResponse(stream);

                    SendMessage(stream, messageReader.getMessage("passwordMessage"));
                    string password = ReadResponse(stream);

                    try
                    {
                        if (userHandler.Login(login,password))
                        {

                            SendMessage(stream, messageReader.getMessage("welcomeMessage"));

                            int length = stream.Read(msg, 0, msg.Length);
                            string result = Encoding.UTF8.GetString(msg).ToUpper();
                            msg = Encoding.ASCII.GetBytes(result);
                            stream.Write(msg, 0, length);

                            Game.guessingGame(stream, messageReader);
                            
                        }
                        else
                        {
                            SendMessage(stream, messageReader.getMessage("refuseMessage"));
                         
                        }
                    }
                    catch
                    {
                        SendMessage(stream, messageReader.getMessage("refuseMessage"));
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
