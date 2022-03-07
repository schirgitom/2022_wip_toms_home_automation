
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Context.DAL.Alarming
{
    public class AlarmThreshold
    {
        public object Threashold { get; set; }
        public AlarmType AlarmType { get; set; }
        public AlarmCheckType AlarmCheckType { get; set; }
        public String Message { get; set; }

    }

    public enum AlarmCheckType
    {
        Equal,
        NotEqual,
        BottomUp,
        TopDown
    }

    public enum AlarmType
    {
        Warning,
        Alarm,
        Trip
    }
}
