using Bdev.Net.Dns.Exceptions;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Bdev.Net.Dns
{
    public partial class Resolver
    {
        private static byte[] TcpTransfer(IPEndPoint server, byte[] requestMessage, int timeout = 2000)
        {
            // allocate 2 extra bytes for the message lenght
            var request = new byte[requestMessage.Length + 2];
            requestMessage.CopyTo(request, 2);

            unchecked
            {
                // message length
                request[0] = (byte)(requestMessage.Length >> 8);
                request[1] = (byte)(requestMessage.Length);

                // mark the request with an id
                request[2] = (byte)(_uniqueId >> 8);
                request[3] = (byte)_uniqueId;
            }

            // this will use DNS over TCP
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.SendTimeout = 2000;
            socket.ReceiveTimeout = 2000;

            // there is no limit of 512 on DNS over TCP
            var responseMessage = new byte[65535];

            try
            {
                // connect with a timeout
                var connect = socket.BeginConnect(server, null, null);
                connect.AsyncWaitHandle.WaitOne(timeout, true);

                if (!socket.Connected)
                {
                    // we will not retry since this is TCP
                    throw new NoResponseException();
                }

                socket.EndConnect(connect);

                // send the dns query
                socket.SendTo(request, request.Length, SocketFlags.None, server);

                // receive the response
                socket.Receive(responseMessage);
            }
            catch(SocketException)
            {
                throw new NoResponseException();
            }
            finally
            {
                _uniqueId++;

                socket.Close();
            }

            // strip the length and return
            return responseMessage.Skip(2).ToArray();
        }
    }
}
