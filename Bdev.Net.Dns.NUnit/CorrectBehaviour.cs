using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Bdev.Net.Dns.Helpers;
using NUnit.Framework;

namespace Bdev.Net.Dns.NUnit
{
    /// <summary>
    ///     Summary description for CorrectBehaviour.
    /// </summary>
    [TestFixture]
    public class CorrectBehaviour
    {
        private static IEnumerable<IPAddress> GetDnsServers()
        {
            return
                NetworkInterface.GetAllNetworkInterfaces()
                    .Where(w1 => w1.OperationalStatus.Equals(OperationalStatus.Up))
                    .SelectMany(s1 => s1.GetIPProperties().DnsAddresses)
                    .Distinct();
        }

        private readonly Action<Action<IPAddress>> _multiDnsServerAction = action =>
        {
            var dnsServers = GetDnsServers().Where(w1 => w1.AddressFamily.Equals(AddressFamily.InterNetwork)).ToList();
            Parallel.ForEach(dnsServers, dnsServer =>
            {
                try
                {
                    action.Invoke(dnsServer);
                }
                catch (Exception e)
                {
                    //sink, it might be dead DNS Server
                    Trace.WriteLine(e);
                }
            });
        };

        [TestCase("apx-international.com")]
        [TestCase("Google.com")]
        [TestCase("ibm.com")]
        public void DomainsMustPass(string name)
        {
            var res = DnsServers.Resolve(name);
            Assert.IsNotNullOrEmpty(res.First().IPAddress.ToString());
            Trace.WriteLine(String.Join(Environment.NewLine, res.Select(s=>s.IPAddress)));
        }

        [Test]
        public void CorrectANameForCodeProject()
        {
            // also 194.72.0.114
            // create a new request
            var request = new Request();

            // add the codeproject ANAME question
            request.AddQuestion(new Question("codeproject.com", DnsType.ANAME, DnsClass.IN));

            // send the request
            Response response = null;
            _multiDnsServerAction.Invoke( dnsServer => response = Resolver.Lookup(request, dnsServer));

            // check the reponse
            Assert.AreEqual(ReturnCode.Success, response.ReturnCode);
            Assert.AreEqual(1, response.Answers.Length);
            var record = (ANameRecord) response.Answers[0].Record;
            Assert.AreEqual(IPAddress.Parse("65.39.148.34"), record.IPAddress);
        }

        [Test]
        public void CorrectMXForCodeProject()
        {
            // also 194.72.0.114
            MXRecord[] records = null;
            _multiDnsServerAction.Invoke(dnsServer => records = Resolver.MXLookup("codeproject.com", dnsServer));                    

            Assert.IsNotNull(records, "MXLookup returning null denoting lookup failure");
            Assert.IsTrue(records.Length>0);
        }

        [Test]
        public void CorrectNSForCodeProject()
        {
            // also 194.72.0.114
            // create a new request
            var request = new Request();

            // add the codeproject ANAME question
            request.AddQuestion(new Question("codeproject.com", DnsType.NS, DnsClass.IN));

            // send the request
            Response response = null;
            _multiDnsServerAction.Invoke(dnsServer => response= Resolver.Lookup(request, dnsServer));

            // check the reponse
            Assert.AreEqual(ReturnCode.Success, response.ReturnCode);

            // we expect 4 records
            Assert.IsTrue(response.Answers.Length>0);

        }

        [Test]
        [ExpectedException(typeof (NoResponseException))]
        public void NoResponseForBadDnsAddress()
        {
            MXRecord[] records = Resolver.MXLookup("codeproject.com", IPAddress.Parse("127.0.0.1"));
        }

        [Test]
        public void RepeatedANameLookups()
        {
            var request = new Request();

            // add the codeproject ANAME question
            request.AddQuestion(new Question("codeproject.com", DnsType.ANAME, DnsClass.IN));

            for (int i = 0; i < 1000; i++)
            {
                // send the request
                Response response = null;
                _multiDnsServerAction.Invoke( dnsServer => response = Resolver.Lookup(request, dnsServer));

                // check the reponse
                Assert.AreEqual(ReturnCode.Success, response.ReturnCode);
                Assert.AreEqual(1, response.Answers.Length);
            }
        }
    }
}
