using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;

namespace Context.DAL.Data.DataPoints
{
    public class ModbusDataPoint : DataPoint
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public RegisterType RegisterType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public ReadingType ReadingType { get; set; }

        public int Register { get; set; }

        public int RegisterCount { get; set; }
    }

    public enum RegisterType
    {
        InputRegister,
        HoldingRegister,
        Coil,
        InputStatus
    }

    public enum ReadingType
    {
        LowToHigh,
        HighToLow,
    }
}
