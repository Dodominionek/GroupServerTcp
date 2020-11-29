using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpServerLibrary
{
    class ClientComunicator
    {
        public string ReadResponse(NetworkStream stream)
        {
            byte[] response = new byte[256];
            do
            {
                stream.Read(response, 0, response.Length);
            } while (Encoding.UTF8.GetString(response).Replace("\0", "") == "\r\n");
            return Encoding.UTF8.GetString(response).Replace("\0", "");
        }

        public void SendMessage(NetworkStream stream, string message)
        {
            byte[] messageBytes = new ASCIIEncoding().GetBytes(message);
            stream.Write(messageBytes, 0, messageBytes.Length);
        }

    
    }

}