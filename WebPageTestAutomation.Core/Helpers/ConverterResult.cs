using System;
using Newtonsoft.Json;
using WebPageTestAutomation.Core.Models;

namespace WebPageTestAutomation.Core.Helpers
{
    public static class ConverterResult
    {
        public static ResultTestReceiveBaseModel ConvertReceive(string json)
        {
            
            dynamic responseObj = JsonConvert.DeserializeObject(json);

            var statusCode = (int)responseObj.statusCode.Value;
            string statusText = responseObj.statusText.Value;

            if (statusCode < 200)
                return new ResultTestReceiveBaseModel
                {
                    StatusCode = statusCode,
                    StatusText = statusText
                };

            if (statusCode == 200)
            {
                var result = new ResultTestReceiveExpandedModel
                {
                    StatusCode = statusCode,
                    StatusText = statusText
                };
                
                result.Url = responseObj.data.url;
                foreach (var r in responseObj.data.runs)
                {
                    var run = new Run();
                    run.Id = (int)r.Value.firstView.run.Value;
                    if (r.Value.firstView.loadTime.Value == 0)
                        throw new Exception("Result response from server is incorrect");

                    run.LoadTime = (int)r.Value.firstView.loadTime.Value;
                    run.RenderStart = (int)r.Value.firstView.render.Value;
                    run.Ttfb = (int)r.Value.firstView.TTFB.Value;
                    run.SpeedIndex = (int)r.Value.firstView.SpeedIndex.Value;
                    run.VisuallyComplete = (int)r.Value.firstView.visualComplete.Value;
                    result.Runs.Add(run);
                    result.KBytes = ((int)r.Value.firstView.breakdown.js.bytes)/1024;
                }
                return result;
            }
            throw new Exception("Error while receive results test. " +
                                $"HttpCode {statusCode} " +
                                $"ErrorText:{statusText}");
        }

        public static ResultTestRequestModel ConvertRequest(string json)
        {
            dynamic responseObj = JsonConvert.DeserializeObject(json);

            var result = new ResultTestRequestModel
            {
                StatusCode = (int)responseObj.statusCode.Value,
                StatusText = responseObj.statusText.Value,
                JsonUrl = responseObj.data.jsonUrl
            };

            if (result.StatusCode != 200)
                throw new Exception($"Error while adding test. " +
                                    $"HttpCode {result.StatusCode} " +
                                    $"ErrorText:{result.StatusText}");
            return result;
        }
    }
}