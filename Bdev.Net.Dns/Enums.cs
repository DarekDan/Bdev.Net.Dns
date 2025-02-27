#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
// 

#endregion

// ReSharper disable UnusedMember.Global
namespace Bdev.Net.Dns
{
    /// <summary>
    ///     The DNS TYPE (RFC1035 3.2.2/3) - 4 types are currently supported. Also, I know that this
    ///     enumeration goes against naming guidelines, but I have done this as an ANAME is most
    ///     definitely an 'ANAME' and not an 'Aname'. See also https://tools.ietf.org/html/rfc1035#section-3.2.2
    /// </summary>
    public enum DnsType
    {
        None = 0, //INVALID
        A = 1, //host address
        ANAME = 1, //host address
        NS = 2, //authoritative name server
        MD = 3, //mail destination - obsolete, use MX
        MF = 4, //mail forwarder -  obsolete, use MX
        CNAME = 5, //canonical name for an alias
        SOA = 6, //start of a zone authority
        WKS = 11, //well known service description
        PTR = 12, //domain name pointer
        HINFO = 13, //host information
        MINFO = 14, //mailbox or mail list information
        MX = 15, //mail exchange
        TXT = 16, //text strings
        AAAA = 28 // ipv6 host address
    }

    /// <summary>
    ///     The DNS CLASS (RFC1035 3.2.4/5)
    ///     Internet will be the one we'll be using (IN), the others are for completeness
    ///     https://tools.ietf.org/html/rfc1035#section-3.2.4
    /// </summary>
    public enum DnsClass
    {
        None = 0, //INVALID
        IN = 1, //Internet
        CS = 2, //OBSOLETE
        CH = 3, //CHAOS
        HS = 4 //HESIOD
    }

    /// <summary>
    ///     (RFC1035 4.1.1) These are the return codes the server can send back
    /// </summary>
    public enum ReturnCode
    {
        Success = 0,
        FormatError = 1,
        ServerFailure = 2,
        NameError = 3,
        NotImplemented = 4,
        Refused = 5,
        Other = 6
    }

    /// <summary>
    ///     (RFC1035 4.1.1) These are the Query Types which apply to all questions in a request
    /// </summary>
    public enum Opcode
    {
        StandardQuery = 0,
        InverseQuery = 1,
        StatusRequest = 2,
        Reserved3 = 3,
        Reserved4 = 4,
        Reserved5 = 5,
        Reserved6 = 6,
        Reserved7 = 7,
        Reserved8 = 8,
        Reserved9 = 9,
        Reserved10 = 10,
        Reserved11 = 11,
        Reserved12 = 12,
        Reserved13 = 13,
        Reserved14 = 14,
        Reserved15 = 15
    }
}