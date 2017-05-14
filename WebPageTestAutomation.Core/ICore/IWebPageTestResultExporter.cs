using System.Threading.Tasks;
using WebPageTestAutomation.Core.Enumerators;
using WebPageTestAutomation.Core.Models;

namespace WebPageTestAutomation.Core.ICore
{
    public interface IWebPageTestResultExporter
    {
        Task Save(ResultTestReceiveExpandedModel result, PageModel page, Browser browser, Connection connection);
    }
}