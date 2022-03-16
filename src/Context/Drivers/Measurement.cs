using Context.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.Drivers
{
    public class Measurement
    {
        public DateTime Time { get; set; }
        public Object Value { get; set; }
        public DataPoint DataPointObject { get; set; }
    }
}
