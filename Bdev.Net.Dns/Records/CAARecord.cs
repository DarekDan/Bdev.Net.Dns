using System;
using System.Collections.Generic;
using System.Text;

namespace Bdev.Net.Dns.Records
{
    /// <summary>
    ///     A CAA Resource Record (RR) (rfc8659 / rfc8657)
    /// </summary>
    public class CAARecord : RecordBase, IEquatable<CAARecord>
    {
        /// <summary>
        ///     Constructs a CAA record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        /// <param name="recordLength">The length of the record in bytes</param>
        internal CAARecord(Pointer pointer, int recordLength)
        {
            Flags = pointer.ReadByte();
            var tagLength = pointer.ReadByte();
            Tag = Encoding.ASCII.GetString(pointer.ReadBytes(tagLength));
            Value = Encoding.ASCII.GetString(pointer.ReadBytes(recordLength - 2 - tagLength));
        }

        /// <summary>
        ///     Gets the flags of the record (bit 0 is the issuer critical flag)
        /// </summary>
        public byte Flags { get; }

        /// <summary>
        ///     Gets the property tag (e.g., "issue", "issuewild", "iodef")
        /// </summary>
        public string Tag { get; }

        /// <summary>
        ///     Gets the property value (e.g., CA domain name or reporting URL)
        /// </summary>
        public string Value { get; }

        /// <summary>
        ///     Gets whether the issuer critical flag is set
        /// </summary>
        public bool IssuerCritical => (Flags & 0x80) != 0;

        public override string ToString()
        {
            return $"Flags = {Flags}, Tag = {Tag}, Value = {Value}";
        }

        #region IEquatable Members

        public bool Equals(CAARecord other)
        {
            return other != null &&
                Flags == other.Flags &&
                Tag == other.Tag &&
                Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CAARecord);
        }

        public override int GetHashCode()
        {
            var hashCode = 1152426255;
            hashCode = hashCode * -1521134295 + Flags.GetHashCode();
            hashCode = hashCode * -1521134295 + (Tag?.GetHashCode() ?? 0);
            hashCode = hashCode * -1521134295 + (Value?.GetHashCode() ?? 0);
            return hashCode;
        }

        public static bool operator ==(CAARecord record1, CAARecord record2)
        {
            return EqualityComparer<CAARecord>.Default.Equals(record1, record2);
        }

        public static bool operator !=(CAARecord record1, CAARecord record2)
        {
            return !(record1 == record2);
        }

        #endregion
    }
}