using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bdev.Net.Dns.Records
{
    /// <summary>
    ///     An DS Resource Record (RR) (RFC4034 5.1)
    /// </summary>
    public class DSRecord : RecordBase, IEquatable<DSRecord>
    {
        /// <summary>
        ///     Constructs an DS record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        /// <param name="recordLength">The length of the record in bytes</param>
        internal DSRecord(Pointer pointer, int recordLength)
        {
            KeyTag = pointer.ReadUShort();
            Algorithm = (DnsSecAlgorithm)pointer.ReadByte();
            DigestType = (DnsSecDigestType)pointer.ReadByte();
            Digest = pointer.ReadBytes(recordLength - 4);
        }

        /// <summary>
        ///     Gets the key tag of the record
        /// </summary>
        public int KeyTag { get; }

        /// <summary>
        ///     Gets the algorithm of the record
        /// </summary>
        public DnsSecAlgorithm Algorithm { get; }

        /// <summary>
        ///     Gets the digest type of the record
        /// </summary>
        public DnsSecDigestType DigestType { get; }

        /// <summary>
        ///     Gets the digest of the record
        /// </summary>
        public IReadOnlyList<byte> Digest { get; }

        public override string ToString()
        {
            var digest = string.Concat(Digest.Select(b => b.ToString("X2")));

            return $"Key Tag = {KeyTag}, Algorithm = {Algorithm}, Digest Type = {DigestType}, Digest = {digest}";
        }

        #region IEquatable Members

        public bool Equals(DSRecord other)
        {
            return other != null &&
                KeyTag == other.KeyTag &&
                Algorithm == other.Algorithm &&
                DigestType == other.DigestType &&
                Digest.SequenceEqual(other.Digest);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DSRecord);
        }

        public override int GetHashCode()
        {
            var hashCode = 1152426255;
            hashCode = hashCode * -1521134295 + KeyTag.GetHashCode();
            hashCode = hashCode * -1521134295 + Algorithm.GetHashCode();
            hashCode = hashCode * -1521134295 + DigestType.GetHashCode();
            hashCode = hashCode * -1521134295 + ((IStructuralEquatable)Digest).GetHashCode(EqualityComparer<byte>.Default);
            return hashCode;
        }

        public static bool operator ==(DSRecord record1, DSRecord record2)
        {
            return EqualityComparer<DSRecord>.Default.Equals(record1, record2);
        }

        public static bool operator !=(DSRecord record1, DSRecord record2)
        {
            return !(record1 == record2);
        }

        #endregion
    }
}
