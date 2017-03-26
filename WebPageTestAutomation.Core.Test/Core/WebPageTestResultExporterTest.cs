using System.Collections.Generic;
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
        private readonly string _folder = @"Result\";
        private readonly ResultTestReceiveExpandedModel _modelReceive = new ResultTestReceiveExpandedModel();

        [TestInitialize]
        public void Init()
        {
            _modelReceive.Details = new ResultTestReceiveDetails();
            _modelReceive.Details.Url = "Example.pl";
            _modelReceive.Details.Runs = new Dictionary<string, Run>();
            var run1 = new Run();
            run1.FirstView = new View();
            run1.RepeatView = new View();

            run1.FirstView.LoadTime = 100;
            run1.RepeatView.LoadTime = 50;
            run1.FirstView.BytesOut = 1000;
            run1.RepeatView.BytesOut = 10000;

            var run2 = new Run();
            run2.FirstView = new View();
            run2.RepeatView = new View();

            run2.FirstView.LoadTime = 100;
            run2.RepeatView.LoadTime = 50;
            run2.FirstView.BytesOut = 1000;
            run2.RepeatView.BytesOut = 10000;

            _modelReceive.Details.Runs.Add("1", run1);
            _modelReceive.Details.Runs.Add("2", run2);
        }

        [TestMethod]
        public async Task TestSave()
        {
            var fileName = "Example";
            var exporter = new WebPageTestResultExporter(_folder);
            var obj = new PrivateObject(exporter);
            var args = new object[1] {fileName};
            var retval = obj.Invoke("GetPathFile", args);
            if (File.Exists(retval.ToString()))
                File.Delete(retval.ToString());
            await exporter.Save(_modelReceive, fileName);
            Assert.IsTrue(File.Exists(retval.ToString()));
        }
    }
}