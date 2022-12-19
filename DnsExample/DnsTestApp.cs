using System.Net;
using Bdev.Net.Dns;
using Bdev.Net.Dns.Helpers;

IPAddress dnsServer = DnsServers.IP4.First();

Console.Write("Please enter the address of the DNS server to query (or hit enter for first IP4 available): ");
var ip = Console.ReadLine();

try { if (!string.IsNullOrWhiteSpace(ip)) dnsServer = IPAddress.Parse(ip); }
catch { Console.WriteLine("Not a valid address, using first IP4 available."); }

Console.WriteLine($"DNS Query Tool using '{dnsServer}', type 'quit' to exit.");

while (true)
{
    Console.Write("> "); var domain = Console.ReadLine();

    // Break out on no input or the quit command.
    if (string.IsNullOrWhiteSpace(domain) || string.Equals(domain, "quit", StringComparison.InvariantCultureIgnoreCase)) break;

    // Show information.
    Console.WriteLine($"Querying DNS records for domain: '{domain}'.");

    // Query AName, MX, NS, SOA.
    Query(dnsServer, domain, DnsType.ANAME);
    Query(dnsServer, domain, DnsType.MX);
    Query(dnsServer, domain, DnsType.NS);
    Query(dnsServer, domain, DnsType.SOA);
}

void Query(IPAddress dnsServer, string domain, DnsType type)
{
    try
    {
        var request = new Request();                         // Create a DNS request.
        request.AddQuestion(new Question(domain, type));    // Create a question for this domain and DNS CLASS.
        var response = Resolver.Lookup(request, dnsServer); // Send it to the DNS server and get the response.

        Console.WriteLine("--------------------------------------------------------------");        // Display each RR returned and whether
        Console.WriteLine($"{(response.AuthoritativeAnswer ? "A" : "Non-a")}uthoritative answer:"); // this is an authoritative answer or not.

        foreach (var answer in response.Answers)                        // Dump all the records - answers / name servers / additional records.
            Console.WriteLine($"{answer.Type} ({answer.Domain}) : {answer.Record}");

        foreach (var nameServer in response.NameServers)
            Console.WriteLine($"{nameServer.Type} ({nameServer.Domain}) : {nameServer.Record}");

        foreach (var additionalRecord in response.AdditionalRecords)
            Console.WriteLine($"{additionalRecord.Type} ({additionalRecord.Domain}) : {additionalRecord.Record}");
    }
    // catch (NoResponseException ex) { Console.WriteLine(ex.Message); } // Kind of redundant...
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}
