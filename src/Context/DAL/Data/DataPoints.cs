using Context.DAL.Alarming;
using Context.DAL.Data.DataPoints;
using Context.DAL.Visuals;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.DAL.Data
{
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(MQTTDataPoint), typeof(ModbusDataPoint))]
    public class DataPoint : MongoDocument
    {

        public DataPoint()
        {

        }

        public String DatabaseName { get; set; }
        public String Name { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DataType DataType { get; set; }
        public int Offset { get; set; }
        public String Description { get; set; }
        public Dictionary<String, AlarmThreshold> AlarmThresholds { get; set; } = new();

        //     [InverseSide]
        //   public Many<DataSource> DataSources { get; set; }

        public One<DataSource> DataSource { get; set; }

        public One<DataPointVisual> Internal_Visual { get; set; } = null;

        [BsonIgnore()]
        public DataPointVisual Visual
        {
            get
            {
                if (Internal_Visual != null)
                {
                    Task<DataPointVisual> sk = Internal_Visual.ToEntityAsync();
                    sk.Wait();

                    return sk.Result;
                }

                return null;
            }
        }


    }

    public enum DataType
    {
        Boolean,
        Float
    }
}
