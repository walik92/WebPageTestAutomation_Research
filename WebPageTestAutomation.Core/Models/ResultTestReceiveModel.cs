using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebPageTestAutomation.Core.Models
{
    public class ResultTestReceiveBaseModel
    {
        public int StatusCode { get; set; }
        public string StatusText { get; set; }
    }

    public class ResultTestReceiveExpandedModel : ResultTestReceiveBaseModel
    {
        [JsonProperty("data")]
        public ResultTestReceiveDetails Details { get; set; }
    }

    public class ResultTestReceiveDetails
    {
        public string Id { get; set; }
        public string Url { get; set; }

        [JsonProperty("runs")]
        public Dictionary<string, Run> Runs { get; set; }
    }

    public class Run
    {
        public View FirstView { get; set; }
        public View RepeatView { get; set; }
    }

    public class View
    {
        public int LoadTime { get; set; }
        public int BytesOut { get; set; }
        public int BytesOutDoc { get; set; }
        public int BytesIn { get; set; }
        public int BytesInDoc { get; set; }
    }
}