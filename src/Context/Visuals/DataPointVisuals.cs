using Context.DAL.Data;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.DAL.Visuals
{
    public abstract class DataPointVisual : MongoDocument
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public String Icon { get; set; }


        [InverseSide]
        public Many<DataPoint> DataPoints { get; set; }

    }
}
