# Service Discovery

## Overview

The service discovery module provides both advertiser and browser functionality to enable clients to find the IP address and port of the running server.

## Example

```csharp

// to advertise a service:
var service = "http";
var port = 80;
var myName = "Test Web Server";
var advertiser = new Advertiser(service, myName, port);
advertiser.Start();


// to find a service:
var browser = new Browser(service);
browser.Start();

// we can either bind to this event to be notified when a server comes online/offline
browser.ServicesUpdated += (sender, args) => { };

// or we can wait for a number of milliseconds for a service to be available
var timeout = 10000;
var service = browser.WaitForService(timeout);
var endpoint = service.GetBestEndPoint();
Console.WriteLine("IP: " + endpoint.IpAddress);
Console.WriteLine("Port: " + endpoint.Port);
```
