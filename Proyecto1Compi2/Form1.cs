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
			if (Analizador.AnalizarCql(this.textBox1.Text)) //si no hay ErroresCQL sintácticos/léxicos
			{
				if (Analizador.ErroresCQL.Count==0) {
					//si no hay ErroresCQL semánticos
					this.textBox2.Text = "Finalizado con éxito\n";
				}
				else
				{
					foreach (Error er in Analizador.ErroresCQL)
					{
						this.textBox2.AppendText(er+"\n");
					}
				}
				generadorDOT.GenerarDOT(Analizador.Raiz, "C:\\Users\\Emely\\Desktop\\exp.dot");
			}
			else
			{
				foreach (Error er in Analizador.ErroresCQL)
				{
					this.textBox2.AppendText(er + "\n");
				}
			}
		}

		private void Btn_leerXml_Click(object sender, EventArgs e)
		{

			this.textBox2.Clear();
				if (Analizador.AnalizarChison(this.textBox1.Text))
				{
					this.textBox2.Text = "Finalizado con éxito\n";
					//generadorDOT.GenerarDOT(Analizador.Raiz, "C:\\Users\\Emely\\Desktop\\chison.dot");
				}
				else
				{
					//foreach (Error er in Analizador.ErroresCHISON)
					//{
					//	this.textBox2.AppendText(er + "\n");
					//}
				}
		}

		private void Bt_EjecutarLup_Click_1(object sender, EventArgs e)
		{
			this.textBox2.Clear();
			if (Analizador.AnalizarLup(this.textBox1.Text)) //si no hay ErroresCQL sintácticos/léxicos
			{
				if (Analizador.ErroresCQL.Count == 0)
				{
					//si no hay ErroresCQL semánticos
					this.textBox2.Text = "Finalizado con éxito\n";
				}
				else
				{
					foreach (Error er in Analizador.ErroresCQL)
					{
						this.textBox2.AppendText(er + "\n");
					}
				}
				generadorDOT.GenerarDOT(Analizador.Raiz, "C:\\Users\\Emely\\Desktop\\exp.dot");
			}
			else
			{
				foreach (Error er in Analizador.ErroresCQL)
				{
					this.textBox2.AppendText(er + "\n");
				}
			}
		}

		private void Btn_LimpiarDB_Click(object sender, EventArgs e)
		{
			Analizador.Clear();
		}

		private void Btn_GenerarArchivos_Click(object sender, EventArgs e)
		{
			Analizador.GenerarArchivos("ArchivoPrincipal.txt");
		}

		private void Btn_cargarChison_Click(object sender, EventArgs e)
		{
			this.textBox2.Clear();
			String chi = HandlerFiles.AbrirArchivo(Analizador.PATH + "principal.chison");
			if (chi != null)
			{
				if (Analizador.AnalizarChison(chi))
				{
					this.textBox2.Text = "Finalizado con éxito\n";
					//generadorDOT.GenerarDOT(Analizador.Raiz, "C:\\Users\\Emely\\Desktop\\chison.dot");
				}
				else
				{
					//foreach (Error er in Analizador.)
					//{
					//	this.textBox2.AppendText(er + "\n");
					//}
				}
			}
		}
	}
}
