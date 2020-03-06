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
    [Cmdlet(VerbsCommon.Set, "LinkAggregations")]
    [OutputType(typeof(LinkAggregations))]
    public class SetLinkAggregationsCommand : PSCmdlet
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
        public string firstswitchserial { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 3,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string firstswitchportid { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 4,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string secondswitchserial { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 5,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string secondswitchportid { get; set; }

        private static async Task<string> AddLinkAggs(string Token, string netid, LinkAggregations lagg)
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonString;
                string uri;
                uri = $"https://dashboard.meraki.com/api/v0/networks/{netid}/switch/linkAggregations";
                jsonString=JsonSerializer.Serialize<LinkAggregations>(lagg);
                
                var content = new StringContent(jsonString);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                client.DefaultRequestHeaders.Add("X-Cisco-Meraki-API-Key", Token);

                var response = await client.PutAsync(uri,content);
                var contents = await response.Content.ReadAsStringAsync();
                
                return contents;
            }
        }

        private static string ProcessRecordAsync(string Token, string netid, LinkAggregations lagg)
        {
            var task = AddLinkAggs(Token, netid, lagg);
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
            LinkAggregations lagg = new LinkAggregations();
            SwitchPort port1 = new SwitchPort();
            SwitchPort port2 = new SwitchPort();
            port1.serial = firstswitchserial;
            port1.portId = firstswitchportid;
            port2.serial = secondswitchserial;
            port2.portId = secondswitchportid;
            lagg.switchPorts.Add(port1);
            lagg.switchPorts.Add(port2);

            WriteVerbose("Entering Get Orgs call");
            var list = ProcessRecordAsync(Token, netid, lagg);

            LinkAggregations result = JsonSerializer.Deserialize<LinkAggregations>(list);
            WriteObject(result,true);

            WriteVerbose("Exiting foreach");
        }


        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }

}