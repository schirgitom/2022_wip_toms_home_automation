using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.DAL.Visuals
{
    public class BinaryDataPointVisuals : DataPointVisual
    {
        public BinaryDataPointVisuals()
        {

        }


        public List<BinaryValueMapping> ValueMapping { get; set; } = new();
    }
}
