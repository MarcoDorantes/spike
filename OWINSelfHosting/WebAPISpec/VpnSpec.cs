using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http;

/*
*** Reliable WebAPI ***

POST REQUEST:
	POST api/reliable
	<SendRequest>
		<vpn>registeredVPN</vpn>
		<topic>topic</topic>
		<message>text-message</message>
	</SendRequest>

POST RESPONSE: 204 NoContent

*** Guaranteed WebAPI ***

POST REQUEST:
	POST api/guaranteed
	<SendRequest>
		<vpn>registeredVPN</vpn>
		<destination_type>topic-or-queue</destination_type>
		<destination>topicpath-or-queue</destination>
		<correlationid>integer</correlationid>
		<message>text-message</message>
	</SendRequest>

POST RESPONSE:
	<SendResponse>
		<correlationid>integer</correlationid>
		<status>string</status>
		<description>string</description>
	</SendResponse>

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