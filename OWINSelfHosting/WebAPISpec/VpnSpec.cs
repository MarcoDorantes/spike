using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http;

/*
*** Reliable ***

POST REQUEST:
	URL	: api/reliable
	Payload	: 
		<SendRequest>
			<vpn>registeredVPN</vpn>
			<destination.type>topic</destination.type>
			<destination>topic</destination>
			<correlationID>integer</correlationID>
			<message>text-message</message>
		</SendRequest>

POST RESPONSE:
		<SendResponse>
			<correlationID>integer</correlationID>
			<status>enum</status>
		</SendResponse>

PUT REQUEST:
	URL	: api/queue
	Payload	: 
		<SendRequest>
			<vpn>registeredVPN</vpn>
			<destination>queue</destination>
			<correlationID>integer</correlationID>
			<message>text-message</message>
		</SendRequest>

Ejemplos:
A tópico:
	URL	: POST VPN_1/topic
	Payload	: 
		<destination>topicpathj</destination>
		<message>8=FIX ...</message>


A queue:
	URL	: PUT VPN_2/queue
	Payload	: 
		<destination>QUEUE1</destination>
		<message>8=FIX ...</message>

*** Guaranteed ***

  POST REQUEST:
	URL	: api/guaranteed

  */

namespace WebAPISpec
{
    public class VpnController : ApiController
    {
        // POST api/value
        public SendResponse Post([FromBody]SendRequest request) { return null; }


    }

    [TestClass]
    public class VpnSpec
    {
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}