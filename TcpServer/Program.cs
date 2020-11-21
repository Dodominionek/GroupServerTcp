using System.Net;
using ServerLibrary;

namespace IO_Lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncTcpServer tcpServer = new AsyncTcpServer(IPAddress.Parse("127.0.0.1"), 8000);
            tcpServer.Start();
        }
    }
}
