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
        private readonly int _refreshIntervalTime;

        public WebPageTestExecutor(ILog logger, IWebPageTestApiService pageTestApiService,
            IWebPageTestResultExporter pageTestResultExporter, int refreshIntervalTime)
        {
            _logger = logger;
            _pageTestApiService = pageTestApiService;
            _pageTestResultExporter = pageTestResultExporter;
            _refreshIntervalTime = refreshIntervalTime;
        }

        private int RefreshTimeInterval => _refreshIntervalTime * 1000;

        public async Task Execute(IList<PageModel> pages, IList<Browser> browsers, IList<Connection> connections,
            int numberOfRuns)
        {
            foreach (var page in pages)
                foreach (var browser in browsers)
                    foreach (var connection in connections)

                    {
                        _logger.Info($"Start test: {page.Name}, browser: {browser}, connection: {connection} ");

                        //run test
                        var resultTestRequest = ConverterResult.ConvertRequest(
                            await _pageTestApiService.SendTestAsync(page.Url, browser, connection,
                                numberOfRuns));

                        _logger.Info("Test added correctly. " +
                                     $"Response of server {resultTestRequest.StatusCode}" +
                                     $" : {resultTestRequest.StatusText}");

                        //waiting for result
                        do
                        {
                            var jsonResult = await _pageTestApiService.GetResultOfTestAsync(resultTestRequest.JsonUrl);
                            if (string.IsNullOrEmpty(jsonResult))
                                continue;
                            var testResult = ConverterResult.ConvertReceive(jsonResult);

                            if (testResult is ResultTestReceiveExpandedModel)
                            {
                                _logger.Debug(testResult as ResultTestReceiveExpandedModel);

                                //save result
                                await _pageTestResultExporter.Save(testResult as ResultTestReceiveExpandedModel, page, browser, connection);
                                break;
                            }

                            _logger.Info("Waiting from result test of server. " +
                                         $"Response of server {testResult.StatusCode}" +
                                         $" : {testResult.StatusText}");
                            await Task.Delay(RefreshTimeInterval);
                        } while (true);

                        _logger.Info($"End test: {page.Name} browser: {browser} connection: {connection} ");
                    }
        }
    }
}