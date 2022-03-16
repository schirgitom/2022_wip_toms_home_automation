using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.DAL.Data.DataPoints
{
    public class MQTTDataPoint : DataPoint
    {
        public String TopicName { get; set; }
    }
}
