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
    [Cmdlet(VerbsCommon.Set, "FirewalledService")]
    [OutputType(typeof(NetFirewallService))]
    public class SetFirewalledServiceCommand : PSCmdlet
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

       [ValidateSet("blocked", "restricted", "unrestricted", IgnoreCase = true)]
       [Parameter(
            Mandatory = true,
            Position = 2,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string access { get; set; } 

       [Parameter(
            Mandatory = true,
            Position = 3,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string allowedIp { get; set; } 

        [Parameter(
            Mandatory = true,
            Position = 4,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string service { get; set; } 

        private static async Task<string> UpdateFWService(string Token, string netid, SetNetFirewallService serv, string service)
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonString;
                string uri;
                uri = $"https://dashboard.meraki.com/api/v0/networks/{netid}/firewalledServices/{service}";
                jsonString=JsonSerializer.Serialize<SetNetFirewallService>(serv);
                
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

        private static string ProcessRecordAsync(string Token, string netid, SetNetFirewallService serv, string service)
        {
            var task = UpdateFWService(Token, netid, serv, service);
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
            SetNetFirewallService serv = new SetNetFirewallService();
            serv.access = access;
            serv.allowedIps.Add(allowedIp);
            
            WriteVerbose("Entering Get Orgs call");
            var list = ProcessRecordAsync(Token, netid, serv, service);

            NetFirewallService result = JsonSerializer.Deserialize<NetFirewallService>(list);
            WriteObject(result,true);

            WriteVerbose("Exiting foreach");
        }


        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }

    public class SetNetFirewallService
    {       
        public string access { get; set; }
        public List<string> allowedIps { get; set; }
    }
}