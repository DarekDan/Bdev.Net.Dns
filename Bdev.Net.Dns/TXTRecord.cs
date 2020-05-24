using System;
using System.Text;

namespace Bdev.Net.Dns
{
    public class TXTRecord : RecordBase
    {
        public string Value { get; set; }

        public int Length { get; set; }
        internal TXTRecord(Pointer pointer)
        {
            Length = pointer.ReadByte();
            var sb = new StringBuilder(Length);
            for (int i = 0; i < Length; i++)
            {
                sb.Append(pointer.ReadChar());
            }
            Value = sb.ToString();
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }

    public class CNameRecord : RecordBase
    {
        public string Value { get; set; }

        public int Length { get; set; }
        internal CNameRecord(Pointer pointer)
        {
            Length = pointer.ReadByte();
            var sb = new StringBuilder(Length);
            for (int i = 0; i < Length; i++)
            {
                sb.Append(pointer.ReadChar());
            }
            Value = sb.ToString();
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}