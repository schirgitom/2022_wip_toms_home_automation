using Context.DAL.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.DAL.Alarming
{
    public class AlarmListEntry : MongoDocument
    {
        public DateTime ActiveDate { get; set; }
        public DateTime DeactiveDate { get; set; }
        [BsonRepresentation(BsonType.String)]
        public AlarmType AlarmType { get; set; }
        public String AlarmText { get; set; }
        [BsonRepresentation(BsonType.String)]
        public AlarmStatus AlarmStatus { get; set; }
        [BsonRepresentation(BsonType.String)]
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
