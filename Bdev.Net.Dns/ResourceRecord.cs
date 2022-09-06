#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
//

#endregion

using Bdev.Net.Dns.Records;

namespace Bdev.Net.Dns
{
    /// <summary>Represents a Resource Record as detailed in RFC1035 4.1.3.</summary>
    [Serializable]
    public record ResourceRecord
    {
        /// <summary>Construct a resource record from a pointer to a byte array.</summary>
        /// <param name="pointer">the position in the byte array of the record.</param>
        internal ResourceRecord(Pointer pointer)
        {
            // extract the domain, question type, question class and Ttl
            Domain = pointer.ReadDomain();
            Type = (DnsType)pointer.ReadShort();
            Class = (DnsClass)pointer.ReadShort();
            Ttl = pointer.ReadInt();

            // the next short is the record length, we only use it for unrecognized record types
            int recordLength = pointer.ReadShort();

            // and create the appropriate RDATA record based on the dnsType
            Record = Type switch
            {
                DnsType.NS => new NSRecord(pointer),
                DnsType.MX => new MXRecord(pointer),
                DnsType.ANAME => new ANameRecord(pointer),
                DnsType.CNAME => new CNameRecord(pointer),
                DnsType.SOA => new SOARecord(pointer),
                DnsType.TXT => new TXTRecord(pointer, recordLength),
                _ => new EmptyRecord() /* null */
            };

            if (Record is EmptyRecord) pointer.Seek(recordLength); // move the pointer over this unrecognized record /* null */
        }

        // read only properties applicable for all records
        public string Domain { get; }
        public DnsType Type { get; }
        public DnsClass Class { get; }
        public int Ttl { get; }
        public RecordBase Record { get; } // = new EmptyRecord(); /* null */
    }

    // Answers, Name Servers and Additional Records all share the same RR format.
    [Serializable]
    public record Answer : ResourceRecord
    {
        internal Answer(Pointer pointer) : base(pointer) { }
    }

    [Serializable]
    public record NameServer : ResourceRecord
    {
        internal NameServer(Pointer pointer) : base(pointer) { }
    }

    [Serializable]
    public record AdditionalRecord : ResourceRecord
    {
        internal AdditionalRecord(Pointer pointer) : base(pointer) { }
    }
}
