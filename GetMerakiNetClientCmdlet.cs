using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Collections.Generic;

namespace GetMerakiOrgsCmdlet
{
    [Cmdlet(VerbsCommon.Get, "merakinetclient")]
    [OutputType(typeof(NetworkClient))]
    public class GetMerakiClientCommand : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string Token { get; set; }

       [Parameter(
            Mandatory = true,
            Position = 1,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string netid { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 2,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string clientid { get; set; }

        private static async Task<NetworkClient> GetClient(string Token, string netid, string clientid)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Cisco-Meraki-API-Key", Token);

                var streamTask = client.GetStreamAsync($"https://dashboard.meraki.com/api/v0/networks/{netid}/clients/{clientid}");
                
                return await JsonSerializer.DeserializeAsync<NetworkClient>(await streamTask);
            }
        }

        private static  NetworkClient ProcessRecordAsync(string Token, string netid, string clientid)
        {
            var task = GetClient(Token, netid, clientid);
            task.Wait();
            var result = task.Result;
            return result;
        }

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteVerbose("Begin!");
            WriteVerbose(Token);
            WriteVerbose(clientid);
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            WriteVerbose("Entering Get Orgs call");
            var list = ProcessRecordAsync(Token, netid, clientid);
            
            WriteObject(list,true);


            WriteVerbose("Exiting foreach");
        }

        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    } // end Get-MerakiOrgs

    public class ClientVpnConnection
    {
        public string remoteIp { get; set; }
        public int connectedAt { get; set; }
        public int disconnectedAt { get; set; }
    }

    public class NetworkClient
    {
        public string id { get; set; }
        public string description { get; set; }
        public string mac { get; set; }
        public string ip { get; set; }
        public string user { get; set; }
        public string vlan { get; set; }
        public object switchport { get; set; }
        public string ip6 { get; set; }
        public int firstSeen { get; set; }
        public int lastSeen { get; set; }
        public string manufacturer { get; set; }
        public string os { get; set; }
        public string ssid { get; set; }
        public string wirelessCapabilities { get; set; }
        public bool smInstalled { get; set; }
        public string recentDeviceMac { get; set; }
        public ClientVpnConnection[] clientVpnConnections { get; set; }
        public object cdp { get; set; }
        public string status { get; set; }
    }
}