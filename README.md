# Bdev.Net.Dns

A .Net library to execute DNS lookups from one, or multiple DNS server.

## Sample usage

### Helpers

Return all available ANAME records for Google

    DnsServer.Resolve("google.com")
	
Return all MX Records for a domain

    DnsServers.Resolve<MXRecord>("codeproject.com", DnsType.MX, DnsClass.IN);
	
Get all known DNS Servers on all active network interfaces

    DnsServers.All
	
	DnsServers.IP4
	
	DnsServers.IP6

### Specifying DNS lookup

Resolve a record on a DNS server

    // create a new request
    var request = new Request();
<<<<<<< HEAD
    
	// add the codeproject NS question
    request.AddQuestion(new Question("codeproject.com", DnsType.NS, DnsClass.IN));
    
	// send the request
    Response response = Resolver.Lookup(request, DnsServers.IP4.First());
	
 
=======
    // add the codeproject ANAME question
    request.AddQuestion(new Question("codeproject.com", DnsType.NS, DnsClass.IN));
    // send the request
    Response response = Resolver.Lookup(request, DnsServers.IP4.First());
	
>>>>>>> 5371da39f76db90a429a9a08f1b72dcad38da6ef
