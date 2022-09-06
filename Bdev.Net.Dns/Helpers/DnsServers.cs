#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
//

#endregion

using System.Collections.Concurrent;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Bdev.Net.Dns.Records;

namespace Bdev.Net.Dns.Helpers
{
    public static class DnsServers
    {
        public static IEnumerable<IPAddress> IP4 => All.Where(w => w.AddressFamily.Equals(AddressFamily.InterNetwork));

        public static IEnumerable<IPAddress> IP6 => All.Where(w => w.AddressFamily.Equals(AddressFamily.InterNetworkV6));

        public static IEnumerable<IPAddress> All => GetAliveNetworks()
            .SelectMany(s1 => s1.GetIPProperties().DnsAddresses)
            .Distinct();

        private static IEnumerable<NetworkInterface> GetAliveNetworks() => NetworkInterface.GetAllNetworkInterfaces()
            .Where(w1 => w1.OperationalStatus.Equals(OperationalStatus.Up));

        private static readonly Dictionary<Type, DnsType> RecordTypeToDnsTypeMapper = new()
        {
            { typeof(ANameRecord), DnsType.A },
            { typeof(CNameRecord), DnsType.CNAME },
            { typeof(MXRecord), DnsType.MX },
            { typeof(NSRecord), DnsType.NS },
            { typeof(SOARecord), DnsType.SOA },
            { typeof(TXTRecord), DnsType.TXT }
        };

        public static IEnumerable<ANameRecord> Resolve(string name) => Resolve<ANameRecord>(name);
        public static IEnumerable<T> Resolve<T>(string name) => Resolve<T>(name, RecordTypeToDnsTypeMapper[typeof(T)], DnsClass.IN);
        public static IEnumerable<T> Resolve<T>(string name, DnsType type, DnsClass @class, bool recursionDesired = true)
        {
            if (!RecordTypeToDnsTypeMapper.ContainsKey(typeof(T))) throw new NotImplementedException("This record type has not yet been implemented.");

            var req = new Request() { RecursionDesired = recursionDesired }.WithQuestion(new Question(name, type, @class));
            var bag = new ConcurrentBag<RecordBase>();

            Parallel.ForEach(IP4, server =>
            {
                try
                {
                    var res = Resolver.Lookup(req, server);
                    if (res.ReturnCode.Equals(ReturnCode.Success))
                        Parallel.ForEach(res.Answers, answer => { if (answer.Record is not EmptyRecord) bag.Add(answer.Record); }); /* null */
                }
                catch { /* sink it, server might be dead or down */ }
            });

            return bag.Cast<T>().Distinct().ToList();
        }
    }
}
