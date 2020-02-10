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
     [Cmdlet(VerbsCommon.Get, "OrgDevices")]
    [OutputType(typeof(OrgDevice))]
    public class GetOrgDevicesCommand : PSCmdlet
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
        public string orgid { get; set; }

        // This method creates the API call and returns a Task object that can be waited on
        private static async Task<IList<OrgDevice>> GetDevs(string Token, string orgid)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Cisco-Meraki-API-Key", Token);
                
                var streamTask = client.GetStreamAsync($"https://dashboard.meraki.com/api/v0/organizations/{orgid}/devices");
                
                return await JsonSerializer.DeserializeAsync<IList<OrgDevice>>(await streamTask);
            }
            
        }
        //This method calls GetNets and waits on the result. It then returns the List of MerakiNet objects
        private static  IList<OrgDevice> ProcessRecordAsync(string Token, string orgid)
        {
            var task = GetDevs(Token, orgid);
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
            var list = ProcessRecordAsync(Token, orgid);
            
            WriteObject(list,true);


            WriteVerbose("Exiting foreach");
        }

        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    } //end Get-MerakiNets
    
    public class OrgDevice
    {
        public string name { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string address { get; set; }
        public string notes { get; set; }
        public string tags { get; set; }
        public string networkId { get; set; }
        public string serial { get; set; }
        public string model { get; set; }
        public string mac { get; set; }
        public string lanIp { get; set; }
        public string firmware { get; set; }
    }
}