using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

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

        public static IEnumerable<ANameRecord> Resolve(string name)
        {
            return Resolve<ANameRecord>(name, DnsType.ANAME, DnsClass.IN);
        }

        public static IEnumerable<T> Resolve<T>(string name, DnsType type, DnsClass @class)
        {
            var req = new Request();
            req.AddQuestion(new Question(name, type, @class));
            var bag = new ConcurrentBag<RecordBase>();
            Parallel.ForEach(IP4, server =>
            {
                var res = Resolver.Lookup(req, server);
                if (res.ReturnCode.Equals(ReturnCode.Success))
                    Parallel.ForEach(res.Answers, answer => bag.Add(answer.Record));
            });
            return bag.Cast<T>().ToList().Distinct();
        }
    }
}