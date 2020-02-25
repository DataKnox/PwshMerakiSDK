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
    [Cmdlet(VerbsCommon.Get, "MRFirewallRules")]
    [OutputType(typeof(MRFirewallRule))]
    public class GetMRFirewallRulesCommand : PSCmdlet
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
        public int number { get; set; }

        private static async Task<IList<MRFirewallRule>> GetMRFWRules(string Token, string netid, int number)
        {
            using (HttpClient client = new HttpClient())
            {
                string numberval = number.ToString();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Cisco-Meraki-API-Key", Token);

                var streamTask = client.GetStreamAsync($"https://dashboard.meraki.com/api/v0/networks/{netid}/ssids/{numberval}/l3FirewallRules");
                
                return await JsonSerializer.DeserializeAsync<IList<MRFirewallRule>>(await streamTask);
            }
        }

        private static  IList<MRFirewallRule> ProcessRecordAsync(string Token, string netid, int number)
        {
            var task = GetMRFWRules(Token, netid, number);
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
            var list = ProcessRecordAsync(Token, netid, number);
            
            WriteObject(list,true);


            WriteVerbose("Exiting foreach");
        }

        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }

    public class MRFirewallRule
    {
        public string comment { get; set; }
        public string policy { get; set; }
        public string protocol { get; set; }
        public int destPort { get; set; }
        public string destCidr { get; set; }
    }
}