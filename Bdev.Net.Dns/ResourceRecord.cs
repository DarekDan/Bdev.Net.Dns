#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
// 

#endregion

using System;
using Bdev.Net.Dns.Records;

namespace Bdev.Net.Dns
{
    /// <summary>
    ///     Represents a Resource Record as detailed in RFC1035 4.1.3
    /// </summary>
    [Serializable]
    public class ResourceRecord : IEquatable<ResourceRecord>
    {
        /// <summary>
        ///     Construct a resource record from a pointer to a byte array
        /// </summary>
        /// <param name="pointer">the position in the byte array of the record</param>
        internal ResourceRecord(Pointer pointer)
        {
            // extract the domain, question type, question class and Ttl
            Domain = pointer.ReadDomain();
            Type = (DnsType) pointer.ReadShort();
            Class = (DnsClass) pointer.ReadShort();
            Ttl = pointer.ReadInt();

            // the next short is the record length, we only use it for unrecognized record types
            int recordLength = pointer.ReadShort();

            // and create the appropriate RDATA record based on the dnsType
            switch (Type)
            {
                case DnsType.NS:
                    Record = new NSRecord(pointer);
                    break;
                case DnsType.MX:
                    Record = new MXRecord(pointer);
                    break;
                case DnsType.ANAME:
                    Record = new ANameRecord(pointer);
                    break;
                case DnsType.CNAME:
                    Record = new CNameRecord(pointer);
                    break;
                case DnsType.SOA:
                    Record = new SoaRecord(pointer);
                    break;
                case DnsType.TXT:
                    Record = new TXTRecord(pointer, recordLength);
                    break;
                case DnsType.AAAA:
                    Record = new AaaaRecord(pointer);
                    break;
                default:
                {
                    // move the pointer over this unrecognized record
                    pointer.Seek(recordLength);
                    break;
                }
            }
        }

        // read only properties applicable for all records
        public string Domain { get; }

        public DnsType Type { get; }

        public DnsClass Class { get; }

        public int Ttl { get; }

        public RecordBase Record { get; }

        public bool Equals(ResourceRecord other)
        {
            return other != null && Type.Equals(other.Type) && Class.Equals(other.Class) &&
                   Domain.Equals(other.Domain) &&
                   Record.Equals(other.Record);
        }
    }

    // Answers, Name Servers and Additional Records all share the same RR format
    [Serializable]
    public class Answer : ResourceRecord
    {
        internal Answer(Pointer pointer) : base(pointer)
        {
        }
    }

    [Serializable]
    public class NameServer : ResourceRecord
    {
        internal NameServer(Pointer pointer) : base(pointer)
        {
        }
    }

    [Serializable]
    public class AdditionalRecord : ResourceRecord
    {
        internal AdditionalRecord(Pointer pointer) : base(pointer)
        {
        }
    }
}