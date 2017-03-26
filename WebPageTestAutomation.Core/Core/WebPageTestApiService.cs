using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebPageTestAutomation.Core.Enumerators;
using WebPageTestAutomation.Core.ICore;

namespace WebPageTestAutomation.Core.Core
{
    public class WebPageTestApiService : IWebPageTestApiService
    {
        private readonly string _baseAddress;

        /// <summary>
        ///     WebPageTest Api Caller
        ///     https://sites.google.com/a/webpagetest.org/docs/advanced-features/webpagetest-restful-apis
        /// </summary>
        /// <param name="baseAddress">Base address private instance WebPageTest</param>
        public WebPageTestApiService(string baseAddress)
        {
            _baseAddress = baseAddress;
        }

        /// <summary>
        ///     Send request to server for execution test of page
        /// </summary>
        /// <param name="urlPage">Address site</param>
        /// <param name="browser">Type of browser</param>
        /// <param name="connection">Type of connection</param>
        /// <param name="numberRuns">Number of test run</param>
        /// <returns>Response in format json</returns>
        public async Task<string> SendTestAsync(string urlPage, Browser browser, Connection connection,
            int numberRuns)
        {
            if (string.IsNullOrEmpty(urlPage) || string.IsNullOrWhiteSpace(urlPage))
                throw new ArgumentException("Url page can't be empty.");
            if (numberRuns < 1)
                throw new ArgumentException($"The number of run tests is invalid. Value: {numberRuns}");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseAddress);

                var httpResponseMessage = await client.GetAsync($"runtest.php?url={urlPage}" +
                                                                "&f=json" +
                                                                $"&location=Test:{browser}.{connection.GetString()}" +
                                                                $"&runs={numberRuns}");

                return await httpResponseMessage.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        ///     Get result of test from server
        /// </summary>
        /// <param name="urlTestResult">Url address where server returns result of test</param>
        /// <returns>Response in format json</returns>
        public async Task<string> GetResultOfTestAsync(string urlTestResult)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseAddress);

                var responseMessage =
                    await client.GetAsync(urlTestResult);

                return await responseMessage.Content.ReadAsStringAsync();
            }
        }
    }
}