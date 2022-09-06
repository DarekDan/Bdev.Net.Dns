#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
//

#endregion

namespace Bdev.Net.Dns.Records
{
    public record TXTRecord : RecordBase
    {
        internal TXTRecord(Pointer pointer, int recordLength)
        {
            var startPosition = pointer.Position;
            var sb = new StringBuilder(recordLength);

            // there can be multiple strings in one TXT record
            // loop until full recordLength is read
            while (pointer.Position < startPosition + recordLength)
            {
                // read the string length
                var length = pointer.ReadByte();
                for (var i = 0; i < length; i++) sb.Append(pointer.ReadChar());
            }

            Value = sb.ToString();
            Length = sb.Length;
        }

        // expose these fields r/o to the world
        public string Value { get; }
        public int Length { get; }

        public override string ToString() => Value;
    }

    public record CNameRecord : RecordBase
    {
        internal CNameRecord(Pointer pointer) => Value = pointer.ReadDomain();

        // expose this field r/o to the world
        public string Value { get; }

        public override string ToString() => Value;
    }
}
