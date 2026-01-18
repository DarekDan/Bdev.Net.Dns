using System;
using System.Net;
using Bdev.Net.Dns.Exceptions;
using NUnit.Framework;

namespace Bdev.Net.Dns.NUnit
{
	/// <summary>
	///     Summary description for Class1.
	/// </summary>
	[TestFixture]
	public class Exceptions
	{
		// --------------------------   
		[Test]
		public void BadDnsServerShouldFail()
		{
			var request = new Request();
			request.AddQuestion(new Question("bisoftware.com", DnsType.ANAME));
			Assert.Throws<NoResponseException>(() =>
				Resolver.Lookup(request, IPAddress.Parse("127.0.0.1"), false)
			);
		}

		[Test]
		public void LookupNullBothParameters()
		{
			Assert.Throws<ArgumentNullException>(() =>
				Resolver.Lookup(null, null)
			);
		}

		[Test]
		public void LookupNullFirstParameter()
		{
			Assert.Throws<ArgumentNullException>(() =>
				Resolver.Lookup(null, IPAddress.Parse("127.0.0.1"))
			);
		}

		[Test]
		public void LookupNullSecondParameter()
		{
			Assert.Throws<ArgumentNullException>(() =>
				Resolver.Lookup(new Request(), null)
			);
		}

		[Test]
		public void MXLookupBadDomainName()
		{
			Assert.Throws<ArgumentException>(() =>
				Resolver.MXLookup("!£$%^&*()", IPAddress.Parse("127.0.0.1"))
			);
		}

		[Test]
		public void MXLookupEmptyDomainName()
		{
			Assert.Throws<ArgumentException>(() =>
				Resolver.MXLookup(string.Empty, IPAddress.Parse("127.0.0.1"))
			);
		}

		[Test]
		public void MXLookupNullBothParameters()
		{
			Assert.Throws<ArgumentNullException>(() =>
				Resolver.MXLookup(null, null)
			);
		}

		[Test]
		public void MXLookupNullFirstParameter()
		{
			Assert.Throws<ArgumentNullException>(() =>
				Resolver.MXLookup(null, IPAddress.Parse("127.0.0.1"))
			);
		}

		[Test]
		public void MXLookupNullSecondParameter()
		{
			Assert.Throws<ArgumentNullException>(() =>
				Resolver.MXLookup("codeproject.com", null)
			);
		}

		[Test]
		public void NewQuestionBadClass()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
				_ = new Question("codeproject.com", DnsType.ANAME, (DnsClass)1999)
			);
		}

		[Test]
		public void NewQuestionBadType()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
				_ = new Question("codeproject.com", (DnsType)1999)
			);
		}

		[Test]
		public void NewQuestionDomainBad()
		{
			Assert.Throws<ArgumentException>(() =>
				_ = new Question("$$$$$.com", DnsType.MX)
			);
		}

		[Test]
		public void NewQuestionDomainNull()
		{
			Assert.Throws<ArgumentNullException>(() =>
				_ = new Question(null, DnsType.MX)
			);
		}
	}
}