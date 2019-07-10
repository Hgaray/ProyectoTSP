using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel;
using System.IO;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

using System.Net.Http;
using GeoAPI.Geometries;
using Itinero;
using Itinero.IO.Osm;
using Itinero.LocalGeo;
using Itinero.Osm.Vehicles;
using Newtonsoft.Json;
using System.Web.UI.WebControls;

namespace ProyectoTSP
{
    public partial class BaseDeDatos : Form
    {
        GMarkerGoogle marker;
        GMapOverlay markerOverlay;
        DataTable dt;
        float[] latitudInicial = { 6.283984F, 6.251868F, 6.249969F, 6.252721F, 6.247398F, 6.245836F, 6.244805F, 6.244442F, 6.246524F, 6.241526F, 6.243165F, 6.246579F, 6.240901F, 6.243165F };
        float longitudInicial = -75.608310F;

        List<double> listLat = new List<double>();
        List<double> listLng = new List<double>();
        List<string> strGeometries = new List<string>();
        List<string> strNameGeometries = new List<string>();
        string[] coordenadasIndividuales;



        Itinero.LocalGeo.Coordinate[] locations;

        public BaseDeDatos()
        {
            InitializeComponent();


            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CanDragMap = true;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.Position = new PointLatLng(6.251868, -75.596144);
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 24;
            gMapControl1.Zoom = 9;
            gMapControl1.AutoScroll = true;

            strNameGeometries.Add("StartPoint");
            strNameGeometries.Add("StartPoint");

        }
        #region Leer base de datos en DataGridView
        DataSet result;
        private void BtnAbrir_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Libro de Excel 97-2003|*.xls|Libro de Excel |*.xlsx", ValidateNames = true })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    FileStream fs = File.Open(ofd.FileName, FileMode.Open, FileAccess.Read);
                    IExcelDataReader reader;
                    if (ofd.FilterIndex == 1)
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(fs);
                    }
                    else
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                    }
                    reader.IsFirstRowAsColumnNames = true;
                    result = reader.AsDataSet();
                    cboxHojas.Items.Clear();
                    foreach (DataTable dt in result.Tables)
                        cboxHojas.Items.Add(dt.TableName);
                    reader.Close();
                }
            }
        }
        private void CboxHojas_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = result.Tables[cboxHojas.SelectedIndex];

            #region Leer Base de datos en matriz
            string[,] matriz = new string[dataGridView1.Rows.Count, dataGridView1.Columns.Count];

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    matriz[i, j] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                }
            }
            //listLat listLng

            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                strGeometries.Add(item.Cells[1].Value.ToString());
                strNameGeometries.Add(item.Cells[0].Value.ToString());
            };


            locations = new Itinero.LocalGeo.Coordinate[strGeometries.Count() + 2];

            int contador = 2;


            string coorinicio = txtInicio.Text;

            coordenadasIndividuales = coorinicio.Split(',');
            locations[0] = new Itinero.LocalGeo.Coordinate(float.Parse(coordenadasIndividuales[0].ToString()), float.Parse(coordenadasIndividuales[1].ToString()));
            locations[1] = new Itinero.LocalGeo.Coordinate(float.Parse(coordenadasIndividuales[0].ToString()), float.Parse(coordenadasIndividuales[1].ToString()));

            foreach (var item in strGeometries)
            {
                coordenadasIndividuales = item.Split(',');
                locations[contador] = new Itinero.LocalGeo.Coordinate(float.Parse(coordenadasIndividuales[0].ToString()), float.Parse(coordenadasIndividuales[1].ToString()));
                contador++;
            }


           // label2.Text = matriz[4, 1];
            #endregion
        }


        /// <summary>
        /// Metodo para obtener mapa (Archivo .pbf ) y calcular ruta optima con las geometrias obtenidas
        /// </summary>
        /// <returns></returns>
        public string CalculrRutaOptima()
        {

            // enable logging.
            OsmSharp.Logging.Logger.LogAction = (o, level, message, parameters) =>
            {
                Console.WriteLine(string.Format("[{0}] {1} - {2}", o, level, message));
            };
            Itinero.Logging.Logger.LogAction = (o, level, message, parameters) =>
            {
                Console.WriteLine(string.Format("[{0}] {1} - {2}", o, level, message));
            };

            var routerDb = new RouterDb();

            ///Archivo con Mapa , debe estar creado en C:\MapasDb\MapaDb.pbf
            using (var stream = new FileInfo(@"/MapasDb/MapaDb.pbf").OpenRead())
            {
                routerDb.LoadOsmData(stream, Vehicle.Car);
            }

            ///Forma de recorrer la ruta ooptima
            var car = routerDb.GetSupportedProfile("car");

            // add a contraction hierarchy.
            routerDb.AddContracted(car);

            // create router.
            var router = new Router(routerDb);
            
            ///Metodo que entrega la ruta mas optima segun las coordenadas indicadas en locations
            var route = router.Calculate(car, locations);
            var routeGeoJson = route.ToGeoJson();

            return routeGeoJson;

        }


        /// <summary>
        /// Evento que se encarga de disparar los eventos necesarios para recolectar informacion, enviarla a procesar y mostrarlas graficada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalcular_Click(object sender, EventArgs e)
        {
            string OptimeRoute = CalculrRutaOptima();
            RootObject resultCalculate = JsonConvert.DeserializeObject<RootObject>(OptimeRoute);
            GMapOverlay Ruta = new GMapOverlay("CapaRuta");
            List<PointLatLng> puntos = new List<PointLatLng>();

            double lng, lat;
            foreach (var item in resultCalculate.features)
            {
                foreach (var item2 in item.geometry.coordinates)
                {
                    try
                    {
                        lng = Convert.ToDouble(((Newtonsoft.Json.Linq.JContainer)item2).First.ToString());
                        lat = Convert.ToDouble(((Newtonsoft.Json.Linq.JContainer)item2).Last.ToString());
                        puntos.Add(new PointLatLng(lat, lng));
                    }
                    catch (Exception)
                    {


                    }

                }
            }

            puntos.Add(puntos[0]);

            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CanDragMap = true;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.Position = puntos[0];
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 24;
            gMapControl1.Zoom = 9;
            gMapControl1.AutoScroll = true;

            GMapRoute PuntosRuta = new GMapRoute(puntos, "Ruta");
            Ruta.Routes.Add(PuntosRuta);
            gMapControl1.Overlays.Add(Ruta);

            markerOverlay = new GMapOverlay("Marcador");

            string[] coordenadasFile;
            int marcador = 0;
            foreach (var item in locations)
            {
                if (marcador > 0) ///para no repetir el punto d epartida
                {
                    coordenadasFile = item.ToString().Split(',');
                    marker = new GMarkerGoogle(new PointLatLng(Convert.ToDouble(coordenadasFile[0].ToString()), Convert.ToDouble(coordenadasFile[1].ToString())), GMarkerGoogleType.green);

                    marker.ToolTipMode = MarkerTooltipMode.Always;
                    marker.ToolTipText = string.Format("Ubicación \n {0}", strNameGeometries[marcador].ToString()); ;
                    markerOverlay.Markers.Add(marker);

                    gMapControl1.Overlays.Add(markerOverlay);

                }
               
                marcador++;
            }

            gMapControl1.Zoom = gMapControl1.Zoom + 1;
            gMapControl1.Zoom = gMapControl1.Zoom - 1;
        }
    }
}
#endregion

