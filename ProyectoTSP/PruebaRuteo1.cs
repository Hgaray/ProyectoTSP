using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Itinero;
using Itinero.IO.Osm;
using Itinero.Osm.Vehicles;

namespace ProyectoTSP
{
    public partial class PruebaRuteo1 : Form
    {
        public PruebaRuteo1()
        {
            InitializeComponent();
            // load some routing data and build a routing network.
            var routerDb = new RouterDb();
            using (var stream = new System.IO.FileInfo(@"/path/to/some/osmfile.osm.pbf").OpenRead())
            {
                // create the network for cars only.
                routerDb.LoadOsmData(stream, Vehicle.Car);
            }

            // write the routerdb to disk.
            using (var stream = new System.IO.FileInfo(@"/path/to/some/file.routerdb").Open(System.IO.FileMode.Create))
            {
                routerDb.Serialize(stream);
                routerDb = RouterDb.Deserialize(stream, RouterDbProfile.NoCache);
            }
            

        }
    }
}
