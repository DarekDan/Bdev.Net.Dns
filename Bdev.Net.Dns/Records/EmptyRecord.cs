#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
//

#endregion

namespace Bdev.Net.Dns.Records
{
    /// <summary>Empty record as a replacement for 'null'.</summary>
    public record EmptyRecord : RecordBase
    {
        /// <summary>Constructs an empty record to signal null or error.</summary>
        internal EmptyRecord() { }

        public override string ToString() => "n/a";
    }
}
