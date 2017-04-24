namespace WebPageTestAutomation.Core.Models
{
    public class Run
    {
        public int Id { get; set; }
        public int Ttfb { get; set; }
        public int RenderStart { get; set; }
        public int VisuallyComplete { get; set; }
        public int LoadTime { get; set; }
        public int SpeedIndex { get; set; }
    }
}