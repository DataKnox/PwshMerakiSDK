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
    [Cmdlet(VerbsCommon.Get, "SwitchPortSchedules")]
    [OutputType(typeof(SwitchPortSchedule))]
    public class GetMerakiSwitchPortScheduleCommand : PSCmdlet
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

        private static async Task<IList<SwitchPortSchedule>> GetSwitchPortSchedules(string Token, string netid)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Cisco-Meraki-API-Key", Token);

                var streamTask = client.GetStreamAsync($"https://dashboard.meraki.com/api/v0/networks/{netid}/switch/portSchedules");
                
                return await JsonSerializer.DeserializeAsync<IList<SwitchPortSchedule>>(await streamTask);
            }
        }

        private static IList<SwitchPortSchedule> ProcessRecordAsync(string Token, string netid)
        {
            var task = GetSwitchPortSchedules(Token, netid);
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

    public class Monday
    {
        public bool active { get; set; }
        public string from { get; set; }
        public string to { get; set; }
    }

    public class Tuesday
    {
        public bool active { get; set; }
        public string from { get; set; }
        public string to { get; set; }
    }

    public class Wednesday
    {
        public bool active { get; set; }
        public string from { get; set; }
        public string to { get; set; }
    }

    public class Thursday
    {
        public bool active { get; set; }
        public string from { get; set; }
        public string to { get; set; }
    }

    public class Friday
    {
        public bool active { get; set; }
        public string from { get; set; }
        public string to { get; set; }
    }

    public class Saturday
    {
        public bool active { get; set; }
        public string from { get; set; }
        public string to { get; set; }
    }

    public class Sunday
    {
        public bool active { get; set; }
        public string from { get; set; }
        public string to { get; set; }
    }

    public class PortSchedule
    {
        public Monday monday { get; set; }
        public Tuesday tuesday { get; set; }
        public Wednesday wednesday { get; set; }
        public Thursday thursday { get; set; }
        public Friday friday { get; set; }
        public Saturday saturday { get; set; }
        public Sunday sunday { get; set; }
    }

    public class SwitchPortSchedule
    {
        public string id { get; set; }
        public string networkId { get; set; }
        public string name { get; set; }
        public PortSchedule portSchedule { get; set; }
    }
}