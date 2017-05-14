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

        public async Task Execute(IList<PageModel> pages)
        {
            var result = await AddTest(pages);
            await WaitForResult(result);
        }
        public async Task<IDictionary<PageModel, string>> AddTest(IList<PageModel> pages)
        {
            var result = new Dictionary<PageModel, string>();
            foreach (var page in pages)
                foreach (var connection in Connections)
                    foreach (var browser in Browsers)
                    {
                        _logger.Info($"Start test: {page.Name} v{page.Version}, browser: {browser}, connection: {connection} ");

                        //run test
                        var resultTestRequest = ConverterResult.ConvertRequest(
                            await _pageTestApiService.SendTestAsync(page.Url, browser, connection,
                                NumberRunsTest));

                        var urlResult = resultTestRequest.JsonUrl;
                        result.Add(page, urlResult);

                        _logger.Info("Test added correctly. " +
                                     $"Response of server {resultTestRequest.StatusCode}" +
                                     $" : {resultTestRequest.StatusText}");

                        _logger.Info($"End test: {page.Name} v{page.Version}, browser: {browser} connection: {connection} ");
                    }
            return result;
        }
        private async Task WaitForResult(IDictionary<PageModel, string> runnedTests)
        {
            foreach (var test in runnedTests)
            {
                do
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
                    await Task.Delay(RefreshIntervalTime);
                } while (true);
            }

        }


    }
}