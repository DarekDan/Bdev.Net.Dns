# Bdev.Net.Dns

[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Bdev.Net.Dns)](https://github.com/DarekDan/Bdev.Net.Dns)


A .Net library to execute DNS lookups from one, or multiple DNS server.

## Sample usage

    Install-Package Bdev.Net.Dns

### Helpers

Return all available ANAME records for Google

    DnsServer.Resolve("google.com")

Return all MX Records for a domain

    DnsServers.Resolve<MXRecord>("codeproject.com", DnsType.MX, DnsClass.IN);

Return all TXT records for a domain

    Resolver.Lookup(new Request { RecursionDesired = true }.WithQuestion(new Question("google.com", DnsType.TXT)));

or with a helper

     DnsServers.Resolve<TXTRecord>("google.com");

Return a CNAME

    DnsServers.Resolve<CNameRecord>("mail.google.com").First();

Get all known DNS Servers on all active network interfaces

    DnsServers.All

    DnsServers.IP4

    DnsServers.IP6

### Specifying DNS lookup

Resolve a record on a DNS server

    // create a new request
    var request = new Request();

    // add the codeproject NS question
    request.AddQuestion(new Question("codeproject.com", DnsType.NS, DnsClass.IN));

    // send the request
    Response response = Resolver.Lookup(request, DnsServers.IP4.First());


### Release history
1.0.13 Bug fix

1.0.12 Fix CNAME lookup #7

1.0.11 Fix TXT records with multiple strings
       Add DNS over TCP fallback for truncated messages

1.0.10 Quality improvements

1.0.9 Support for .Net Core 3.1 and .Net 4.8

1.0.8 Added support for CNAME

#### Breaking changes

##### 1.0.10
New project organization will require to update imports of references for records and custom exceptions. 