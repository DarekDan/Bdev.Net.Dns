#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
//

#endregion

namespace Bdev.Net.Dns.Records
{
    /// <summary>An SOA Resource Record (RR) (RFC1035 3.3.13).</summary>
    public record SOARecord : RecordBase // These fields constitute an SOA RR.
    {
        /// <summary>Constructs an SOA record by reading bytes from a return message.</summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record.</param>
        internal SOARecord(Pointer pointer)
        {
            // read all fields RFC1035 3.3.13
            PrimaryNameServer = pointer.ReadDomain();
            ResponsibleMailAddress = pointer.ReadDomain();
            Serial = pointer.ReadInt();
            Refresh = pointer.ReadInt();
            Retry = pointer.ReadInt();
            Expire = pointer.ReadInt();
            DefaultTtl = pointer.ReadInt();
        }

        // expose these fields r/o to the world
        public string PrimaryNameServer { get; }
        public string ResponsibleMailAddress { get; }
        public int Serial { get; }
        public int Refresh { get; }
        public int Retry { get; }
        public int Expire { get; }
        public int DefaultTtl { get; }

        public override string ToString() => $"""
            primary name server = {PrimaryNameServer}
            responsible mail addr = {ResponsibleMailAddress}
            serial  = {Serial}
            refresh = {Refresh}
            retry   = {Retry}
            expire  = {Expire}
            default TTL = {DefaultTtl}
            """;
    }
}
