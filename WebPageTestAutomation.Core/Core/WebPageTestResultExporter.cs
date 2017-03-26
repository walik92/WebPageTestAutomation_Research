using System;
using System.IO;
using System.Threading.Tasks;
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

        public async Task Save(ResultTestReceiveExpandedModel result, string fileName)
        {
            if (result == null)
                throw new ArgumentException("Result test of page can't be empty");
            if (result.Details == null)
                throw new ArgumentException("Result details test of page can't be empty");
            if (result.Details.Runs == null)
                throw new ArgumentException("Result details runs test of page can't be empty");

            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);

            using (var outputFile = new StreamWriter(GetPathFile(fileName), false))
            {
                outputFile.WriteLine(result.Details.Url);
                outputFile.WriteLine("#;FirstView Bytes;FirstView LoadTime[ms];" +
                                     "RepeatView Bytes;RepeatView LoadTime[ms]");
                foreach (var run in result.Details.Runs)
                    await outputFile.WriteLineAsync(
                        $"{run.Key};{run.Value.FirstView.BytesOut};{run.Value.FirstView.LoadTime};" +
                        $"{run.Value.RepeatView.BytesOut};{run.Value.RepeatView.LoadTime}");
            }
        }

        private string GetPathFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("FileName value can't be empty");
            return $"{_folder}{fileName}{FileExtension}";
        }
    }
}