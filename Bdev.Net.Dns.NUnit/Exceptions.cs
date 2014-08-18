using System;
using System.Net;
using Bdev.Net.Dns;
using NUnit.Framework;

namespace Bdev.Net.Dns.NUnit
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestFixture]
	public class Exceptions
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MXLookupNullFirstParameter()
		{
			MXRecord[] records = Resolver.MXLookup(null, IPAddress.Parse("127.0.0.1"));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MXLookupNullSecondParameter()
		{
			MXRecord[] records = Resolver.MXLookup("codeproject.com", null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MXLookupNullBothParameters()
		{
			MXRecord[] records = Resolver.MXLookup(null, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void MXLookupBadDomainName()
		{
			MXRecord[] records = Resolver.MXLookup("!£$%^&*()", IPAddress.Parse("127.0.0.1"));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void MXLookupEmptyDomainName()
		{
			MXRecord[] records = Resolver.MXLookup(string.Empty, IPAddress.Parse("127.0.0.1"));
		}


		// --------------------------   

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void LookupNullFirstParameter()
		{
			Response response = Resolver.Lookup(null, IPAddress.Parse("127.0.0.1"));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void LookupNullSecondParameter()
		{
			Response response = Resolver.Lookup(new Request(), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void LookupNullBothParameters()
		{
			Response response = Resolver.Lookup(null, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NewQuestionDomainNull()
		{
			Question question = new Question(null, DnsType.MX, DnsClass.IN);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void NewQuestionDomainBad()
		{
			Question question = new Question("$$$$$.com", DnsType.MX, DnsClass.IN);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void NewQuestionBadType()
		{
			Question question = new Question("codeproject.com", (DnsType)1999, DnsClass.IN);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void NewQuestionBadClass()
		{
			Question question = new Question("codeproject.com", DnsType.ANAME, (DnsClass)1999);
		}
	}
}
