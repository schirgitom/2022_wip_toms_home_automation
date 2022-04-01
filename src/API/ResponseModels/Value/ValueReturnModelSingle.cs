using Context.DAL.InfluxDB;
using DataCollector.ReturnModels.Visuals;

namespace DataCollector.ReturnModels
{
    public class ValueReturnModelSingle : ValueReturnModelBase
    {
        public Sample Sample { get; set; }
        public VisualsReturnModel Visuals { get; set; }
    }
}
