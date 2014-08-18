using System;
using System.Net;
using Bdev.Net.Dns;

namespace DnsExample
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;

    class DnsTestApp
	{
		[STAThread]
		static void Main(string[] args)
		{
            Query(IPAddress.Parse("208.67.222.222"),"bisoftware.com", DnsType.MX);
            Query("bisoftware.com", DnsType.MX);

			// I'd love to know if there a programmatic way of retreiving the computer's default DNS servers
			// in managed code, if you how to do this, please let me know at rob@bigdevelopments.co.uk so I
			// can update the code, I have searched fruitlessly for sometime for a simple answer to this
			//
			// in the meantime - we are going to have to ask for it :(
			//
			Console.Write("Please enter the address of the DNS server to query: ");
			string ip = Console.ReadLine();

			IPAddress dnsServer = IPAddress.Parse(ip);
			Console.WriteLine("DNS Query Tool, type 'quit' to exit");

			while (true)
			{
				Console.Write(">");
				string domain = Console.ReadLine();
				
				// break out on quit command
				if (domain.ToLower() == "quit") break;

				// Information
				Console.WriteLine("Querying DNS records for domain: " + domain);
				
				// query AName, MX, NS, SOA
				Query(dnsServer, domain, DnsType.ANAME);
				Query(dnsServer, domain, DnsType.MX);
				Query(dnsServer, domain, DnsType.NS);
				Query(dnsServer, domain, DnsType.SOA);
			}
		}


        private static void Query(string domain, DnsType type)
        {
            IEnumerable<IPAddress> dnsServers =
                NetworkInterface.GetAllNetworkInterfaces()
                    .SelectMany(s => s.GetIPProperties().DnsAddresses)
                    .Where(w => w.AddressFamily.Equals(AddressFamily.InterNetwork))
                    .Distinct()
                    .ToList();
            var isValid = dnsServers.Any(
                a =>
                {
                    try
                    {
                        var mx = Resolver.MXLookup(domain, a);
                        return mx.Any();
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });
            Console.WriteLine(isValid);
        }

		private static void Query(IPAddress dnsServer, string domain, DnsType type)
		{
			try
			{
				// create a DNS request
				Request request = new Request();

				// create a question for this domain and DNS CLASS
				request.AddQuestion(new Question(domain, type, DnsClass.IN));
						
				// send it to the DNS server and get the response
				Response response = Resolver.Lookup(request, dnsServer);

				// check we have a response
				if (response == null)
				{
					Console.WriteLine("No answer");
					return;

				}
				// display each RR returned
				Console.WriteLine("--------------------------------------------------------------");

				// display whether this is an authoritative answer or not
				if (response.AuthoritativeAnswer)
				{
					Console.WriteLine("authoritative answer");
				}
				else
				{
					Console.WriteLine("Non-authoritative answer");
				}

				// Dump all the records - answers/name servers/additional records
				foreach (Answer answer in response.Answers)
				{
					Console.WriteLine("{0} ({1}) : {2}", answer.Type.ToString(), answer.Domain, answer.Record.ToString());
				}

				foreach (NameServer nameServer in response.NameServers)
				{
					Console.WriteLine("{0} ({1}) : {2}", nameServer.Type.ToString(), nameServer.Domain, nameServer.Record.ToString());
				}

				foreach (AdditionalRecord additionalRecord in response.AdditionalRecords)
				{
					Console.WriteLine("{0} ({1}) : {2}", additionalRecord.Type.ToString(), additionalRecord.Domain, additionalRecord.Record.ToString());
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
