using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using WebPageTestAutomation.Core.Core;
using WebPageTestAutomation.Core.Enumerators;
using WebPageTestAutomation.Core.Models;

namespace WebPageTestAutomation.Console
{
    internal class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        private static void Main(string[] args)
        {
            #region configuration

            var pages = new List<PageModel>
            {
                new PageModel {Url = "https://onet.pl", Name = "onet"}
            };
            var browsers = new List<Browser> {Browser.Chrome};
            var connections = new List<Connection> {Connection.Cable, Connection.ThreeG};
            var numberRunsTest = 1;
            var refreshIntervalTime = 5; //sekunds
            var baseAddress = "http://localhost/";
            var resultPath = @"Result\";

            #endregion

            var executor = new WebPageTestExecutor(Logger,
                new WebPageTestApiService(baseAddress),
                new WebPageTestResultExporter(resultPath),
                refreshIntervalTime);

            Task.Run(async () =>
                {
                    try
                    {
                        await executor.Execute(pages, browsers, connections, numberRunsTest);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message, ex);
                    }
                })
                .GetAwaiter()
                .GetResult();
        }
    }
}