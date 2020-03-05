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
    [Cmdlet(VerbsCommon.Set, "MxL7FirewallRule")]
    [OutputType(typeof(L7FirewallRule))]
    public class SetL7FirewallRuleCommand : PSCmdlet
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
        public string policy { get; set; } 

       [Parameter(
            Mandatory = true,
            Position = 3,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string type { get; set; } 

        [Parameter(
            Mandatory = true,
            Position = 4,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string value { get; set; } 

        private static async Task<string> UpdateL7FWRule(string Token, string netid, L7FirewallRule rules)
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonString;
                string uri;
                uri = $"https://dashboard.meraki.com/api/v0/networks/{netid}/l7FirewallRules";
                jsonString=JsonSerializer.Serialize<L7FirewallRule>(rules);
                
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

        private static string ProcessRecordAsync(string Token, string netid, L7FirewallRule rules)
        {
            var task = UpdateL7FWRule(Token, netid, rules);
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
            L7FirewallRule rules = new L7FirewallRule();
            L7Rule rule = new L7Rule();
            rule.value = value;
            rule.type = type;
            rule.policy = policy;
            rules.rules.Add(rule);
                        
            WriteVerbose("Entering Get Orgs call");
            var list = ProcessRecordAsync(Token, netid, rules);

            L7FirewallRule result = JsonSerializer.Deserialize<L7FirewallRule>(list);
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