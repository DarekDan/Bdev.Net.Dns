using System;
using System.Collections.Generic;
using System.Net;

namespace Bdev.Net.Dns.Records
{
    /// <summary>
    ///     AAAA Resource Record (RR) (RFC3596 2.2)
    /// </summary>
    public class AAAARecord : RecordBase, IEquatable<AAAARecord>
    {
        /// <summary>
        ///     Constructs an AAAA record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal AAAARecord(Pointer pointer)
        {
            IPAddress = new IPAddress(pointer.ReadBytes(16));
        }

        /// <summary>
        /// Gets the IPv6 address of the record
        /// </summary>
        public IPAddress IPAddress { get; }

        public bool Equals(AAAARecord other)
        {
            return other != null &&
                   EqualityComparer<IPAddress>.Default.Equals(IPAddress, other.IPAddress);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AAAARecord);
        }

        public override int GetHashCode()
        {
            return -2138420020 + EqualityComparer<IPAddress>.Default.GetHashCode(IPAddress);
        }

        public override string ToString()
        {
            return IPAddress.ToString();
        }

        public static bool operator ==(AAAARecord record1, AAAARecord record2)
        {
            return EqualityComparer<AAAARecord>.Default.Equals(record1, record2);
        }

        public static bool operator !=(AAAARecord record1, AAAARecord record2)
        {
            return !(record1 == record2);
        }
    }
}
