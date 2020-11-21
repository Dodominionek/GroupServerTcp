using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerLibrary
{
    public class AsyncTcpServer : AbstractServer
    {
        byte[] loginMessage;
        byte[] passwordMessage;
        byte[] welcomeMessage;
        byte[] refuseMessage;
        static UserHandler userHandler = new UserHandler();
        static Dictionary<int, User> list = new Dictionary<int, User>();


        public delegate void TransmissionDataDelegate(NetworkStream stream);

        public AsyncTcpServer(IPAddress IP, int port) : base(IP, port)
        {
            this.loginMessage = new ASCIIEncoding().GetBytes("Podaj login: \r\n");
            this.passwordMessage = new ASCIIEncoding().GetBytes("Podaj haslo: \r\n");
            this.welcomeMessage = new ASCIIEncoding().GetBytes("Zalogowano \r\n");
            this.refuseMessage = new ASCIIEncoding().GetBytes("Nieprawidlowy login lub haslo \r\n");
        }

        protected override void AcceptClient()
        {
            while (true)
            {
                tcpClient = TcpListener.AcceptTcpClient();
                networkStream = tcpClient.GetStream();
                TransmissionDataDelegate transmissionDelegate = new TransmissionDataDelegate(BeginDataTransmission);
                transmissionDelegate.BeginInvoke(networkStream, TransmissionCallback, tcpClient);
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
         
            while(true)
            {
                try
                {
                    byte[] msg = new byte[256];
                    byte[] login = new byte[256];
                    byte[] password = new byte[256];

                    stream.Write(loginMessage, 0, loginMessage.Length);
                    do
                    {
                        stream.Read(login, 0, login.Length);
                    }while (Encoding.UTF8.GetString(login).Replace("\0", "") == "\r\n");
                    string login_s = Encoding.UTF8.GetString(login).Replace("\0","");

                    stream.Write(passwordMessage, 0, passwordMessage.Length);
                    do
                    {
                        stream.Read(password, 0, password.Length);
                    } while (Encoding.UTF8.GetString(password).Replace("\0", "") == "\r\n");
                    string password_s = Encoding.UTF8.GetString(password).Replace("\0", "");
                    try
                    {
                        if (credentials[login_s] == password_s)
                        {
                            stream.Write(welcomeMessage, 0, welcomeMessage.Length);
                            while (true)
                            {
                                int length = stream.Read(msg, 0, msg.Length);
                                string result = Encoding.UTF8.GetString(msg).ToUpper();
                                msg = Encoding.ASCII.GetBytes(result);
                                stream.Write(msg, 0, length);
                            }
                        }
                        else 
                        {
                             stream.Write(refuseMessage, 0, refuseMessage.Length);
                        }
                    }
                    catch
                    {
                        stream.Write(refuseMessage, 0, refuseMessage.Length);
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
