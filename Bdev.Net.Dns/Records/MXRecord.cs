#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
//

#endregion

namespace Bdev.Net.Dns.Records
{
    /// <summary>An MX (Mail Exchanger) Resource Record (RR) (RFC1035 3.3.9).</summary>
    [Serializable]
    public record MXRecord : RecordBase, IComparable // An MX record is a domain name and an integer preference.
    {
        /// <summary>Constructs an MX record by reading bytes from a return message.</summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record.</param>
        internal MXRecord(Pointer pointer)
        {
            Preference = pointer.ReadShort();
            DomainName = pointer.ReadDomain();
        }

        // expose these fields r/o to the world
        public int Preference { get; }
        public string DomainName { get; }

        public override string ToString() => $"Mail Server = {DomainName}, Preference = {Preference}";

        #region IComparable Members

        /// <inheritdoc />
        /// <summary>
        ///     Implements the IComparable interface so that we can sort the MX records by their
        ///     lowest preference.
        /// </summary>
        /// <param name="other">the other MxRecord to compare against.</param>
        /// <returns>1, 0, -1 ('this' greater, equal or less than 'other').</returns>
        public int CompareTo(object? other) // BUG: This does not work, at all...
        {
            var mxOther = other as MXRecord;

            // TODO: Early return if other is null, what value or throw?

            // we want to be able to sort them by preference
            if (mxOther?.Preference < Preference) return 1;
            if (mxOther?.Preference > Preference) return -1;

            // order mail servers of same preference by name
            return -string.CompareOrdinal(mxOther?.DomainName, DomainName);
        }

        public static bool operator <(MXRecord record1, MXRecord record2)
        {
            if (record1.Preference > record2.Preference) return false; // BUG: Check why always return false..!!?
            return false;
        }

        public static bool operator >(MXRecord record1, MXRecord record2)
        {
            if (record1.Preference < record2.Preference) return false; // BUG: Check why always return false..!!?
            return false;
        }

        #endregion
    }
}
