using System;
using System.Collections.Generic;
using System.Text;

namespace Bdev.Net.Dns.Records
{
    [Serializable]
    public class TXTRecord : RecordBase, IComparable, IEquatable<TXTRecord>
    {
        public string Value { get; }

        public int Length { get; }
        internal TXTRecord(Pointer pointer, int recordLength)
        {
            var position = pointer.Position;

            var sb = new StringBuilder(recordLength);

            // there can be multiple strings in one TXT record
            // loop until full recordLength is read
            while (pointer.Position - position < recordLength)
            {
                // read the string length
                var length = pointer.ReadByte();
                for (int i = 0; i < length; i++)
                {
                    sb.Append(pointer.ReadChar());
                }
            }

            Value = sb.ToString();
            Length = sb.Length;
        }

        public bool Equals(TXTRecord other)
        {
            return other != null && this.Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != this.GetType()) return false;
            return Equals(obj as TXTRecord);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<string>.Default.GetHashCode(Value);
        }

        public override string ToString()
        {
            return $"{Value}";
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (obj is TXTRecord otherRecord)
            {
                return string.Compare(this.Value, otherRecord.Value, StringComparison.Ordinal);
            }
            else
            {
                throw new ArgumentException("Object is not a TXTRecord");
            }
        }
    }
}