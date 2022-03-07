using Context.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.DAL.Alarming
{
    public class AlarmListEntry
    {
        public DateTime ActiveDate { get; set; }
        public DateTime DeactiveDate { get; set; }
        public AlarmType AlarmType { get; set; }
        public String AlarmText { get; set; }
        public AlarmStatus AlarmStatus { get; set; }
        public AcknowledgeStatus AcknowledgeStatus { get; set; }
        public User AcknowledgeUser { get; set; }
        public String AcknowledgeComment { get; set; }
        public DateTime AcknowledgeDate { get; set; }
        public DataPoint DataPoint { get; set; }
    }

    public enum AcknowledgeStatus
    {
        Acknowledged,
        NotAcknowledged
    }

    public enum AlarmStatus
    {
        Active,
        Deactive
    }
}
