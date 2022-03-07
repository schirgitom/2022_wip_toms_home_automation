using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.DAL.Visuals
{
    public class NumericDataPointVisuals : DataPointVisual
    {
        public NumericDataPointVisuals()
        {

        }

        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public String Unit { get; set; }
    }
}
