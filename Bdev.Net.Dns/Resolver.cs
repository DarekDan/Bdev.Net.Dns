#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
// 

#endregion

using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Bdev.Net.Dns.Exceptions;
using Bdev.Net.Dns.Helpers;
using Bdev.Net.Dns.Records;

namespace Bdev.Net.Dns
{
    /// <summary>
    ///     Summary description for Dns.
    /// </summary>
    public static partial class Resolver
    {
        private const int DnsPort = 53;
        private const int UdpRetryAttempts = 2;

        /// <summary>
        ///     Shorthand form to make MX querying easier, essentially wraps up the retrieval
        ///     of the MX records, and sorts them by preference
        /// </summary>
        /// <param name="domain">domain name to retrieve MX RRs for</param>
        /// <param name="dnsServer">the server we're going to ask</param>
        /// <returns>An array of MXRecords</returns>
        public static MXRecord[] MXLookup(string domain, IPAddress dnsServer)
        {
            // check the inputs
            if (domain == null) throw new ArgumentNullException(nameof(domain));
            if (dnsServer == null) throw new ArgumentNullException(nameof(dnsServer));

            // create a request for this
            var request = new Request();

            // add one question - the MX IN lookup for the supplied domain
            request.AddQuestion(new Question(domain, DnsType.MX));

            // fire it off
            var response = Lookup(request, dnsServer);

            // if we didn't get a response, then return null
            if (response == null) return null;

            // create a expandable array of MX records
            var resourceRecords = response.Answers.Where(w => w.Record is MXRecord).Select(s => s.Record as MXRecord)
                .OrderBy(o => o.Preference).ToArray();

            return resourceRecords;
        }

        public static MXRecord[] MXLookup(string domain)
        {
            return MXLookup(domain, DnsServers.IP4.First());
        }

        /// <summary>
        ///     The principal look up function, which sends a request message to the given
        ///     DNS server and collects a response. This implementation re-sends the message
        ///     via UDP up to two times in the event of no response/packet loss
        ///     
        ///     If the message is truncated and tcpFallback is set it will attempt to retry
        ///     via DNS over TCP.
        /// </summary>
        /// <param name="request">The logical request to send to the server</param>
        /// <param name="dnsServer">The IP address of the DNS server we are querying</param>
        /// <param name="tcpFallback">Whether it should fall back to TCP if the message is truncated</param>
        /// <returns>The logical response from the DNS server or null if no response</returns>
        public static Response Lookup(Request request, IPAddress dnsServer, bool tcpFallback = true)
        {
            // check the inputs
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (dnsServer == null) throw new ArgumentNullException(nameof(dnsServer));

            // We will not catch exceptions here, rather just refer them to the caller

            // create an end point to communicate with
            var server = new IPEndPoint(dnsServer, DnsPort);

            // get the message
            var requestMessage = request.GetMessage();

            // send the request and get the response
            var responseMessage = UdpTransfer(server, requestMessage);

            // populate a response object
            var response = new Response(responseMessage);

            if(response.MessageTruncated && tcpFallback)
            {
                // message is truncated, retry over TCP
                responseMessage = TcpTransfer(server, requestMessage);

                response = new Response(responseMessage);
            }

            return response;
        }



        public static Response Lookup(string value, DnsType type = DnsType.ANAME, IPAddress dnsServer = null)
        {
            var request = new Request();
            request.AddQuestion(new Question(value, type));
            return Lookup(request, dnsServer ?? DnsServers.IP4.First());
        }

        public static Response Lookup(Request request)
        {
            return Lookup(request, DnsServers.IP4.First());
        }

        private static byte[] UdpTransfer(IPEndPoint server, byte[] requestMessage)
        {
            // UDP can fail - if it does try again keeping track of how many attempts we've made
            var attempts = 0;

            // try repeatedly in case of failure
            while (attempts <= UdpRetryAttempts)
            {
                var uniqueId = DateTime.Now.Ticks;
                // firstly, uniquely mark this request with an id
                unchecked
                {
                    // substitute in an id unique to this lookup, the request has no idea about this
                    requestMessage[0] = (byte) (uniqueId >> 8);
                    requestMessage[1] = (byte) uniqueId;
                }

                // we'll be send and receiving a UDP packet
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // we will wait at most 1 second for a dns reply
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);

                // send it off to the server
                socket.SendTo(requestMessage, requestMessage.Length, SocketFlags.None, server);

                // RFC1035 states that the maximum size of a UDP datagram is 512 octets (bytes)
                var responseMessage = new byte[512];

                try
                {
                    // wait for a response up to 1 second
                    socket.Receive(responseMessage);

                    // make sure the message returned is ours
                    if (responseMessage[0] == requestMessage[0] && responseMessage[1] == requestMessage[1])
                        // its a valid response - return it, this is our successful exit point
                        return responseMessage;
                }
                catch (SocketException)
                {
                    // failure - we better try again, but remember how many attempts
                    attempts++;
                }
                finally
                {
                    // close the socket
                    socket.Close();
                }
            }

            // the operation has failed, this is our unsuccessful exit point
            throw new NoResponseException();
        }
    }
}