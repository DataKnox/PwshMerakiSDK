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
    [Cmdlet(VerbsCommon.Set, "UplinkSettings")]
    [OutputType(typeof(UplinkSettings))]
    public class SetUplinkSettingsCommand : PSCmdlet
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
            Mandatory = false,
            Position = 2,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public int wan1UpLimit { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 3,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public int wan1DownLimit { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 4,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public int wan2UpLimit { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 5,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public int wan2DownLimit { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 6,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public int CellularUpLimit { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 7,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public int CellularDownLimit { get; set; }

        private static async Task<string> UpdateUplinks(string Token, string netid, UplinkSettings set)
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonString;
                string uri;
                uri = $"https://dashboard.meraki.com/api/v0/networks/{netid}/uplinkSettings";
                jsonString = JsonSerializer.Serialize<UplinkSettings>(set);

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

        private static string ProcessRecordAsync(string Token, string netid, UplinkSettings set)
        {
            var task = UpdateUplinks(Token, netid, set);
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
            UplinkSettings set = new UplinkSettings();
            SetBandwidthLimits band = new SetBandwidthLimits();
            Cellular cell = new Cellular();
            Wan1 wan1 = new Wan1();
            Wan2 wan2 = new Wan2();
            wan1.limitDown = wan1DownLimit;
            wan1.limitUp = wan1UpLimit;
            wan2.limitDown = wan2DownLimit;
            wan2.limitUp = wan2UpLimit;
            cell.limitUp = CellularUpLimit;
            cell.limitDown = CellularDownLimit;
            band.wan1 = wan1;
            band.wan2 = wan2;
            band.cellular = cell;
            set.bandwidthLimits = band;

            WriteVerbose("Entering Get Orgs call");
            var list = ProcessRecordAsync(Token, netid, set);

            UplinkSettings result = JsonSerializer.Deserialize<UplinkSettings>(list);
            WriteObject(result, true);

            WriteVerbose("Exiting foreach");
        }


        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }
    public class Wan1
    {
        public int limitUp { get; set; }
        public int limitDown { get; set; }
    }

    public class Wan2
    {
        public int limitUp { get; set; }
        public int limitDown { get; set; }
    }

    public class Cellular
    {
        public int limitUp { get; set; }
        public int limitDown { get; set; }
    }

    public class SetBandwidthLimits
    {
        public Wan1 wan1 { get; set; }
        public Wan2 wan2 { get; set; }
        public Cellular cellular { get; set; }
    }

    public class UplinkSettings
    {
        public SetBandwidthLimits bandwidthLimits { get; set; }
    }
}