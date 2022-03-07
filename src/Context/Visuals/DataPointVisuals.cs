using Context.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.DAL.Visuals
{
    public abstract class DataPointVisual
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public String Icon { get; set; }

    }
}
