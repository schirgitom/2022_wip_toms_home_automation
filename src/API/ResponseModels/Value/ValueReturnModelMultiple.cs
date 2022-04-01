using Context.DAL.InfluxDB;

namespace DataCollector.ReturnModels
{
    public class ValueReturnModelMultiple : ValueReturnModelBase
    {
        public List<Sample> Samples { get; set; } = new();
    }
}
