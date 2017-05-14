using System;
using System.IO;
using System.Threading.Tasks;
using WebPageTestAutomation.Core.Enumerators;
using WebPageTestAutomation.Core.ICore;
using WebPageTestAutomation.Core.Models;

namespace WebPageTestAutomation.Core.Core
{
    public class WebPageTestResultExporter : IWebPageTestResultExporter
    {
        private readonly string _folder;

        /// <summary>
        ///     Save result of page test to file
        /// </summary>
        /// <param name="folder">Folder path to save result</param>
        public WebPageTestResultExporter(string folder)
        {
            _folder = folder;
        }

        private string FileExtension { get; } = ".txt";

        public async Task Save(ResultTestReceiveExpandedModel result, PageModel page)
        {
            if (result == null)
                throw new ArgumentException("Result test of page can't be empty");

            if (result.Runs == null)
                throw new ArgumentException("Result runs test of page can't be empty");

            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);

            using (var outputFile = new StreamWriter(GetPathFile(page.Name, result.Browser, result.Connection), false))
            {
                await outputFile.WriteLineAsync($"URL: {result.Url}");
                await outputFile.WriteLineAsync($"Framework: {page.Name} v{page.Version}");
                await outputFile.WriteLineAsync($"KBytes of JS: {result.KBytes} kB");
                await outputFile.WriteLineAsync(Environment.NewLine);
                await outputFile.WriteLineAsync("#;TTFB [ms];Render Start[ms];" +
                                                "Visually Complete[ms];Load TIme[ms];Speed Index");
                foreach (var run in result.Runs)
                    await outputFile.WriteLineAsync(
                        $"{run.Id};{run.Ttfb};{run.RenderStart};{run.VisuallyComplete};" +
                        $"{run.LoadTime};{run.SpeedIndex}");
            }
        }

        private string GetPathFile(string name, string browser, string connection)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name value can't be empty");
            return $"{_folder}{name}_{browser}_{connection}{ FileExtension}";

        }
    }
}