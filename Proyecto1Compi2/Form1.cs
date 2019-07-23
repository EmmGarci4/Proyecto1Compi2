using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto1Compi2
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			this.textBox2.Clear();
			if (Analizador.AnalizarUsql(this.textBox1.Text))
			{
				this.textBox2.Text = "Finalizado con éxito\n";
				 generadorDOT.GenerarDOT(Analizador.Raiz, "C:\\Users\\Emely\\Desktop\\exp.dot");
			}
			else {
				foreach (Error er in Analizador.Errores)
				{
					this.textBox2.AppendText(er.Mensaje+"En linea: "+er.Linea+"y columna: "+er.Columna + "\n");
				}
			}

			
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			BaseDatos db = new BaseDatos("MiBase", "C:\\Users\\Emely\\Desktop\\miBase.txt");
			Tabla tb = new Tabla("Alumnos", "C:\\Users\\Emely\\Desktop\\miBase_AlumnosTb.txt");
			tb.AgregarColumna(new Columna("carnet", TipoDatoDB.INTEGER));
			tb.AgregarColumna(new Columna("nombre", TipoDatoDB.TEXT));
			tb.AgregarColumna(new Columna("grado", TipoDatoDB.INTEGER));
			db.AgregarTabla(tb);
			tb = new Tabla("Clases", "C:\\Users\\Emely\\Desktop\\miBase_ClasesTb.txt");
			tb.AgregarColumna(new Columna("codigo", TipoDatoDB.INTEGER));
			tb.AgregarColumna(new Columna("Nombre", TipoDatoDB.TEXT));
			tb.AgregarColumna(new Columna("Descripcion", TipoDatoDB.TEXT));
			db.AgregarTabla(tb);
			List<Celda> cls = new List<Celda>();
			cls.Add(new Celda("12",TipoDatoDB.INTEGER));
			cls.Add(new Celda("Matematica", TipoDatoDB.TEXT));
			cls.Add(new Celda("Descripcion", TipoDatoDB.TEXT));

			db.Insertar("Clases",cls);//tabla-fila de valores

			cls = new List<Celda>();
			cls.Add(new Celda("1", TipoDatoDB.INTEGER));
			cls.Add(new Celda("Ciencias", TipoDatoDB.TEXT));

			List<string> columnas = new List<string>();
			columnas.Add("codigo");
			columnas.Add("Nombre");
			db.Insertar("Clases", columnas,cls);//tabla-fila de valores

			db.MostrarBaseDatos();
		}
	}
}
