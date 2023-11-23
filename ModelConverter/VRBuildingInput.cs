using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverter
{
    public class VRBuildingInputClass
    {
        public Wall[] walls { get; set; }
    }

    public class Wall
    {
        public int id { get; set; }
        public Corner[] corners { get; set; }
    }

    public class Corner
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
}
