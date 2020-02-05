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
    [Cmdlet(VerbsCommon.Get, "wifidevicestats")]
    [OutputType(typeof(MerakiDeviceStats))]
    public class GetMerakiDeviceStatsCommand : PSCmdlet
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

        private static async Task<IList<MerakiDeviceStats>> GetDeviceStats(string Token, string netid)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Cisco-Meraki-API-Key", Token);

                var streamTask = client.GetStreamAsync($"https://dashboard.meraki.com/api/v0/networks/{netid}/devices/connectionStats?timespan=600000.00");

                return await JsonSerializer.DeserializeAsync<IList<MerakiDeviceStats>>(await streamTask);
            }
        }
        // random comment
        private static IList<MerakiDeviceStats> ProcessRecordAsync(string Token, string netid)
        {
            var task = GetDeviceStats(Token, netid);
            task.Wait();
            var result = task.Result;
            return result;
        }

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteVerbose("Begin!");
            WriteVerbose(Token);
            WriteVerbose($"netid is {netid}");
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            WriteVerbose("Entering Get Orgs call");
            var list = ProcessRecordAsync(Token, netid);

            WriteObject(list, true);


            WriteVerbose("Exiting foreach");
        }

        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    } // end Get-MerakiOrgs

    public class ConnectionStats
    {
        public int assoc { get; set; }
        public int auth { get; set; }
        public int dhcp { get; set; }
        public int dns { get; set; }
        public int success { get; set; }
    }

    public class MerakiDeviceStats
    {
        public string serial { get; set; }
        public ConnectionStats connectionStats { get; set; }
    }
}