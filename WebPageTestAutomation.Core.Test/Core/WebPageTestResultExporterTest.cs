using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebPageTestAutomation.Core.Core;
using WebPageTestAutomation.Core.Models;

namespace WebPageTestAutomation.Core.Test.Core
{
    [TestClass]
    public class WebPageTestResultExporterTest
    {
        private const string _folder = @"Result\";
        private const string _browser = "Opera";
        private const string _connection = "Cable";
        private ResultTestReceiveExpandedModel _modelReceive;

        [TestInitialize]
        public void Init()
        {
            _modelReceive = new ResultTestReceiveExpandedModel();
            _modelReceive.Url = "Example.pl";
            _modelReceive.KBytes = 100;
            _modelReceive.Runs.Add(new Run
            {
                Id = 1,
                LoadTime = 1000,
                RenderStart = 1000,
                SpeedIndex = 2000,
                Ttfb = 100,
                VisuallyComplete = 1000
            });
            _modelReceive.Browser = _browser;
            _modelReceive.Connection = _connection;
            _modelReceive.Runs.Add(new Run
            {
                Id = 2,
                LoadTime = 1000,
                RenderStart = 1000,
                SpeedIndex = 2000,
                Ttfb = 100,
                VisuallyComplete = 1000
            });
        }

        [TestMethod]
        public async Task TestSave()
        {
            var fileName = "Example";
            var page = new PageModel();
            page.Name = "test";
            page.Version = "1.0.0";
            page.Url = "test.pl";

            var exporter = new WebPageTestResultExporter(_folder);
            var obj = new PrivateObject(exporter);
            var args = new object[3] {page.Name, _browser, _connection};
            var retval = obj.Invoke("GetPathFile", args);
            if (File.Exists(retval.ToString()))
                File.Delete(retval.ToString());

            await exporter.Save(_modelReceive, page);
            Assert.IsTrue(File.Exists(retval.ToString()));
        }
    }
}