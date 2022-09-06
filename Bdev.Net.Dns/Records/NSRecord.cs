#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
//

#endregion

namespace Bdev.Net.Dns.Records
{
    /// <summary>A Name Server Resource Record (RR) (RFC1035 3.3.11).</summary>
    public record NSRecord : RecordBase
    {
        /// <summary>Constructs a NS record by reading bytes from a return message.</summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record.</param>
        internal NSRecord(Pointer pointer) => DomainName = pointer.ReadDomain();

        // expose this domain name r/o to the world
        public string DomainName { get; }

        public override string ToString() => DomainName;
    }
}
