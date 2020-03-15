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
    [Cmdlet(VerbsCommon.Set, "MxL3Firewall")]
    [OutputType(typeof(MxFirewallRule))]
    public class SetL3FirewallRuleCommand : PSCmdlet
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

        [ValidateSet("allow", "deny", IgnoreCase = true)]
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
        public string comment { get; set; }

        [ValidateSet("tcp", "udp", "icmp", "any", IgnoreCase = true)]
        [Parameter(
            Mandatory = true,
            Position = 4,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string protocol { get; set; }

        [Parameter(
           Mandatory = true,
           Position = 5,
           ValueFromPipeline = true,
           ValueFromPipelineByPropertyName = true)]
        public string srcPort { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 6,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string srcCidr { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 7,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string destPort { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 8,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string destCidr { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 9,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public bool syslogEnabled { get; set; }

        private static async Task<string> UpdateL3FWRule(string Token, string netid, L3RulesList rules)
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonString;
                string uri;
                uri = $"https://dashboard.meraki.com/api/v0/networks/{netid}/l3FirewallRules";
                jsonString = JsonSerializer.Serialize<L3RulesList>(rules);

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

        private static string ProcessRecordAsync(string Token, string netid, L3RulesList rules)
        {
            var task = UpdateL3FWRule(Token, netid, rules);
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
            MxFirewallRule rule = new MxFirewallRule();
            L3RulesList rules = new L3RulesList();
            rule.comment = comment;
            rule.protocol = protocol;
            rule.policy = policy;
            rule.srcCidr = srcCidr;
            rule.srcPort = srcPort;
            rule.destCidr = destCidr;
            rule.destPort = destPort;
            rule.syslogEnabled = syslogEnabled;
            rules.rules.Add(rule);

            WriteVerbose("Entering Get Orgs call");
            var list = ProcessRecordAsync(Token, netid, rules);

            MxFirewallRule result = JsonSerializer.Deserialize<MxFirewallRule>(list);
            WriteObject(result, true);

            WriteVerbose("Exiting foreach");
        }


        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }
    public class L3RulesList
    {
        public List<MxFirewallRule> rules { get; set; }
    }

}