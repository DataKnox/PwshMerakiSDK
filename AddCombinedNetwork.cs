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
    [Cmdlet(VerbsCommon.Add, "CombinedNetwork")]
    [OutputType(typeof(NewNetworkResponse))]
    public class AddCombinedNetworksCommand : PSCmdlet
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

        [Parameter(
            Mandatory = true,
            Position = 2,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string name { get; set; }

      private static async Task<string> AddNets(string Token, string orgid, NewNetwork net)
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonString;
                string uri;
                uri = $"https://dashboard.meraki.com/api/v0/organizations/{orgid}/networks";
                jsonString=JsonSerializer.Serialize<NewNetwork>(net);
                
                var content = new StringContent(jsonString);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                client.DefaultRequestHeaders.Add("X-Cisco-Meraki-API-Key", Token);

                var response = await client.PostAsync(uri,content);
                var contents = await response.Content.ReadAsStringAsync();
                
                return contents;
            }
        }

        private static string ProcessRecordAsync(string Token, string orgid, NewNetwork net)
        {
            var task = AddNets(Token, orgid, net);
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
            NewNetwork net = new NewNetwork();
            net.name = name;
            net.type = "wireless appliance switch systemsManager camera cellularGateway";
            

            WriteVerbose("Entering Get Orgs call");
            var list = ProcessRecordAsync(Token, orgid, net);

            NewNetworkResponse result = JsonSerializer.Deserialize<NewNetworkResponse>(list);
            WriteObject(result,true);

            WriteVerbose("Exiting foreach");
        }


        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }

    public class NewNetwork
    {
        public string name {get; set;}
        public string type {get; set;}
    }

    public class NewNetworkResponse
    {
        public string id { get; set; }
        public string organizationId { get; set; }
        public string name { get; set; }
        public string timeZone { get; set; }
        public string tags { get; set; }
        public string type { get; set; }
        public List<string> productTypes { get; set; }
        public bool disableMyMerakiCom { get; set; }
        public string enrollmentString { get; set; }
    }
}