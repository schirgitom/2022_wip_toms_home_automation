using Context.DAL.Alarming;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DataCollector.ReturnModels.Visuals
{
    public class VisualsThresholdModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public AlarmType AlarmType { get; set; }

        public object Value { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]

        public AlarmCheckType AlarmCheckType { get; set; }


    }
}
