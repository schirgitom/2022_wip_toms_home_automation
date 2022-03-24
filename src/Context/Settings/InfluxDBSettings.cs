using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.Settings
{
    public class InfluxDBSettings
    {
        public String Bucket { get; set; }
        public String Server { get; set; }
        public int Port { get; set; }
        public String Token { get; set; }
        public String Organization { get; set; }
    }
}
