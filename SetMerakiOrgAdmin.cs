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
    [Cmdlet(VerbsCommon.Set, "merakiAdmin")]
    [OutputType(typeof(ResponseAdmin))]
    public class SetMerakiAdminCommand : PSCmdlet
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

        [Parameter(
            Mandatory = true,
            Position = 3,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string adminid { get; set; }

        [ValidateSet("None", "Full", "Read-only", "Enterprise", IgnoreCase = true)]
        [Parameter(
            Mandatory = true,
            Position = 4,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string role { get; set; }



        private static async Task<string> AddAdmin(string Token, string orgid, UpdateAdmin admin, string adminid)
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonString;
                string uri;
                uri = $"https://dashboard.meraki.com/api/v0/organizations/{orgid}/admins/{adminid}";
                jsonString = JsonSerializer.Serialize<UpdateAdmin>(admin);

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

        private static string ProcessRecordAsync(string Token, string orgid, UpdateAdmin admin, string adminid)
        {
            var task = AddAdmin(Token, orgid, admin, adminid);
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
            UpdateAdmin admin = new UpdateAdmin();
            admin.name = name;
            admin.orgAccess = role;


            WriteVerbose("Entering Get Orgs call");
            var list = ProcessRecordAsync(Token, orgid, admin, adminid);

            ResponseAdmin result = JsonSerializer.Deserialize<ResponseAdmin>(list);
            WriteObject(result, true);

            WriteVerbose("Exiting foreach");
        }


        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }

    public class UpdateAdmin
    {
        public string name { get; set; }
        public string orgAccess { get; set; }
    }
}