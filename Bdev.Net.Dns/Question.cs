#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
//

#endregion

using System.Text.RegularExpressions;

namespace Bdev.Net.Dns
{
    /// <summary>
    ///     Represents a DNS Question, comprising of a domain to query, the type of query (QTYPE) and the class
    ///     of query (QCLASS). This class is an encapsulation of these three things, and extensive argument checking
    ///     in the constructor as this may well be created outside the assembly (public protection).
    /// </summary>
    [Serializable]
    public class Question // A question is these three things combined.
    {
        /// <summary>Construct the question from parameters, checking for safety.</summary>
        /// <param name="domain">the domain name to query eg. 'bigdevelopments.co.uk'.</param>
        /// <param name="dnsType">the QTYPE of query eg. 'DnsType.MX'.</param>
        /// <param name="dnsClass">the CLASS of query, invariably 'DnsClass.IN'.</param>
        public Question(string? domain, DnsType dnsType, DnsClass dnsClass = DnsClass.IN)
        {
            // check the input parameters
            if (domain is null) throw new ArgumentNullException(nameof(domain));

            // do a sanity check on the domain name to make sure its legal
            if (domain.Length == 0 || domain.Length > 255 ||
                !Regex.IsMatch(domain, @"^[a-z|A-Z|0-9|\-|_]{1,63}(\.[a-z|A-Z|0-9|\-|_]{1,63})+$"))
                // domain names can't be bigger than 255 chars, and individual labels can't be bigger than 63 chars
                throw new ArgumentException("The supplied domain name was not in the correct form.", nameof(domain));

            // sanity check the DnsType parameter
            if (!Enum.IsDefined(typeof(DnsType), dnsType) || dnsType == DnsType.None)
                throw new ArgumentOutOfRangeException(nameof(dnsType), "Not a valid value.");

            // sanity check the DnsClass parameter
            if (!Enum.IsDefined(typeof(DnsClass), dnsClass) || dnsClass == DnsClass.None)
                throw new ArgumentOutOfRangeException(nameof(dnsClass), "Not a valid value.");

            // just remember the values
            Domain = domain;
            Type = dnsType;
            Class = dnsClass;
        }

        /// <summary>
        ///     Construct the question reading from a DNS Server response. Consult RFC1035 4.1.2
        ///     for byte-wise details of this structure in byte array form.
        /// </summary>
        /// <param name="pointer">a logical pointer to the Question in byte array form.</param>
        internal Question(Pointer pointer)
        {
            // extract from the message
            Domain = pointer.ReadDomain();
            Type = (DnsType)pointer.ReadShort();
            Class = (DnsClass)pointer.ReadShort();
        }

        // expose these fields r/o to the world
        public string Domain { get; }
        public DnsType Type { get; }
        public DnsClass Class { get; }
    }
}
