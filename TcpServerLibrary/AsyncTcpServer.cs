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
        byte[] guess;
        byte[] msg;
        byte[] msg2;
        byte[] msg3;
        byte[] msg4;

        public delegate void TransmissionDataDelegate(NetworkStream stream);

        public AsyncTcpServer(IPAddress IP, int port) : base(IP, port)
        {
            this.loginMessage = new ASCIIEncoding().GetBytes("Podaj login: \r\n");
            this.passwordMessage = new ASCIIEncoding().GetBytes("Podaj haslo: \r\n");
            this.welcomeMessage = new ASCIIEncoding().GetBytes("Zalogowano \r\n");
            this.refuseMessage = new ASCIIEncoding().GetBytes("Nieprawidlowy login lub haslo \r\n");
            this.guess = new ASCIIEncoding().GetBytes(" Zgadnij liczbe od 0 do 9 \r \n");
            this.msg = new ASCIIEncoding().GetBytes(" Brawo udalo ci sie zgadnac liczbe! \r \n");
            this.msg2 = new ASCIIEncoding().GetBytes(" Wybrano zla wartosc \r \n");
            this.msg3 = new ASCIIEncoding().GetBytes(" Chcesz kontynuowac rozgrywke ? (1-tak 0-nie) \r \n");
            this.msg4 = new ASCIIEncoding().GetBytes(" Rozgrywka zakonczona \r \n");
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

                    stream.Write(loginMessage, 0, loginMessage.Length);
                    do
                    {
                        stream.Read(login, 0, login.Length);
                    } while (Encoding.UTF8.GetString(login).Replace("\0", "") == "\r\n");
                    string login_s = Encoding.UTF8.GetString(login).Replace("\0", "");

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
                            int nextGame = 1;
                            while (true)
                            {
                                int length = stream.Read(msg, 0, msg.Length);
                                string result = Encoding.UTF8.GetString(msg).ToUpper();
                                msg = Encoding.ASCII.GetBytes(result);
                                stream.Write(msg, 0, length);

                                Random random = new Random();
                                int number = random.Next(10);
                               
                                byte[] buffer = new byte[256];
                                Console.WriteLine("Number to guess: " + number);
                                while (nextGame == 1)
                                {
                                    try
                                    {
                                        stream.Write(guess, 0, guess.Length);

                                        int responseLength = stream.Read(buffer, 0, buffer.Length);
                                        if (Encoding.UTF8.GetString(buffer, 0, responseLength) == "\r\n")
                                        {
                                            responseLength = stream.Read(buffer, 0, buffer.Length);
                                        }
                                        var guessedVal = Encoding.UTF8.GetString(buffer, 0, responseLength);

                                        String time = DateTime.Now.ToString("h:mm:ss");
                                        Console.WriteLine(time + " -> " + guessedVal);
                                        int guessedVal2;
                                        try
                                        {
                                            guessedVal2 = Int32.Parse(guessedVal);
                                        }
                                        catch (FormatException e)
                                        {
                                            guessedVal2 = 12;
                                        }

                                        if (guessedVal2 > 11 || guessedVal2 < 0)
                                        {
                                            stream.Write(msg2, 0, msg2.Length);
                                        }

                                        if (number.Equals(guessedVal2))
                                        {
                                            stream.Write(msg, 0, msg.Length);
                                            Console.WriteLine("Client guessed the number");

                                            stream.Write(msg3, 0, msg3.Length);

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
                                                stream.Write(msg4, 0, msg4.Length);
                                                tcpClient.GetStream().Close();
                                                tcpClient.Close();
                                                return;
                                            }
                                            else
                                            {
                                                number = random.Next(10);
                                                Console.WriteLine("Number to guess: " + number);
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
