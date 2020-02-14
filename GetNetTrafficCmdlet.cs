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
    [Cmdlet(VerbsCommon.Get, "NetTraffic")]
    [OutputType(typeof(NetTraffic))]
    public class GetMerakiNetTrafficCommand : PSCmdlet
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

        private static async Task<IList<NetTraffic>> GetNetTraffic(string Token, string netid)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Cisco-Meraki-API-Key", Token);

                var streamTask = client.GetStreamAsync($"https://dashboard.meraki.com/api/v0/networks/{netid}/traffic?timespan=604800");
                
                return await JsonSerializer.DeserializeAsync<IList<NetTraffic>>(await streamTask);
            }
        }

        private static  IList<NetTraffic> ProcessRecordAsync(string Token, string netid)
        {
            var task = GetNetTraffic(Token, netid);
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
            var list = ProcessRecordAsync(Token, netid);
            
            WriteObject(list,true);


            WriteVerbose("Exiting foreach");
        }

        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    } // end Get-MerakiOrgs

    public class NetTraffic
    {
        public string application { get; set; }
        public object destination { get; set; }
        public string protocol { get; set; }
        public int port { get; set; }
        public double sent { get; set; }
        public double recv { get; set; }
        public int numClients { get; set; }
        public int activeTime { get; set; }
        public int flows { get; set; }
    }
}