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
    [Cmdlet(VerbsCommon.Get, "NetClientTraffic")]
    [OutputType(typeof(TrafficHistory))]
    public class GetNetClientTrafficCommand : PSCmdlet
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

        private static async Task<IList<TrafficHistory>> GetClientTraffic(string Token, string netid, string clientid)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Cisco-Meraki-API-Key", Token);

                var streamTask = client.GetStreamAsync($"https://dashboard.meraki.com/api/v0/networks/{netid}/clients/{clientid}/trafficHistory?perPage=3&startingAfter=&endingBefore=");
                
                return await JsonSerializer.DeserializeAsync<IList<TrafficHistory>>(await streamTask);
            }
        }

        private static  IList<TrafficHistory> ProcessRecordAsync(string Token, string netid, string clientid)
        {
            var task = GetClientTraffic(Token, netid, clientid);
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

    public class TrafficHistory
    {
        public int ts { get; set; }
        public string application { get; set; }
        public string destination { get; set; }
        public string protocol { get; set; }
        public int port { get; set; }
        public int recv { get; set; }
        public int sent { get; set; }
        public int numFlows { get; set; }
        public int activeSeconds { get; set; }
    }
}