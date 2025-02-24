using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Bdev.Net.Dns.Records;

namespace Bdev.Net.Dns.Helpers
{
    public class DnsServers
    {
        public static IEnumerable<IPAddress> IP4 => All.Where(w => w.AddressFamily.Equals(AddressFamily.InterNetwork));

        public static IEnumerable<IPAddress> IP6 =>
            All.Where(w => w.AddressFamily.Equals(AddressFamily.InterNetworkV6));

        public static IEnumerable<IPAddress> All => GetAliveNetworks()
            .SelectMany(s1 => s1.GetIPProperties().DnsAddresses)
            .Distinct();

        private static IEnumerable<NetworkInterface> GetAliveNetworks()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(w1 => w1.OperationalStatus.Equals(OperationalStatus.Up));
        }

        private static readonly Dictionary<Type,DnsType> RecordTypeToDnsTypeMapper = new Dictionary<Type, DnsType>()
        {
            {typeof(ANameRecord), DnsType.A},
            {typeof(CNameRecord), DnsType.CNAME},
            {typeof(MXRecord), DnsType.MX},
            {typeof(NSRecord), DnsType.NS},
            {typeof(SoaRecord), DnsType.SOA},
            {typeof(TXTRecord), DnsType.TXT},
            {typeof(AaaaRecord), DnsType.AAAA},
        };

        public static IEnumerable<ANameRecord> Resolve(string name)
        {
            return Resolve<ANameRecord>(name);
        }
        public static IEnumerable<T> Resolve<T>(string name)
        {
            if(!RecordTypeToDnsTypeMapper.ContainsKey(typeof(T))) throw new NotImplementedException("This record type has not yet been implemented");
            return Resolve<T>(name, RecordTypeToDnsTypeMapper[typeof(T)], DnsClass.IN);
        }

        public static IEnumerable<T> Resolve<T>(string name, DnsType type, DnsClass @class, bool recursionDesired = true)
        {
            var req = new Request(){RecursionDesired = recursionDesired};
            req.AddQuestion(new Question(name, type, @class));
            var bag = new ConcurrentBag<RecordBase>();
            Parallel.ForEach(IP4, server =>
            {
                try
                {
                    var res = Resolver.Lookup(req, server);
                    if (res.ReturnCode.Equals(ReturnCode.Success))
                        Parallel.ForEach(res.Answers, answer => bag.Add(answer.Record));
                }
                catch (Exception)
                {
                    //sink it, server might be dead or down
                }
            });
            return bag.Cast<T>().Distinct().ToList();
        }
    }
}