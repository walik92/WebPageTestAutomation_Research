using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using WebPageTestAutomation.Core.Enumerators;
using WebPageTestAutomation.Core.Helpers;
using WebPageTestAutomation.Core.ICore;
using WebPageTestAutomation.Core.Models;

namespace WebPageTestAutomation.Core.Core
{
    public class WebPageTestExecutor : IWebPageTestExecutor
    {
        private readonly ILog _logger;
        private readonly IWebPageTestApiService _pageTestApiService;
        private readonly IWebPageTestResultExporter _pageTestResultExporter;
        private int _refreshIntervalTime;

        public WebPageTestExecutor(ILog logger, IWebPageTestApiService pageTestApiService,
            IWebPageTestResultExporter pageTestResultExporter)
        {
            _logger = logger;
            _pageTestApiService = pageTestApiService;
            _pageTestResultExporter = pageTestResultExporter;
        }

        public int RefreshIntervalTime
        {
            get { return _refreshIntervalTime * 1000; }
            set { _refreshIntervalTime = value; }
        }

        public int NumberRunsTest { get; set; }
        public IList<Connection> Connections { get; set; }
        public IList<Browser> Browsers { get; set; }
        public IList<Mobile> Mobiles { get; set; }

        public async Task Execute(IList<PageModel> pages)
        {
            var result = await RunTests(pages);
            await WaitForResult(result);
        }

        public async Task<List<KeyValuePair<PageModel, string>>> RunTests(IList<PageModel> pages)
        {
            var result = new List<KeyValuePair<PageModel, string>>();
            foreach (var page in pages)
            {
                if (Connections != null && Browsers != null)
                    foreach (var connection in Connections)
                        foreach (var browser in Browsers)
                            result.Add(await AddTest(page, connection, browser));
                if (Mobiles != null)
                    foreach (var mobile in Mobiles)
                        result.Add(await AddTest(page, mobile));
            }

            return result;
        }

        private async Task<KeyValuePair<PageModel, string>> AddTest(PageModel page, Connection connection,
            Browser browser)
        {
            var resultTestRequest = ConverterResult.ConvertRequest(
                await _pageTestApiService.SendTestAsync(page.Url, browser, connection,
                    NumberRunsTest));

            _logger.Info("Test added correctly. " +
                         $"Response of server {resultTestRequest.StatusCode}" +
                         $" : {resultTestRequest.StatusText} {page.Name} v{page.Version} {connection.GetString()} {browser}");

            var urlResult = resultTestRequest.JsonUrl;
            return new KeyValuePair<PageModel, string>(page, urlResult);
        }

        private async Task<KeyValuePair<PageModel, string>> AddTest(PageModel page, Mobile mobile)
        {
            var resultTestRequest = ConverterResult.ConvertRequest(
                await _pageTestApiService.SendTestAsync(page.Url, mobile,
                    NumberRunsTest));

            _logger.Info("Test added correctly. " +
                         $"Response of server {resultTestRequest.StatusCode}" +
                         $" : {resultTestRequest.StatusText} {page.Name} v{page.Version} {mobile}");

            var urlResult = resultTestRequest.JsonUrl;
            return new KeyValuePair<PageModel, string>(page, urlResult);
        }

        private async Task WaitForResult(IList<KeyValuePair<PageModel, string>> runnedTests)
        {
            foreach (var test in runnedTests)
                do
                {
                    try
                    {
                        var jsonResult = await _pageTestApiService.GetResultOfTestAsync(test.Value);
                        if (string.IsNullOrEmpty(jsonResult))
                            continue;

                        var testResult = ConverterResult.ConvertReceive(jsonResult);

                        //result ok
                        if (testResult is ResultTestReceiveExpandedModel)
                        {
                            _logger.Debug(testResult as ResultTestReceiveExpandedModel);

                            //save result
                            await _pageTestResultExporter.Save(testResult as ResultTestReceiveExpandedModel, test.Key);
                            break;
                        }

                        _logger.Info("Waiting from result test of server. " +
                                     $"Response of server {testResult.StatusCode}" +
                                     $" : {testResult.StatusText} {test.Key.Name} v{test.Key.Version}");
                    }
                    catch (Exception exception)
                    {
                        _logger.Error(exception.Message);
                    }
                    await Task.Delay(RefreshIntervalTime);
                } while (true);
        }
    }
}