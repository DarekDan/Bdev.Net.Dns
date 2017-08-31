#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
// 

#endregion

using System;
using System.Collections.Generic;

namespace Bdev.Net.Dns
{
    /// <summary>
    ///     An SOA Resource Record (RR) (RFC1035 3.3.13)
    /// </summary>
    public class SoaRecord : RecordBase, IEquatable<SoaRecord>
    {
        // these fields constitute an SOA RR
        private readonly int _defaultTtl;
        private readonly int _expire;
        private readonly string _primaryNameServer;
        private readonly int _refresh;
        private readonly string _responsibleMailAddress;
        private readonly int _retry;
        private readonly int _serial;

        /// <summary>
        ///     Constructs an SOA record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal SoaRecord(Pointer pointer)
        {
            // read all fields RFC1035 3.3.13
            _primaryNameServer = pointer.ReadDomain();
            _responsibleMailAddress = pointer.ReadDomain();
            _serial = pointer.ReadInt();
            _refresh = pointer.ReadInt();
            _retry = pointer.ReadInt();
            _expire = pointer.ReadInt();
            _defaultTtl = pointer.ReadInt();
        }

        // expose these fields public read/only
        public string PrimaryNameServer => _primaryNameServer;

        public string ResponsibleMailAddress => _responsibleMailAddress;

        public int Serial => _serial;

        public int Refresh => _refresh;

        public int Retry => _retry;

        public int Expire => _expire;

        public int DefaultTtl => _defaultTtl;

        public override bool Equals(object obj)
        {
            return Equals(obj as SoaRecord);
        }

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
                string.Format(
                    "primary name server = {0}\nresponsible mail addr = {1}\nserial  = {2}\nrefresh = {3}\nretry   = {4}\nexpire  = {5}\ndefault TTL = {6}",
                    _primaryNameServer,
                    _responsibleMailAddress,
                    _serial,
                    _refresh,
                    _retry,
                    _expire,
                    _defaultTtl);
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