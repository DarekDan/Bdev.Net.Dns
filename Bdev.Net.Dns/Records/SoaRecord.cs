#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
// 

#endregion

using System;
using System.Collections.Generic;

namespace Bdev.Net.Dns.Records
{
    /// <summary>
    ///     An SOA Resource Record (RR) (RFC1035 3.3.13)
    /// </summary>
    public class SoaRecord : RecordBase, IEquatable<SoaRecord>
    {
        // these fields constitute an SOA RR

        /// <summary>
        ///     Constructs an SOA record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal SoaRecord(Pointer pointer)
        {
            // read all fields RFC1035 3.3.13
            PrimaryNameServer = pointer.ReadDomain();
            ResponsibleMailAddress = pointer.ReadDomain();
            Serial = pointer.ReadInt();
            Refresh = pointer.ReadInt();
            Retry = pointer.ReadInt();
            Expire = pointer.ReadInt();
            DefaultTtl = pointer.ReadInt();
        }

        // expose these fields public read/only
        public string PrimaryNameServer { get; }

        public string ResponsibleMailAddress { get; }

        public int Serial { get; }

        public int Refresh { get; }

        public int Retry { get; }

        public int Expire { get; }

        public int DefaultTtl { get; }

        public bool Equals(SoaRecord other)
        {
            return other != null &&
                   PrimaryNameServer == other.PrimaryNameServer &&
                   ResponsibleMailAddress == other.ResponsibleMailAddress &&
                   Serial == other.Serial &&
                   Refresh == other.Refresh &&
                   Retry == other.Retry &&
                   Expire == other.Expire &&
                   DefaultTtl == other.DefaultTtl;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SoaRecord);
        }

        public override int GetHashCode()
        {
            var hashCode = 1152426255;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PrimaryNameServer);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ResponsibleMailAddress);
            hashCode = hashCode * -1521134295 + Serial.GetHashCode();
            hashCode = hashCode * -1521134295 + Refresh.GetHashCode();
            hashCode = hashCode * -1521134295 + Retry.GetHashCode();
            hashCode = hashCode * -1521134295 + Expire.GetHashCode();
            hashCode = hashCode * -1521134295 + DefaultTtl.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return
                $"primary name server = {PrimaryNameServer}\nresponsible mail addr = {ResponsibleMailAddress}\nserial  = {Serial}\nrefresh = {Refresh}\nretry   = {Retry}\nexpire  = {Expire}\ndefault TTL = {DefaultTtl}";
        }

        public static bool operator ==(SoaRecord record1, SoaRecord record2)
        {
            return EqualityComparer<SoaRecord>.Default.Equals(record1, record2);
        }

        public static bool operator !=(SoaRecord record1, SoaRecord record2)
        {
            return !(record1 == record2);
        }
    }
}