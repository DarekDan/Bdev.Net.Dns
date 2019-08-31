#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
// 

#endregion

using System;
using System.Collections.Generic;

namespace Bdev.Net.Dns
{
    /// <summary>
    ///     A Name Server Resource Record (RR) (RFC1035 3.3.11)
    /// </summary>
    public class NSRecord : RecordBase, IEquatable<NSRecord>
    {
        // the fields exposed outside the assembly

        // expose this domain name address r/o to the world

        /// <summary>
        ///     Constructs a NS record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal NSRecord(Pointer pointer)
        {
            DomainName = pointer.ReadDomain();
        }

        public string DomainName { get; }

        public bool Equals(NSRecord other)
        {
            return other != null &&
                   DomainName == other.DomainName;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NSRecord);
        }

        public override int GetHashCode()
        {
            return 1022487930 + EqualityComparer<string>.Default.GetHashCode(DomainName);
        }

        public override string ToString()
        {
            return DomainName;
        }

        public static bool operator ==(NSRecord record1, NSRecord record2)
        {
            return EqualityComparer<NSRecord>.Default.Equals(record1, record2);
        }

        public static bool operator !=(NSRecord record1, NSRecord record2)
        {
            return !(record1 == record2);
        }
    }
}