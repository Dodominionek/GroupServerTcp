using System;
using System.Net;
using System.Net.Sockets;

namespace ServerLibrary
{
    public abstract class AbstractServer
    {
        protected IPAddress iPAddress;
        protected int port;
        protected bool running;
        protected TcpListener tcpListener;
        protected TcpClient tcpClient;
        protected NetworkStream networkStream;

        public IPAddress IPAddress { get => iPAddress; set { if (!running) iPAddress = value; else throw new Exception("Podano zły adres IP"); } }

        public int Port
        {
            get => port; set
            {
                if (value < 1024 || value > 49151)
                {
                    throw new Exception("Podano złą wartość portu");
                }
                if (!running) port = value; else throw new Exception("Nie można zmienić portu w trakcie działania serwera");
            }
        }

        protected TcpListener TcpListener { get => tcpListener; set => tcpListener = value; }
        protected TcpClient TcpClient { get => tcpClient; set => tcpClient = value; }
        protected NetworkStream Stream { get => networkStream; set => networkStream = value; }

        public AbstractServer(IPAddress IP, int port)
        {
            running = false;
            IPAddress = IP;
            Port = port;
            if (port < 1024 || port > 49151)
            {
                Port = 8000;
                throw new Exception("Zła wartość portu. Ustawiono na 8000");
            }
        }

        protected void StartListening()
        {
            TcpListener = new TcpListener(IPAddress, Port);
            TcpListener.Start();
            Console.WriteLine("Serwer został uruchomiony");
        }
        protected abstract void AcceptClient();
        protected abstract void BeginDataTransmission(NetworkStream stream);
        public abstract void Start();

    }
}
