using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bdev.Net.Dns.Exceptions;
using Bdev.Net.Dns.Helpers;
using Bdev.Net.Dns.Records;
using NLog;
using NUnit.Framework;

namespace Bdev.Net.Dns.NUnit
{
    /// <summary>
    ///     Summary description for CorrectBehaviour.
    /// </summary>
    [TestFixture]
    public class CorrectBehaviour
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static object[] _dnsServersTable =
        {
            new object[] { "1.1.1.1", "1.0.0.1" },
            new object[] { "208.67.222.222", "208.67.220.220" },
            new object[] { "8.8.8.8", "8.8.4.4" },
            new object[] { "9.9.9.9", "149.112.112.112" },
            new object[] { "8.8.4.4", "1.1.1.1" }
        };

        [TestCase("mercedes-benz.com")]
        [TestCase("Google.com")]
        [TestCase("ibm.com")]
        public void DomainsMustPass(string name)
        {
            var res = DnsServers.Resolve(name).ToList();
            Assert.IsNotNullOrEmpty(res.First().IPAddress.ToString());
            Console.WriteLine(string.Join(Environment.NewLine, res.Select(s => s.IPAddress)));
        }

        [TestCaseSource(nameof(_dnsServersTable))]
        public void CompareTwoRecords(string firstDns, string secondDns)
        {
            var request = Request.Question(new Question("bisoftware.com", DnsType.ANAME));

            var firstAnswers = Resolver.Lookup(request, IPAddress.Parse(firstDns)).Answers;
            var secondAnswers = Resolver.Lookup(request, IPAddress.Parse(secondDns)).Answers;
            var first = firstAnswers.OrderBy(o => o.Record).First();
            var second = secondAnswers.OrderBy(o => o.Record).First();
            Logger.Info($"First: {first.Record} Second: {second.Record}");
            Assert.True(first.Equals(second));
        }

        [Test]
        public void CompareTwoRecordsOneDefault()
        {
            var request = new Request { RecursionDesired = true };
            const string value = "bisoftware.com";
            request.AddQuestion(new Question(value, DnsType.ANAME));

            var firstAnswers = Resolver.Lookup(value).Answers;
            var first = firstAnswers.OrderBy(o => o.Record).First();
            var secondAnswers = Resolver.Lookup(request, IPAddress.Parse("1.1.1.1")).Answers;
            var second = secondAnswers.OrderBy(o => o.Record).First();
            Assert.True(first.Record.ToString().Equals(second.Record.ToString()));
        }

        [Test]
        public void TextRecordsMustExist()
        {
            var result =
                Resolver.Lookup(
                    new Request { RecursionDesired = true }.WithQuestion(new Question("test.txt.bisoftware.com",
                        DnsType.TXT)));
            var l = DnsServers.Resolve<TXTRecord>("test.txt.bisoftware.com");
            var list = result.Answers.Select(s => s.Record).OfType<TXTRecord>().Select(s => s.Value).OrderBy(o => o)
                .ToList();
            Assert.True(result.Answers.Length > 0);
            Assert.IsTrue(list.SequenceEqual(l.Select(s => s.Value).OrderBy(o => o).ToList()));
        }

        [Test]
        public void CNameRecordMustExist()
        {
            var result = DnsServers.Resolve<CNameRecord>("test.cname.bisoftware.com").First();
            Console.WriteLine(result);
            Assert.IsNotEmpty(result.Value);
        }

        [Test]
        public void CorrectCNameForGmail()
        {
            var result = DnsServers.Resolve<CNameRecord>("test.cname.bisoftware.com").First();

            Assert.AreEqual(result.Value, "bisoftware.com");
        }

        [Test]
        public void CorrectANameForCodeProject()
        {
            var response = DnsServers.Resolve("codeproject.com").ToList();

            // check the response
            Assert.AreEqual(1, response.Count);
            Assert.AreEqual(IPAddress.Parse("76.74.234.210"), response.First().IPAddress);
        }

        [Test]
        public void CorrectMXForCodeProject()
        {
            // also 194.72.0.114
            var records = Resolver.MXLookup("codeproject.com", DnsServers.IP4.First());

            Assert.IsNotNull(records, "MXLookup returning null denoting lookup failure");
            Assert.IsTrue(records.Length > 0);
        }

        [Test]
        public void CorrectMXForCodeProjectCompare()
        {
            var result = DnsServers.Resolve<MXRecord>("codeproject.com", DnsType.MX, DnsClass.IN);

            var records = Resolver.MXLookup("codeproject.com", DnsServers.IP4.First());
            Assert.IsNotNull(records, "MXLookup returning null denoting lookup failure");
            Assert.IsTrue(records.Length > 0);

            Assert.IsTrue(records.All(a => result.Contains(a)));
            Assert.IsTrue(result.All(a => records.Contains(a)));
        }

        [Test]
        public void CorrectNSForCodeProject()
        {
            // also 194.72.0.114
            // create a new request
            var request = new Request();

            // add the codeproject ANAME question
            request.AddQuestion(new Question("codeproject.com", DnsType.NS));

            // send the request
            var response = Resolver.Lookup(request, DnsServers.IP4.First());

            // check the response
            Assert.AreEqual(ReturnCode.Success, response.ReturnCode);

            // we expect 4 records
            Assert.IsTrue(response.Answers.Length > 0);
        }

        [Test]
        [ExpectedException(typeof(NoResponseException))]
        public void NoResponseForBadDnsAddress()
        {
            Resolver.MXLookup("codeproject.com", IPAddress.Parse("127.0.0.1"));
        }

        [Test]
        public void RepeatedANameLookups()
        {
            Parallel.For(0, 10, i =>
            {
                // send the request
                var response = DnsServers.Resolve("codeproject.com").ToArray();

                // check the response
                Assert.AreEqual(1, response.Length);
            });
        }

        [Test]
        public void CorrectAAAAForCloudflare()
        {
            var result = DnsServers.Resolve<AAAARecord>("one.one.one.one").ToList();
            Logger.Info($"{result[0].IPAddress}");
            Logger.Info($"{result[1].IPAddress}");

            Assert.True(result.Select(s=>s.IPAddress).SequenceEqual(new []{ IPAddress.Parse("2606:4700:4700::1111"), IPAddress.Parse("2606:4700:4700::1001") }));
        }

        [Test]
        public void CorrectSRVForDebian()
        {
            var result = DnsServers.Resolve<SRVRecord>("_http._tcp.ftp.debian.org").First();

            Assert.AreEqual(10, result.Priority);
            Assert.AreEqual(1, result.Weight);
            Assert.AreEqual(80, result.Port);
            Assert.AreEqual("debian.map.fastlydns.net", result.Target);
        }

        [Test]
        public void CorrectDSForIana()
        {
            var result = DnsServers.Resolve<DSRecord>("iana.org").First();

            Assert.AreEqual(39229, result.KeyTag);
            Assert.AreEqual(DnsSecAlgorithm.ECDSAP256SHA256, result.Algorithm);
            Assert.AreEqual(DnsSecDigestType.SHA256, result.DigestType);
            Assert.IsNotEmpty(result.Digest);
        }

        [Test]
        public void LookupShortRecordType()
        {
            // looks up a record type that exceeds a single octet
            var result = Resolver.Lookup("google.com", DnsType.CAA);

            Assert.True(result.Questions.All(q => q.Type == DnsType.CAA));
            Assert.True(result.Answers.All(a => a.Type == DnsType.CAA));
        }
    }
}