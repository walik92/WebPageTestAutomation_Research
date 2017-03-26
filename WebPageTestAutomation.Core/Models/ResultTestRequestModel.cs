using Newtonsoft.Json;

namespace WebPageTestAutomation.Core.Models
{
    public class ResultTestRequestModel
    {
        public int StatusCode { get; set; }
        public string StatusText { get; set; }

        [JsonProperty("data")]
        public ResultTestRequestDetails Details { get; set; }
    }

    public class ResultTestRequestDetails
    {
        public string TestId { get; set; }
        public string OwnerKey { get; set; }
        public string JsonUrl { get; set; }
        public string XmlUrl { get; set; }
        public string UserUrl { get; set; }
        public string SummaryCsv { get; set; }
        public string DetailCsv { get; set; }
    }
}