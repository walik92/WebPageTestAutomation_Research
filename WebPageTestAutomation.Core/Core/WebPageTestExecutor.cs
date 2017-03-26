﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using WebPageTestAutomation.Core.Enumerators;
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

                var resultTestRequest =
                    ConvertResultTestRequest(await _pageTestApiService.SendTestAsync(page.Url, browser, connection,
                        numberOfRuns));
                _logger.Info("Test added correctly. " +
                             $"Response of server {resultTestRequest.StatusCode}" +
                             $" : {resultTestRequest.StatusText}");
                do
                {
                    var testResult = ConvertResultTestReceive(
                        await _pageTestApiService.GetResultOfTestAsync(resultTestRequest.Details.JsonUrl));
                    if (testResult is ResultTestReceiveExpandedModel)
                    {
                        _logger.Debug(testResult as ResultTestReceiveExpandedModel);
                        await _pageTestResultExporter.Save(testResult as ResultTestReceiveExpandedModel,
                            $"{page.Name}_{browser}_{connection}");
                        break;
                    }
                    _logger.Info("Waiting from result test of server. " +
                                 $"Response of server {testResult.StatusCode}" +
                                 $" : {testResult.StatusText}");
                    await Task.Delay(RefreshTimeInterval);
                } while (true);

                _logger.Info($"End test: {page} browser:{browser} connection: {connection} ");
            }
        }

        private ResultTestRequestModel ConvertResultTestRequest(string json)
        {
            _logger.Debug(json);
            var resultTestRequest = JsonConvert.DeserializeObject
                <ResultTestRequestModel>(json);
            if (resultTestRequest.StatusCode != 200)
                throw new Exception($"Error while adding test. " +
                                    $"HttpCode {resultTestRequest.StatusCode} " +
                                    $"ErrorText:{resultTestRequest.StatusText}");

            return resultTestRequest;
        }

        private ResultTestReceiveBaseModel ConvertResultTestReceive(string json)
        {
            _logger.Debug(json);
            var testResultBase = JsonConvert.DeserializeObject<ResultTestReceiveBaseModel>(json);
            if (testResultBase.StatusCode < 200)
                return testResultBase;
            if (testResultBase.StatusCode == 200)
                return JsonConvert.DeserializeObject<ResultTestReceiveExpandedModel>(json);
            throw new Exception("Error while receive results test. " +
                                $"HttpCode {testResultBase.StatusCode} " +
                                $"ErrorText:{testResultBase.StatusText}");
        }
    }
}