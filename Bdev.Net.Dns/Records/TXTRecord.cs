using System.Text;

namespace Bdev.Net.Dns.Records
{
    public record TXTRecord : RecordBase
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
                for (var i = 0; i < length; i++) sb.Append(pointer.ReadChar());
            }

            Value = sb.ToString();
            Length = sb.Length;
        }

        public override string ToString() => Value;
    }

    public record CNameRecord : RecordBase
    {
        public string Value { get; }

        internal CNameRecord(Pointer pointer) => Value = pointer.ReadDomain();

        public override string ToString() => Value;
    }
}
