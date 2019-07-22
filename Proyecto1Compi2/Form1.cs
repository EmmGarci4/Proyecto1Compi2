using com.Analisis;
using com.Analisis.Util;
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
	}
}
