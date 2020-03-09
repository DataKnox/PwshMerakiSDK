using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace GetMerakiOrgsCmdlet
{
    [Cmdlet(VerbsCommon.Get, "merakivlan")]
    [OutputType(typeof(CMerakiVlan))]
    public class GetMerakiVlanCommand : PSCmdlet
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
        public string vlanid { get; set; }

        private static async Task<IList<CMerakiVlan>> GetVlans(string Token, string netid, string vlanid)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Cisco-Meraki-API-Key", Token);

                var streamTask = client.GetStreamAsync($"https://dashboard.meraki.com/api/v0/networks/{netid}/vlans/{vlanid}");
                
                return await JsonSerializer.DeserializeAsync<IList<CMerakiVlan>>(await streamTask);
            }
        }

        private static  IList<CMerakiVlan> ProcessRecordAsync(string Token, string netid, string vlanid)
        {
            var task = GetVlans(Token, netid, vlanid);
            task.Wait();
            var result = task.Result;
            return result;
        }

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteVerbose("Begin!");
            WriteVerbose(Token);
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            WriteVerbose("Entering Get Orgs call");
            var list = ProcessRecordAsync(Token, netid, vlanid);
            
            WriteObject(list,true);


            WriteVerbose("Exiting foreach");
        }

        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }

    public class ReservedIpRange
    {
        public string start { get; set; }
        public string end { get; set; }
        public string comment { get; set; }
    }

    public class DhcpOption
    {
        public int code { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class CMerakiVlan
    {
        [JsonPropertyName("id")]
        public int vlanid { get; set; }
        public string networkId { get; set; }
        public string name { get; set; }
        public string applianceIp { get; set; }
        public string subnet { get; set; }
        public List<ReservedIpRange> reservedIpRanges { get; set; }
        public string dnsNameservers { get; set; }
        public string dhcpHandling { get; set; }
        public string dhcpLeaseTime { get; set; }
        public bool dhcpBootOptionsEnabled { get; set; }
        public object dhcpBootNextServer { get; set; }
        public object dhcpBootFilename { get; set; }
        public List<DhcpOption> dhcpOptions { get; set; }
    }
}
