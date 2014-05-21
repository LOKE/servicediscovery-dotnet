using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace ServiceDiscovery.Tests
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void Test_Can_Publish_And_Find()
        {
            // arrange
            var id = Guid.NewGuid().ToString();
            var serviceName = Guid.NewGuid().ToString();
            var advertiser = new Advertiser(serviceName, id, 1234);
            advertiser.Start();
            Thread.Sleep(1000);
            var browser = new Browser(serviceName);
            browser.Start();

            // act
            var service = browser.WaitForService(10000);
            var endpoint = service.GetBestEndPoint();

            // assert
            Assert.IsTrue(browser.Services.Any());
            Assert.IsNotNull(service);
            Assert.IsNotNull(endpoint);
            Assert.IsNotNullOrEmpty(endpoint.IpAddress);

            Debug.Print(service.Name);
            Debug.Print(endpoint.IpAddress);
            Debug.Print(endpoint.Port.ToString());

            // dispose
            advertiser.Stop();
            browser.Stop();
        }
    }
}
