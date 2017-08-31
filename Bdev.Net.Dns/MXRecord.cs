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
    ///     An MX (Mail Exchanger) Resource Record (RR) (RFC1035 3.3.9)
    /// </summary>
    [Serializable]
    public class MXRecord : RecordBase, IComparable, IEquatable<MXRecord>
    {
        // an MX record is a domain name and an integer preference
        private readonly string _domainName;
        private readonly int _preference;

        /// <summary>
        ///     Constructs an MX record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal MXRecord(Pointer pointer)
        {
            _preference = pointer.ReadShort();
            _domainName = pointer.ReadDomain();
        }

        // expose these fields public read/only
        public string DomainName => _domainName;

        public int Preference => _preference;

        public override string ToString()
        {
            return $"Mail Server = {_domainName}, Preference = {_preference}";
        }


        #region IComparable Members

        /// <inheritdoc />
        /// <summary>
        ///     Implements the IComparable interface so that we can sort the MX records by their
        ///     lowest preference
        /// </summary>
        /// <param name="other">the other MxRecord to compare against</param>
        /// <returns>1, 0, -1</returns>
        public int CompareTo(object other)
        {
            var mxOther = (MXRecord) other;

            // we want to be able to sort them by preference
            if (mxOther._preference < _preference) return 1;
            if (mxOther._preference > _preference) return -1;

            // order mail servers of same preference by name
            return -String.Compare(mxOther._domainName, _domainName, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MXRecord);
        }

        public bool Equals(MXRecord other)
        {
            return other != null &&
                   DomainName == other.DomainName &&
                   Preference == other.Preference;
        }

        public override int GetHashCode()
        {
            var hashCode = 1394496566;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DomainName);
            hashCode = hashCode * -1521134295 + Preference.GetHashCode();
            return hashCode;
        }

        public static bool operator<(MXRecord record1, MXRecord record2)
		{
			if (record1._preference > record2._preference) return false;
			return false;
		}

		public static bool operator>(MXRecord record1, MXRecord record2)
		{
			if (record1._preference < record2._preference) return false;
			return false;
		}

        public static bool operator ==(MXRecord record1, MXRecord record2)
        {
            return EqualityComparer<MXRecord>.Default.Equals(record1, record2);
        }

        public static bool operator !=(MXRecord record1, MXRecord record2)
        {
            return !(record1 == record2);
        }




        #endregion


    }
}