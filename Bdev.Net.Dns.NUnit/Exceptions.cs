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
        [ExpectedException(typeof(NoResponseException))]
        public void BadDnsServerShouldFail()
        {
            var request = new Request();
            request.AddQuestion(new Question("bisoftware.com", DnsType.ANAME));
            var resolved = Resolver.Lookup(request, IPAddress.Parse("127.0.0.1"), false);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LookupNullBothParameters()
        {
            Resolver.Lookup(null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LookupNullFirstParameter()
        {
            Resolver.Lookup(null, IPAddress.Parse("127.0.0.1"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LookupNullSecondParameter()
        {
            Resolver.Lookup(new Request(), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MXLookupBadDomainName()
        {
            Resolver.MXLookup("!£$%^&*()", IPAddress.Parse("127.0.0.1"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MXLookupEmptyDomainName()
        {
            Resolver.MXLookup(string.Empty, IPAddress.Parse("127.0.0.1"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MXLookupNullBothParameters()
        {
            Resolver.MXLookup(null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MXLookupNullFirstParameter()
        {
            Resolver.MXLookup(null, IPAddress.Parse("127.0.0.1"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MXLookupNullSecondParameter()
        {
            Resolver.MXLookup("codeproject.com", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NewQuestionBadClass()
        {
            _ = new Question("codeproject.com", DnsType.ANAME, (DnsClass)1999);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NewQuestionBadType()
        {
            _ = new Question("codeproject.com", (DnsType)1999);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void NewQuestionDomainBad()
        {
            _ = new Question("$$$$$.com", DnsType.MX);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NewQuestionDomainNull()
        {
            _ = new Question(null, DnsType.MX);
        }
    }
}