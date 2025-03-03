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

1.0.19 Added implementation for two new record types (SRV & DS) and extends the list with more popular record types.
       Fixes a bug where the record type was sent as single octet (which breaks types exceeding 255), and added an appropriate test for it.

1.0.18 Bug fix for MXRecords
       Added support for AAAA records

1.0.17 Bug fixes and added support for .Net 9.0

1.0.16 Added version for .Net 7.0 and 8.0

1.0.15 Bring-back .Net 2.0

1.0.14 Wait 5 seconds for a dns request to complete.
       Discontinue support for .Net 3.1

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