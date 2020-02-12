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
    [Cmdlet(VerbsCommon.Set, "MerakiSSID")]
    [OutputType(typeof(ResponseSsid))]
    public class SetMerakiSsidCommand : PSCmdlet
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
        public string name { get; set; }

        [ValidateSet("open", "psk", "open-with-radius", "8021x-meraki", "8021x-radius", IgnoreCase = true)]
        [Parameter(
            Mandatory = false,
            Position = 3,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string authMode { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 4,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public int number { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 5,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string psk { get; set; }

        [ValidateSet("WPA1 and WPA2", "WPA2 Only", IgnoreCase = true)]
        [Parameter(
            Mandatory = false,
            Position = 6,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string wpaEncryptionMode { get; set; }

        private static async Task<string> SetSsid(string Token, string netid, UpdateSsid ssid)
        {
            using (HttpClient client = new HttpClient())
            {
                string numberval = ssid.number.ToString();
                string jsonString;
                string uri;
                uri = $"https://dashboard.meraki.com/api/v0/networks/{netid}/ssids/{numberval}";
                jsonString=JsonSerializer.Serialize<UpdateSsid>(ssid);
                
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

        private static string ProcessRecordAsync(string Token, string netid, UpdateSsid ssid)
        {
            var task = SetSsid(Token, netid, ssid);
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
            UpdateSsid ssid = new UpdateSsid();
            ssid.name = name;
            ssid.number = number;
            ssid.enabled = true;
            if(String.IsNullOrEmpty(authMode)){
                ssid.authMode = authMode;
            }
            

            WriteVerbose("Entering Get Orgs call");
            var list = ProcessRecordAsync(Token, netid, ssid);

            ResponseSsid result = JsonSerializer.Deserialize<ResponseSsid>(list);
            WriteObject(result,true);

            WriteVerbose("Exiting foreach");
        }


        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }

    public class UpdateSsid
    {
        public string name { get; set; }
        public bool enabled { get; set; }
        public int number {get; set;}
        public string authMode { get; set; }
        public string psk { get; set; }
        public string wpaEncryptionMode { get; set; }
    }


    public class ResponseSsid
    {
        public int number { get; set; }
        public string name { get; set; }
        public bool enabled { get; set; }
        public string splashPage { get; set; }
        public bool ssidAdminAccessible { get; set; }
        public string authMode { get; set; }
        public string encryptionMode { get; set; }
        public string wpaEncryptionMode { get; set; }
        public List<RadiusServer> radiusServers { get; set; }
        public bool radiusAccountingEnabled { get; set; }
        public bool radiusEnabled { get; set; }
        public string radiusAttributeForGroupPolicies { get; set; }
        public string radiusFailoverPolicy { get; set; }
        public string radiusLoadBalancingPolicy { get; set; }
        public string ipAssignmentMode { get; set; }
        public string adminSplashUrl { get; set; }
        public string splashTimeout { get; set; }
        public bool walledGardenEnabled { get; set; }
        public string walledGardenRanges { get; set; }
        public int minBitrate { get; set; }
        public string bandSelection { get; set; }
        public int perClientBandwidthLimitUp { get; set; }
        public int perClientBandwidthLimitDown { get; set; }
    }
}