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
    [Cmdlet(VerbsCommon.Get, "MsSwitchPorts")]
    [OutputType(typeof(MsSwitchPorts))]
    public class GetMsSwitchPortsCommand : PSCmdlet
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
        public string serial { get; set; }

        private static async Task<IList<MsSwitchPorts>> GetMsSwitchPort(string Token, string serial)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Cisco-Meraki-API-Key", Token);

                var streamTask = client.GetStreamAsync($"https://dashboard.meraki.com/api/v0/devices/{serial}/switchPorts");

                return await JsonSerializer.DeserializeAsync<IList<MsSwitchPorts>>(await streamTask);
            }
        }

        private static IList<MsSwitchPorts> ProcessRecordAsync(string Token, string serial)
        {
            var task = GetMsSwitchPort(Token, serial);
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
            var list = ProcessRecordAsync(Token, serial);

            WriteObject(list, true);


            WriteVerbose("Exiting foreach");
        }

        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    } // end Get-MerakiOrgs
    public class MsSwitchPorts
    {
        public int number { get; set; }
        public string name { get; set; }
        public string tags { get; set; }
        public bool enabled { get; set; }
        public bool poeEnabled { get; set; }
        public string type { get; set; }
        public int vlan { get; set; }
        public object voiceVlan { get; set; }
        public bool isolationEnabled { get; set; }
        public bool rstpEnabled { get; set; }
        public string stpGuard { get; set; }
        public object accessPolicyNumber { get; set; }
        public string linkNegotiation { get; set; }
        public string portScheduleId { get; set; }
        public string udld { get; set; }
        public List<string> macWhitelist { get; set; }
        public List<string> stickyMacWhitelist { get; set; }
        public object stickyMacWhitelistLimit { get; set; }
        public bool stormControlEnabled { get; set; }
    }
}