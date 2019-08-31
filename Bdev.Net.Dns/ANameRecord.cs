#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
// 

#endregion

using System;
using System.Collections.Generic;
using System.Net;

namespace Bdev.Net.Dns
{
    /// <summary>
    ///     ANAME Resource Record (RR) (RFC1035 3.4.1)
    /// </summary>
    public class ANameRecord : RecordBase, IEquatable<ANameRecord>
    {
        // An ANAME records consists simply of an IP address
        internal IPAddress _ipAddress;

        // expose this IP address r/o to the world

        /// <summary>
        ///     Constructs an ANAME record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal ANameRecord(Pointer pointer)
        {
            var b1 = pointer.ReadByte();
            var b2 = pointer.ReadByte();
            var b3 = pointer.ReadByte();
            var b4 = pointer.ReadByte();

            // this next line's not brilliant - couldn't find a better way though
            _ipAddress = IPAddress.Parse(string.Format("{0}.{1}.{2}.{3}", b1, b2, b3, b4));
        }

        public IPAddress IPAddress => _ipAddress;

        public bool Equals(ANameRecord other)
        {
            return other != null &&
                   EqualityComparer<IPAddress>.Default.Equals(IPAddress, other.IPAddress);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ANameRecord);
        }

        public override int GetHashCode()
        {
            return -2138420020 + EqualityComparer<IPAddress>.Default.GetHashCode(IPAddress);
        }

        public override string ToString()
        {
            return _ipAddress.ToString();
        }

        public static bool operator ==(ANameRecord record1, ANameRecord record2)
        {
            return EqualityComparer<ANameRecord>.Default.Equals(record1, record2);
        }

        public static bool operator !=(ANameRecord record1, ANameRecord record2)
        {
            return !(record1 == record2);
        }
    }
}