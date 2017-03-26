using System.Collections.Generic;
using System.Threading.Tasks;
using WebPageTestAutomation.Core.Enumerators;
using WebPageTestAutomation.Core.Models;

namespace WebPageTestAutomation.Core.ICore
{
    public interface IWebPageTestExecutor
    {
        Task Execute(IList<PageModel> pages, IList<Browser> browsers, IList<Connection> connections, int numberOfRuns);
    }
}