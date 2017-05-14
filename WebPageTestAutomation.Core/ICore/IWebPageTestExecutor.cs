﻿using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WebPageTestAutomation.Core.Enumerators;
using WebPageTestAutomation.Core.Models;

namespace WebPageTestAutomation.Core.ICore
{
    public interface IWebPageTestExecutor
    {
        IList<Connection> Connections { get; set; }
        IList<Browser> Browsers { get; set; }
        int NumberRunsTest { get; set; }
        int RefreshIntervalTime { get; set; }
        Task Execute(IList<PageModel> pages);
    }
}