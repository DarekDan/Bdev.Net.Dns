using System.Text;

namespace Bdev.Net.Dns.Records
{
    public class TXTRecord : RecordBase
    {
        public string Value { get; set; }

        public int Length { get; set; }
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