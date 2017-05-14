using System;
using System.Collections.Generic;
using System.Linq;
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
            RunTestPages("article-versions", GetActionSetArticleVersions());
            RunTestPages("latest-versions", GetActionSetLatestVersions());
        }

        private static Action<List<PageModel>> GetActionSetArticleVersions()
        {
            return delegate (List<PageModel> pages)
            {
                pages[0].Version = "1.3.6";
                pages[1].Version = "1.1.2";
                pages[2].Version = "1.9.0";
                pages[3].Version = "0.12.2";
            };
        }

        private static Action<List<PageModel>> GetActionSetLatestVersions()
        {
            return delegate (List<PageModel> pages)
            {
                pages[0].Version = "2.0.0";
                pages[1].Version = "1.3.3";
                pages[2].Version = "2.10.0";
                pages[3].Version = "0.14.8";
            };
        }

        private static void RunTestPages(string version, Action<List<PageModel>> setVersions)
        {
            var browsers = new List<Browser> { Browser.Chrome };
            var pages = new List<PageModel>
            {
                new PageModel {Url = $"kaiwoklaw.pl/sites/{version}/angular", Name = "Angular", Version = "1.3.6"},
                new PageModel {Url = $"kaiwoklaw.pl/sites/{version}/backbone", Name = "Backbone"},
                new PageModel {Url = $"kaiwoklaw.pl/sites/{version}/ember", Name = "Ember"},
                new PageModel {Url = $"kaiwoklaw.pl/sites/{version}/react", Name = "React"}
            };

            setVersions(pages);

            var connections = new List<Connection> { Connection.Cable, Connection.ThreeG };
            var numberRunsTest = 20;
            var refreshIntervalTime = 5; //sekunds
            var baseAddress = "http://localhost/";
            var path = $@"Result\{version}\";

            var executor = new WebPageTestExecutor(Logger,
                new WebPageTestApiService(baseAddress),
                new WebPageTestResultExporter(path),
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