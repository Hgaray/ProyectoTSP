using Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoTSP
{
    public partial class Barrios : Form
    {

        public DataSet dsBarrios;
        public List<BarriosDto> barriosList = new List<BarriosDto>();
        public List<BarriosDto> barriosSearch = new List<BarriosDto>();
        public List<string> comunasList = new List<string>();

        public Barrios()
        {
            InitializeComponent();
        }

        private void BtnLoadFile_Click(object sender, EventArgs e)
        {

            try
            {
                

                using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Libro de Excel 97-2003|*.xls|Libro de Excel |*.xlsx", ValidateNames = true })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        barriosList = new List<BarriosDto>();
                        dataGridView1.DataSource = barriosList;
                        comunasList = new List<string>();
                        cbxComunas.DataSource = comunasList;

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
                        dsBarrios = reader.AsDataSet();



                       

                        foreach (DataRow dr in dsBarrios.Tables[0].Rows)
                        {
                            BarriosDto  nuevoBarrio = new BarriosDto()
                            {
                                NombrePersona = dr[0].ToString(),
                                Comuna = dr[1].ToString(),
                                Barrio = dr[2].ToString(),
                                Direccion = dr[3].ToString(),

                            };
                            barriosList.Add(nuevoBarrio);
                        }

                        comunasList = barriosList.Select(x => x.Comuna).ToList();

                        comunasList.Insert(0,"Todos");
                        cbxComunas.DataSource = comunasList;
                        

                        dataGridView1.DataSource = barriosList;

                        reader.Close();
                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Verifica que el archivo contenga la estructura correcta, y no este abierto al momento de cargarlo");
            }
            
        }

        private void CbxComunas_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                if (cbxComunas.Text == "Todos")
                {
                    dataGridView1.DataSource = barriosList;
                }
                else
                {
                    barriosSearch = barriosList.Where(x => x.Comuna == cbxComunas.Text).ToList();
                    dataGridView1.DataSource = barriosSearch;
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Ocurrio algo con la comuna seleccionada");
            }
            

            



        }
    }
}
