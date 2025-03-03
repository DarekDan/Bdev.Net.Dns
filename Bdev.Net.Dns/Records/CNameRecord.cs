using System;

namespace Bdev.Net.Dns.Records
{
    public class CNameRecord : RecordBase, IComparable, IEquatable<CNameRecord>
    {
        public string Value { get; }

        internal CNameRecord(Pointer pointer)
        {
            Value = pointer.ReadDomain();
        }

        public bool Equals(CNameRecord other)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"{Value}";
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
}