using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoTSP
{
    public class Geometry
    {
        public string type { get; set; }
        public List<object> coordinates { get; set; }
    }

    public class properties
    {
        public string name { get; set; }
        public string highway { get; set; }
        public string maxspeed { get; set; }
        public string profile { get; set; }
        public string distance { get; set; }
        public string time { get; set; }
        public string oneway { get; set; }
        public string index { get; set; }
    }

    public class Feature
    {
        public string type { get; set; }
        public string name { get; set; }
        public Geometry geometry { get; set; }
        public properties properties { get; set; }
        public string Shape { get; set; }
    }

    public class RootObject
    {
        public string type { get; set; }
        public List<Feature> features { get; set; }
    }
}
