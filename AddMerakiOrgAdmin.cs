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
    [Cmdlet(VerbsCommon.Add, "merakiAdmin")]
    [OutputType(typeof(ResponseAdmin))]
    public class AddMerakiAdminCommand : PSCmdlet
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
        public string email { get; set; }

        [ValidateSet("None", "Full", "Read-only","Enterprise", IgnoreCase = true)]
        [Parameter(
            Mandatory = true,
            Position = 4,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string role { get; set; }

        

        private static async Task<string> AddAdmin(string Token, string orgid, CreateAdmin admin)
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonString;
                string uri;
                uri = $"https://dashboard.meraki.com/api/v0/organizations/{orgid}/admins";
                jsonString=JsonSerializer.Serialize<CreateAdmin>(admin);
                
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

        private static string ProcessRecordAsync(string Token, string orgid, CreateAdmin admin)
        {
            var task = AddAdmin(Token, orgid, admin);
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
            CreateAdmin admin = new CreateAdmin();
            admin.name = name;
            admin.email = email;
            admin.orgAccess = role;
            

            WriteVerbose("Entering Get Orgs call");
            var list = ProcessRecordAsync(Token, orgid, admin);

            ResponseAdmin result = JsonSerializer.Deserialize<ResponseAdmin>(list);
            WriteObject(result,true);

            WriteVerbose("Exiting foreach");
        }


        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }

    public class AdminResponseTag
    {
        public string tag { get; set; }
        public string access { get; set; }
    }

    public class AdminResponseNetwork
    {
        public string id { get; set; }
        public string access { get; set; }
    }

    public class ResponseAdmin
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string orgAccess { get; set; }
        public string accountStatus { get; set; }
        public bool twoFactorAuthEnabled { get; set; }
        public bool hasApiKey { get; set; }
        public string lastActive { get; set; }
        public List<AdminResponseTag> tags { get; set; }
        public List<AdminResponseNetwork> networks { get; set; }
    }

    public class CreateAdmin
    {
        public string name { get; set; }
        public string email { get; set; }
        public string orgAccess { get; set; }        
    }
}