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
        public int Bytes { get; set; }
        public string Url { get; set; }
    }
}