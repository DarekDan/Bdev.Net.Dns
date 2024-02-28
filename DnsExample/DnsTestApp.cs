using System;
using System.Linq;
using System.Net;
using Bdev.Net.Dns;
using Bdev.Net.Dns.Helpers;

namespace DnsExample
{
    internal class DnsTestApp
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Console.Write("Please enter the address of the DNS server to query (or hit enter for first IP4 available): ");
            var ip = Console.ReadLine();

            var dnsServer = String.IsNullOrWhiteSpace(ip) ? DnsServers.IP4.First() : IPAddress.Parse(ip);
            Console.WriteLine("DNS Query Tool, type 'quit' to exit");

            while (true)
            {
                Console.Write(">");
                var domain = Console.ReadLine();

                // break out on quit command
                if (domain?.ToLower() == "quit") break;

                // Information
                Console.WriteLine("Querying DNS records for domain: " + domain);

                // query AName, MX, NS, SOA
                Query(dnsServer, domain, DnsType.A);
                Query(dnsServer, domain, DnsType.AAAA);
                Query(dnsServer, domain, DnsType.MX);
                Query(dnsServer, domain, DnsType.NS);
                Query(dnsServer, domain, DnsType.SOA);
            }
        }

        private static void Query(IPAddress dnsServer, string domain, DnsType type)
        {
            try
            {
                // create a DNS request
                var request = new Request();

                // create a question for this domain and DNS CLASS
                request.AddQuestion(new Question(domain, type));

                // send it to the DNS server and get the response
                var response = Resolver.Lookup(request, dnsServer);

                // check we have a response
                if (response == null)
                {
                    Console.WriteLine("No answer");
                    return;
                }

                // display each RR returned
                Console.WriteLine("--------------------------------------------------------------");

                // display whether this is an authoritative answer or not
                Console.WriteLine(response.AuthoritativeAnswer ? "authoritative answer" : "Non-authoritative answer");

                // Dump all the records - answers/name servers/additional records
                foreach (var answer in response.Answers)
                    Console.WriteLine("{0} ({1}) : {2}", answer.Type, answer.Domain, answer.Record);

                foreach (var nameServer in response.NameServers)
                    Console.WriteLine("{0} ({1}) : {2}", nameServer.Type, nameServer.Domain, nameServer.Record);

                foreach (var additionalRecord in response.AdditionalRecords)
                    Console.WriteLine("{0} ({1}) : {2}", additionalRecord.Type, additionalRecord.Domain,
                        additionalRecord.Record);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}