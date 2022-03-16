
using Context.DAL.Data.Sources;
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
    [BsonKnownTypes(typeof(MQTTDatasource), typeof(ModbusDatasource))]
    public class DataSource : MongoDocument
    {

 

        public DataSource()   {
            this.InitOneToMany(() => Internal_DataPoints);
        }

        public Boolean Active { get; set; }

        public String Name { get; set; }

        public Many<DataPoint> Internal_DataPoints { get; set; } = new();

        [BsonIgnore]
        public List<DataPoint>? DataPoints
        {
            get
            {
                try
                {
                    if (Internal_DataPoints != null)
                    {
                        return Internal_DataPoints.ToList();
                    }
                }
                catch (Exception e)
                {
                    return null;
                }

                return null;
            }
        }

    }
}
