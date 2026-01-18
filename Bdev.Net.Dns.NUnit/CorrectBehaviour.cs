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
			Assert.That(res.First().IPAddress.ToString(), Is.Not.Null.And.Not.Empty);
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
			Assert.That(first, Is.EqualTo(second));
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
            Assert.That(first.Record.ToString().Equals(second.Record.ToString()), Is.True);
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
			Assert.That(result.Answers.Length, Is.GreaterThan(0));
			Assert.That(list, Is.EqualTo(l.Select(s => s.Value).OrderBy(o => o).ToList()));
		}

        [Test]
        public void CNameRecordMustExist()
        {
            var result = DnsServers.Resolve<CNameRecord>("test.cname.bisoftware.com").First();
            Console.WriteLine(result);
			Assert.That(result.Value, Is.Not.Empty);
		}

        [Test]
        public void CorrectCNameForGmail()
        {
            var result = DnsServers.Resolve<CNameRecord>("test.cname.bisoftware.com").First();

			Assert.That(result.Value, Is.EqualTo("bisoftware.com"));
		}

        [Test]
        public void CorrectANameFor1111()
        {
            var response = DnsServers.Resolve("one.one.one.one").ToList();

			// check the response
			Assert.That(response.Count, Is.EqualTo(2));
			Assert.That(response.Select(r => r.IPAddress), Does.Contain(IPAddress.Parse("1.1.1.1")));
		}

        [Test]
        public void CorrectMXForCodeProject()
        {
            // also 194.72.0.114
            var records = Resolver.MXLookup("codeproject.com", DnsServers.IP4.First());

			Assert.That(records, Is.Not.Null, "MXLookup returning null denoting lookup failure");
			Assert.That(records, Is.Not.Empty);
		}

        [Test]
        public void CorrectMXForCodeProjectCompare()
        {
            var result = DnsServers.Resolve<MXRecord>("codeproject.com", DnsType.MX, DnsClass.IN);

            var records = Resolver.MXLookup("codeproject.com", DnsServers.IP4.First());
            Assert.That(records, Is.Not.Null, "MXLookup returning null denoting lookup failure");
			Assert.That(records.Length, Is.GreaterThan(0));

			Assert.That(records.All(a => result.Contains(a)), Is.True);
			Assert.That(result.All(a => records.Contains(a)), Is.True);
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
			Assert.That(response.ReturnCode, Is.EqualTo(ReturnCode.Success));

			// we expect 4 records
			Assert.That(response.Answers.Length, Is.GreaterThan(0));
		}

		[Test]
        public void NoResponseForBadDnsAddress()
        {
	        Assert.Throws<NoResponseException>(() =>
		        Resolver.MXLookup("codeproject.com", IPAddress.Parse("127.0.0.1"))
	        );
        }

		[Test]
        public void RepeatedANameLookups()
        {
            Parallel.For(0, 10, i =>
            {
                // send the request
                var response = DnsServers.Resolve("codeproject.com").ToArray();

				// check the response
				Assert.That(response.Length, Is.GreaterThanOrEqualTo(1));
			});
        }

        [Test]
        public void CorrectAAAAForCloudflare()
        {
            var result = DnsServers.Resolve<AAAARecord>("one.one.one.one").ToList();
            Logger.Info($"{result[0].IPAddress}");
            Logger.Info($"{result[1].IPAddress}");

			Assert.That(result.Select(s => s.IPAddress), Is.EqualTo(new[] {
				IPAddress.Parse("2606:4700:4700::1111"),
				IPAddress.Parse("2606:4700:4700::1001")
			}));
		}

        [Test]
        public void CorrectSRVForDebian()
        {
            var result = DnsServers.Resolve<SRVRecord>("_http._tcp.ftp.debian.org").First();

            Assert.That(result.Priority, Is.EqualTo(10));
            Assert.That(result.Weight, Is.EqualTo(1));
            Assert.That(result.Port, Is.EqualTo(80));
            Assert.That(result.Target, Is.EqualTo("debian.map.fastlydns.net"));
		}

        [Test]
        public void CorrectDSForIana()
        {
            var result = DnsServers.Resolve<DSRecord>("iana.org").First();

            Assert.That(result.KeyTag, Is.EqualTo(2234),"but was:"+result.KeyTag);
            Assert.That(result.Algorithm, Is.EqualTo(DnsSecAlgorithm.ECDSAP256SHA256));
            Assert.That(result.DigestType, Is.EqualTo(DnsSecDigestType.SHA256));
            Assert.That(result.Digest, Is.Not.Empty);
		}

        [Test]
        public void LookupShortRecordType()
        {
            // looks up a record type that exceeds a single octet
            var result = Resolver.Lookup("google.com", DnsType.CAA);

            Assert.That(result.Questions.All(q => q.Type == DnsType.CAA), Is.True);
            Assert.That(result.Answers.All(a => a.Type == DnsType.CAA), Is.True);
		}

        [Test]
        public void CheckCAARecord()
        {
            CAARecord google = DnsServers.Resolve<CAARecord>("google.com").First();
            Assert.That(google.Flags, Is.EqualTo(0));
            Assert.That(google.Tag, Is.EqualTo("issue"));
            Assert.That(google.Value, Is.EqualTo("pki.goog"));
        }
    }
}