using Context.DAL.Data;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.DAL.Visuals
{
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(NumericDataPointVisuals), typeof(BinaryDataPointVisuals))]
    public abstract class DataPointVisual : MongoDocument
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public String Icon { get; set; }

        [BsonIgnore]
        public String Type
        {
            get
            {
                return this.GetType().Name;
            }
        }

        [InverseSide]
        public Many<DataPoint> DataPoints { get; set; }

    }
}
