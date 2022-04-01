namespace DataCollector.ReturnModels.Visuals
{
    public class VisualsNumericReturnModel : VisualsReturnModel
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public String Unit { get; set; }
        public Dictionary<String, VisualsThresholdModel> Thresholds { get; set; } = new();
    }
}
