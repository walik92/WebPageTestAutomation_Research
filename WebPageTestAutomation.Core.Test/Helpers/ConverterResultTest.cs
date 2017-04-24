using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebPageTestAutomation.Core.Helpers;
using WebPageTestAutomation.Core.Models;

namespace WebPageTestAutomation.Core.Test.Helpers
{
    [TestClass]
    public class ConverterResultTest
    {
        private readonly string exampleApiResponesPath = @"ExampleApiRespones\";

        [TestMethod]
        public void TestResultResponse100Ok()
        {
            var response = File.ReadAllText($"{exampleApiResponesPath}ResultTestResponse100.txt");
            var result = ConverterResult.ConvertReceive(response);
            Assert.IsNotNull(result);
            Assert.AreEqual(100, result.StatusCode);
        }

        [TestMethod]
        public void TestResultResponse101Ok()
        {
            var response = File.ReadAllText($"{exampleApiResponesPath}ResultTestResponse101.txt");
            var result = ConverterResult.ConvertReceive(response);
            Assert.IsNotNull(result);
            Assert.AreEqual(101, result.StatusCode);
        }

        [TestMethod]
        public void TestResultResponse200Ok()
        {
            var response = File.ReadAllText($"{exampleApiResponesPath}ResultTestResponse200.txt");
            var result = ConverterResult.ConvertReceive(response);
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(result is ResultTestReceiveExpandedModel);
            Assert.AreEqual(3, ((ResultTestReceiveExpandedModel) result).Runs.Count);
        }

        [TestMethod]
        public void TestConvertRequest()
        {
            var response = File.ReadAllText($"{exampleApiResponesPath}RunTestResponse200.txt");
            var result = ConverterResult.ConvertRequest(response);
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(!string.IsNullOrEmpty(result.JsonUrl));
        }
    }
}