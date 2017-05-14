using System.Collections.Generic;

namespace WebPageTestAutomation.Core.Models
{
    public class ResultTestReceiveExpandedModel : ResultTestReceiveBaseModel
    {
        public ResultTestReceiveExpandedModel()
        {
            Runs = new List<Run>();
        }

        public IList<Run> Runs { get; set; }
        public int KBytes { get; set; }
        public string Url { get; set; }
        public string Connection { get; set; }
        public string Browser { get; set; }
    }
}