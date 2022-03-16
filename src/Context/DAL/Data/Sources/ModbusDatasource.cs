using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.DAL.Data.Sources
{
    public class ModbusDatasource : DataSource
    {
        public String Host { get; set; }
        public int Port { get; set; }
        public int SlaveID { get; set; } = 1;
    }
}
