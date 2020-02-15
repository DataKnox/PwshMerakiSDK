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
    [Cmdlet(VerbsCommon.Get, "NetTrafficRules")]
    [OutputType(typeof(TrafficRule))]
    public class GetNetTrafficRulesCommand : PSCmdlet
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

        // This method creates the API call and returns a Task object that can be waited on
        private static async Task<TrafficRule> GetTrafficRules(string Token, string netid)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Cisco-Meraki-API-Key", Token);
                
                var streamTask = client.GetStreamAsync($"https://dashboard.meraki.com/api/v0//networks/{netid}/trafficShaping");
                
                return await JsonSerializer.DeserializeAsync<TrafficRule>(await streamTask);
            }
            
        }
        //This method calls GetNets and waits on the result. It then returns the List of MerakiDeviceClients objects
        private static TrafficRule ProcessRecordAsync(string Token, string netid)
        {
            var task = GetTrafficRules(Token, netid);
            task.Wait();
            var result = task.Result;
            return result;
        }

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteVerbose("Begin!");
            WriteVerbose($"Token is {Token}");            
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
    }

    public class Definition
    {
        public string type { get; set; }
        public object value { get; set; }
    }

    public class BandwidthLimits
    {
        public int limitUp { get; set; }
        public int limitDown { get; set; }
    }

    public class PerClientBandwidthLimits
    {
        public string settings { get; set; }
        public BandwidthLimits bandwidthLimits { get; set; }
    }

    public class TRule
    {
        public List<Definition> definitions { get; set; }
        public PerClientBandwidthLimits perClientBandwidthLimits { get; set; }
        public object dscpTagValue { get; set; }
        public string priority { get; set; }
    }

    public class TrafficRule
    {
        public bool defaultRulesEnabled { get; set; }
        public List<TRule> rules { get; set; }
    }
}