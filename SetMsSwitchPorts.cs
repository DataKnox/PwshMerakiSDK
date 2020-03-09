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
    [Cmdlet(VerbsCommon.Set, "MsSwitchPorts")]
    [OutputType(typeof(MsSwitchPorts))]
    public class SetMsSwitchPortsCommand : PSCmdlet
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

        [Parameter(
            Mandatory = true,
            Position = 2,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string portNumber { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 3,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string name { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 4,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string tags { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 5,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public bool enabled { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 6,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string type { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 7,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public int vlan { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 8,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public int voiceVlan { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 9,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public bool isolationEnabled { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 10,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public bool rstpEnabled { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 11,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string stpGuard { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 12,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string accessPolicyNumber { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 13,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string linkNegotiation { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 14,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string portScheduleId { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 15,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public bool stormControlEnabled { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 16,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string udld { get; set; }

        private static async Task<string> SetMsSwitchPort(string Token, string serial, string number, MsSwitchPorts port)
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonString;
                string uri;
                uri = $"https://dashboard.meraki.com/api/v0/devices/{serial}/switchPorts/{number}";
                jsonString = JsonSerializer.Serialize<MsSwitchPorts>(port);

                var content = new StringContent(jsonString);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                client.DefaultRequestHeaders.Add("X-Cisco-Meraki-API-Key", Token);

                var response = await client.PutAsync(uri, content);
                var contents = await response.Content.ReadAsStringAsync();

                return contents;
            }
        }

        private static string ProcessRecordAsync(string Token, string serial, string number, MsSwitchPorts port)
        {
            var task = SetMsSwitchPort(Token, serial, number, port);
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
            MsSwitchPorts port = new MsSwitchPorts();
            port.name = name;
            port.tags = tags;
            port.enabled = enabled;
            port.type = type;
            port.vlan = vlan;
            port.voiceVlan = voiceVlan;
            port.isolationEnabled = isolationEnabled;
            port.rstpEnabled = rstpEnabled;
            port.stpGuard = stpGuard;
            port.accessPolicyNumber = accessPolicyNumber;
            port.linkNegotiation = linkNegotiation;
            port.portScheduleId = portScheduleId;
            port.stormControlEnabled = stormControlEnabled;
            port.udld = udld;

            WriteVerbose("Entering Get Orgs call");
            var list = ProcessRecordAsync(Token, serial, portNumber, port);

            MsSwitchPorts result = JsonSerializer.Deserialize<MsSwitchPorts>(list);
            WriteObject(result, true);

            WriteVerbose("Exiting foreach");
        }


        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }

}