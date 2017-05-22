using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebPageTestAutomation.Core.Core;
using WebPageTestAutomation.Core.Enumerators;
using WebPageTestAutomation.Core.ICore;
using WebPageTestAutomation.Core.Models;

namespace WebPageTestAutomation.Core.Test.Core
{
    [TestClass]
    public class WebPageTestExecutorTest
    {
        private readonly string exampleApiResponesPath = @"ExampleApiRespones\";
        private readonly int numberRunsTest = 1;
        private Mock<IWebPageTestResultExporter> _exporter;
        private Mock<ILog> _logger;
        private Mock<IWebPageTestApiService> _service;
        private List<Browser> browsers;
        private List<Connection> connections;

        private List<PageModel> pages;

        [TestInitialize]
        public void Init()
        {
            pages = new List<PageModel>
            {
                new PageModel {Url = "onet.pl", Name = "Onet"},
                new PageModel {Url = "wp.pl", Name = "wp"}
            };
            browsers = new List<Browser> {Browser.Firefox, Browser.Chrome};
            connections = new List<Connection> {Connection.Cable, Connection.DSL, Connection.ThreeG};

            _logger = MockLog();
            _service = MockService();
            _exporter = MockExporter();
        }

        [TestMethod]
        public async Task TestExecute1()
        {
            var executor = new WebPageTestExecutor(_logger.Object, _service.Object, _exporter.Object);
            await executor.Execute(pages);
        }

        private Mock<ILog> MockLog()
        {
            var log = new Mock<ILog>();
            log.Setup(q => q.Info(It.IsAny<string>()));
            log.Setup(q => q.Debug(It.IsAny<string>()));
            log.Setup(q => q.Error(It.IsAny<string>()));
            return log;
        }

        private Mock<IWebPageTestApiService> MockService()
        {
            var service = new Mock<IWebPageTestApiService>();
            service.Setup(q => q.GetResultOfTestAsync(It.IsAny<string>()));

            var runTestResponse200 = File.ReadAllText($"{exampleApiResponesPath}RunTestResponse200.txt");
            service.Setup(
                    q =>
                        q.SendTestAsync(It.IsAny<string>(), It.IsAny<Browser>(), It.IsAny<Connection>(), It.IsAny<int>()))
                .Returns(Task.FromResult(runTestResponse200));

            var resultTestResponse100 = File.ReadAllText($"{exampleApiResponesPath}ResultTestResponse100.txt");
            var resultTestResponse101 = File.ReadAllText($"{exampleApiResponesPath}ResultTestResponse101.txt");
            var resultTestResponse200 = File.ReadAllText($"{exampleApiResponesPath}ResultTestResponse200.txt");

            for (var i = 0; i < pages.Count * browsers.Count * connections.Count; i++)
                service.SetupSequence(q => q.GetResultOfTestAsync(It.IsAny<string>()))
                    .Returns(Task.FromResult(resultTestResponse101))
                    .Returns(Task.FromResult(resultTestResponse100))
                    .Returns(Task.FromResult(resultTestResponse200));
            return service;
        }

        private Mock<IWebPageTestResultExporter> MockExporter()
        {
            var exporter = new Mock<IWebPageTestResultExporter>();
            exporter.Setup(q => q.Save(It.IsAny<ResultTestReceiveExpandedModel>(), It.IsAny<PageModel>()))
                .Returns(Task.CompletedTask);
            return exporter;
        }
    }
}