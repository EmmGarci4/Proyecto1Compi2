using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.AST;
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

		private void ToolStripButton1_Click(object sender, EventArgs e)
		{
			this.textBox2.Clear();
			if (Analizador.AnalizarCql(this.textBox1.Text)) //si no hay errores sintácticos/léxicos
			{
				if (Analizador.Errores.Count==0) {
					//si no hay errores semánticos
					this.textBox2.Text = "Finalizado con éxito\n";
				}
				else
				{
					foreach (Error er in Analizador.Errores)
					{
						this.textBox2.AppendText(er+"\n");
					}
				}
				generadorDOT.GenerarDOT(Analizador.Raiz, "C:\\Users\\Emely\\Desktop\\exp.dot");
			}
			else
			{
				foreach (Error er in Analizador.Errores)
				{
					this.textBox2.AppendText(er + "\n");
				}
			}
		}

		private void ToolStripButton2_Click(object sender, EventArgs e)
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
			List<Celda> cls = new List<Celda>
			{
				new Celda("12", TipoDatoDB.INTEGER),
				new Celda("Matematica", TipoDatoDB.TEXT),
				new Celda("Descripcion", TipoDatoDB.TEXT)
			};

			db.Insertar("Clases",cls);//tabla-fila de valores

			cls = new List<Celda>
			{
				new Celda("1", TipoDatoDB.INTEGER),
				new Celda("Ciencias", TipoDatoDB.TEXT)
			};

			List<string> columnas = new List<string>
			{
				"codigo",
				"Nombre"
			};
			db.Insertar("Clases", columnas,cls);//tabla-fila de valores
			List<Simbolo> atributos= new List<Simbolo> {
				new Simbolo("holi","12",TipoDatoDB.INTEGER,1,1),
				new Simbolo("holi1","12",TipoDatoDB.INTEGER,1,1),
				new Simbolo("holi2","12",TipoDatoDB.INTEGER,1,1)
			};
			db.Objetos.Add(new com.AST.Objeto("Persona",atributos));
			atributos = new List<Simbolo> {
				new Simbolo("holi","12",TipoDatoDB.INTEGER,1,1),
				new Simbolo("holi1","12",TipoDatoDB.TEXT,1,1),
				new Simbolo("holi2","12",TipoDatoDB.DATETIME,1,1)
			};
			db.Procedimientos.Add(new Procedimiento("MarcoProc",atributos));
			db.MostrarBaseDatos();
			db.GenerarArchivo();
		}

		private void Btn_leerXml_Click(object sender, EventArgs e)
		{
			this.textBox2.Clear();
			if (Analizador.AnalizarChison(this.textBox1.Text))
			{
				this.textBox2.Text = "Finalizado con éxito\n";
				generadorDOT.GenerarDOT(Analizador.Raiz, "C:\\Users\\Emely\\Desktop\\xml.dot");
			}
			else
			{
				foreach (Error er in Analizador.Errores)
				{
					this.textBox2.AppendText(er + "\n");
				}
			}
		}

		private void Btn_Probar_Click(object sender, EventArgs e)
		{
			Operacion op = new Operacion(new Operacion("12",TipoOperacion.Numero,1,1), new Operacion("12", TipoOperacion.Numero,1,1),
				TipoOperacion.Suma,1,1);
			Console.WriteLine("Resultado="+op.GetValor(new TablaSimbolos(0,"global")).ToString());
		}

		private void toolStripButton1_Click_1(object sender, EventArgs e)
		{
			this.textBox2.Clear();
			if (Analizador.AnalizarLup(this.textBox1.Text)) //si no hay errores sintácticos/léxicos
			{
				if (Analizador.Errores.Count == 0)
				{
					//si no hay errores semánticos
					this.textBox2.Text = "Finalizado con éxito\n";
				}
				else
				{
					foreach (Error er in Analizador.Errores)
					{
						this.textBox2.AppendText(er + "\n");
					}
				}
				generadorDOT.GenerarDOT(Analizador.Raiz, "C:\\Users\\Emely\\Desktop\\exp.dot");
			}
			else
			{
				foreach (Error er in Analizador.Errores)
				{
					this.textBox2.AppendText(er + "\n");
				}
			}
		}
	}
}
