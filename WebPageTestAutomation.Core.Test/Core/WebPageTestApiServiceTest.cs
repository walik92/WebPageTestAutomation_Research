using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebPageTestAutomation.Core.Core;
using WebPageTestAutomation.Core.Enumerators;

namespace WebPageTestAutomation.Core.Test.Core
{
    /// <summary>
    ///     Summary description for WebPageTestApiServiceTest
    ///     Server WebPageTest must be available
    /// </summary>
    [TestClass]
    public class WebPageTestApiServiceTest
    {
        private readonly string _baseAddress = "http://localhost";
        private string _correctUrlTest = "http://localhost/jsonResult.php?test=170323_CT_N";

        [TestMethod]
        public async Task TestSendTestAsync1()
        {
            var service = new WebPageTestApiService(_baseAddress);
            var result = await service.SendTestAsync("onet.pl", Browser.Firefox, Connection.Cable, 1);
            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task TestSendTestAsync2()
        {
            var service = new WebPageTestApiService(_baseAddress);
            var result = await service.SendTestAsync("onet.pl", Browser.Firefox, Connection.Cable, -1);
            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task TestSendTestAsync3()
        {
            var service = new WebPageTestApiService(_baseAddress);
            var result = await service.SendTestAsync("", Browser.Firefox, Connection.Cable, 1);
            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public async Task TestGetResultOfTestAsync()
        {
            var service = new WebPageTestApiService(_baseAddress);
            var result = await service.GetResultOfTestAsync("http://localhost/jsonResult.php?test=170323_CT_N");
            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }
    }
}