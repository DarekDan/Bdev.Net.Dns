#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
//

#endregion

namespace Bdev.Net.Dns.Records
{
    /// <summary>ANAME Resource Record (RR) (RFC1035 3.4.1).</summary>
    public record ANameRecord : RecordBase // An ANAME records consists simply of an IP address.
    {
        /// <summary>Constructs an ANAME record by reading bytes from a return message.</summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record.</param>
        internal ANameRecord(Pointer pointer)
        {
            var b1 = pointer.ReadByte();
            var b2 = pointer.ReadByte();
            var b3 = pointer.ReadByte();
            var b4 = pointer.ReadByte();

            // this next line's not brilliant - couldn't find a better way though
            IPAddress = IPAddress.Parse($"{b1}.{b2}.{b3}.{b4}");
        }

        // expose this IP address r/o to the world
        public IPAddress IPAddress { get; }

        public override string ToString() => IPAddress.ToString();
    }
}
