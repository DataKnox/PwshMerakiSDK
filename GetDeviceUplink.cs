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
    [Cmdlet(VerbsCommon.Get, "DeviceUplinks")]
    [OutputType(typeof(DeviceUplink))]
    public class GetDeviceUplinksCommand : PSCmdlet
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
        public string netid { get; set; }

        // This method creates the API call and returns a Task object that can be waited on
        private static async Task<IList<DeviceUplink>> GetDevUplinks(string Token, string serial, string netid)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Cisco-Meraki-API-Key", Token);
                
                var streamTask = client.GetStreamAsync($"https://dashboard.meraki.com/api/v0//networks/{netid}/devices/{serial}/uplink");
                
                return await JsonSerializer.DeserializeAsync<IList<DeviceUplink>>(await streamTask);
            }
            
        }
        //This method calls GetNets and waits on the result. It then returns the List of MerakiDeviceClients objects
        private static  IList<DeviceUplink> ProcessRecordAsync(string Token, string serial, string netid)
        {
            var task = GetDevUplinks(Token, serial, netid);
            task.Wait();
            var result = task.Result;
            return result;
        }

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteVerbose("Begin!");
            WriteVerbose($"Token is {Token}");
            WriteVerbose($"serial is {serial}");
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            WriteVerbose("Entering Get Orgs call");
            
            var list = ProcessRecordAsync(Token, serial, netid);
            
            WriteObject(list,true);


            WriteVerbose("Exiting foreach");
        }

        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }

    public class DeviceUplink
    {
        public string @interface { get; set; }
        public string status { get; set; }
        public string ip { get; set; }
        public string gateway { get; set; }
        public string publicIp { get; set; }
        public string dns { get; set; }
        public bool usingStaticIp { get; set; }
    }
}