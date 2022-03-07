using Context.DAL.Alarming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.DAL.Data
{
    public class DataPoint
    {

        public DataPoint()
        {

        }

        public String DatabaseName { get; set; }
        public String Name { get; set; }
        public DataType DataType { get; set; }
        public int Offset { get; set; }
        public String Description { get; set; }
        public Dictionary<String, AlarmThreshold> AlarmThresholds { get; set; } = new();


    }

    public enum DataType
    {
        Boolean,
        Float
    }
}
