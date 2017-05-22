using System.Threading.Tasks;
using WebPageTestAutomation.Core.Enumerators;

namespace WebPageTestAutomation.Core.ICore
{
    public interface IWebPageTestApiService
    {
        Task<string> GetResultOfTestAsync(string urlTestResult);
        Task<string> SendTestAsync(string urlPage, Browser browser, Connection connection, int numberRuns);
        Task<string> SendTestAsync(string urlPage, Mobile mobile, int numberRuns);
    }
}