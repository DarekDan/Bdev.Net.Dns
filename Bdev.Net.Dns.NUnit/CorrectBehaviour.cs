using System;
using System.Net;
using Bdev.Net.Dns;
using NUnit.Framework;

namespace Bdev.Net.Dns.NUnit
{
	/// <summary>
	/// Summary description for CorrectBehaviour.
	/// </summary>
	[TestFixture]
	public class CorrectBehaviour
	{
		public CorrectBehaviour()
		{
		}

		[Test]
		public void CorrectMXForCodeProject()
		{
			// also 194.72.0.114
			MXRecord[] records = Resolver.MXLookup("codeproject.com", IPAddress.Parse("194.74.65.68"));
			
			Assert.IsNotNull(records, "MXLookup returning null denoting lookup failure");
			Assert.AreEqual(1, records.Length);
			Assert.AreEqual("mail.codeproject.com", records[0].DomainName);
			Assert.AreEqual(10, records[0].Preference);
		}

		[Test]
		public void CorrectANameForCodeProject()
		{
			// also 194.72.0.114
			// create a new request
			Request request = new Request();

			// add the codeproject ANAME question
			request.AddQuestion(new Question("codeproject.com", DnsType.ANAME, DnsClass.IN));
			
			// send the request
			Response response = Resolver.Lookup(request, IPAddress.Parse("194.74.65.68"));

			// check the reponse
			Assert.AreEqual(ReturnCode.Success, response.ReturnCode);
			Assert.AreEqual(1, response.Answers.Length);
			ANameRecord record = (ANameRecord)response.Answers[0].Record;
			Assert.AreEqual(IPAddress.Parse("209.171.52.99"), record.IPAddress);

		}

		[Test]
		public void CorrectNSForCodeProject()
		{
			// also 194.72.0.114
			// create a new request
			Request request = new Request();

			// add the codeproject ANAME question
			request.AddQuestion(new Question("codeproject.com", DnsType.NS, DnsClass.IN));
			
			// send the request
			Response response = Resolver.Lookup(request, IPAddress.Parse("194.74.65.68"));

			// check the reponse
			Assert.AreEqual(ReturnCode.Success, response.ReturnCode);
			
			// we expect 4 records
			Assert.AreEqual(5, response.Answers.Length);
			
			// get them
			NSRecord record1 = (NSRecord)response.Answers[0].Record;
			NSRecord record2 = (NSRecord)response.Answers[1].Record;
			NSRecord record3 = (NSRecord)response.Answers[2].Record;
			NSRecord record4 = (NSRecord)response.Answers[3].Record;
			NSRecord record5 = (NSRecord)response.Answers[4].Record;

			Assert.IsTrue(record1.DomainName == "ns1.easydns.com" ||
							record2.DomainName == "ns1.easydns.com" ||
							record3.DomainName == "ns1.easydns.com" ||
							record4.DomainName == "ns1.easydns.com" ||
							record5.DomainName == "ns1.easydns.com");
			
		}

		[Test]
		public void RepeatedANameLookups()
		{
			Request request = new Request();

			// add the codeproject ANAME question
			request.AddQuestion(new Question("codeproject.com", DnsType.ANAME, DnsClass.IN));
			
			for (int i = 0; i < 1000; i++)
			{
				// send the request
				Response response = Resolver.Lookup(request, IPAddress.Parse("194.74.65.68"));

				// check the reponse
				Assert.AreEqual(ReturnCode.Success, response.ReturnCode);
				Assert.AreEqual(1, response.Answers.Length);
			}
		}

		[Test]
		[ExpectedException(typeof(NoResponseException))]
		public void NoResponseForBadDnsAddress()
		{
			MXRecord[] records = Resolver.MXLookup("codeproject.com", IPAddress.Parse("84.234.16.185"));
		}
	}
}
