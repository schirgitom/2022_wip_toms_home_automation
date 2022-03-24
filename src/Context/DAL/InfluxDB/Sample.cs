using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.DAL.InfluxDB
{
    public abstract class Sample
    {
        public String Tag { get; set; }
        public DateTime TimeStamp { get; set; }
        public virtual Object Value { get; set; }
        public abstract Boolean AsBoolean();
        public abstract float AsFloat();
    }
}
