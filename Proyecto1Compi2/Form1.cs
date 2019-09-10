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
			//EjecutarAckermann();
			//EjecutarHanoi();
			textBox2.Clear();
			Analizador.AddUsuario(new Usuario("admin", "admin"));
			Sesion sesion = new Sesion("admin", null);
			if (Analizador.AnalizarCql(this.richTextBox1.Text,sesion)) //si no hay ErroresCQL sintácticos/léxicos
			{
				if (Analizador.ErroresCQL.Count==0) {
					//si no hay ErroresCQL semánticos
					textBox2.AppendText("Finalizado con éxito\n");
				}
				else
				{
					foreach (Error er in Analizador.ErroresCQL)
					{
						textBox2.AppendText(er.ToString());
					}
				}
			}
			else
			{
				foreach (Error er in Analizador.ErroresCQL)
				{
					textBox2.AppendText(er.ToString());
				}
			}
		}

		private void EjecutarHanoi()
		{
			Console.WriteLine("Hanoi 1,2,3\n************************");
			Hanoi(4,1, 2, 3);
		}

		//Método Torres de Hanoi Recursivo
		void Hanoi(int n, int @origen, int auxiliar, int destino)
		{
			if (n == 1)
				Console.WriteLine("mover disco de " + @origen + " a " + @destino);
			else
			{
				Hanoi(@n - 1, origen, destino, auxiliar);
				Console.WriteLine("mover disco de " + origen + " a " + destino);
				Hanoi(n - 1, auxiliar, origen, destino);
			}
		}

	private void EjecutarAckermann()
		{
			Console.WriteLine("Ackermann con 0,1: " + Ackermann(0, 1));
			Console.WriteLine("Ackermann con 1,0: " + Ackermann(1, 0));
			Console.WriteLine("Ackermann con 1,3: " + Ackermann(1, 3));
			Console.WriteLine("Ackermann con 2,4: " + Ackermann(2, 4));
			Console.WriteLine("Ackermann con 5,5: " + Ackermann(5, 5));
		}

		private int Ackermann(int m, int n)
		{
			if (m == 0)
				return (n + 1);
			else if (n == 0)
				return (Ackermann(m - 1, 1));
			else
				return (Ackermann(m - 1, Ackermann(m, n - 1)));
		}

		private void Btn_leerXml_Click(object sender, EventArgs e)
		{

			textBox2.Clear();
				if (Analizador.AnalizarChison(this.richTextBox1.Text))
				{
					textBox2.Text = "Finalizado con éxito\n";
				}
				else
				{
				foreach (Error er in Analizador.ErroresChison)
				{
					textBox2.AppendText(er + "\n");
				}
			}
		}

		private void Bt_EjecutarLup_Click_1(object sender, EventArgs e)
		{
			textBox2.Clear();
			if (Analizador.AnalizarLup(this.richTextBox1.Text)) //si no hay ErroresCQL sintácticos/léxicos
			{
				if (Analizador.ErroresCQL.Count == 0)
				{
					//si no hay ErroresCQL semánticos
					textBox2.Text = "Finalizado con éxito\n";
				}
				else
				{
					foreach (Error er in Analizador.ErroresCQL)
					{
						textBox2.AppendText(er.ToString());
					}
				}
				generadorDOT.GenerarDOT(Analizador.Raiz, "C:\\Users\\Emely\\Desktop\\exp.dot");
			}
			else
			{
				foreach (Error er in Analizador.ErroresCQL)
				{
					textBox2.AppendText(er.ToString());
				}
			}
		}

		private void Btn_LimpiarDB_Click(object sender, EventArgs e)
		{
			Analizador.Clear();
			textBox2.Clear();
		}

		private void Btn_GenerarArchivos_Click(object sender, EventArgs e)
		{
			Analizador.GenerarArchivos("ArchivoPrincipal.txt");
		}

		private void Btn_cargarChison_Click(object sender, EventArgs e)
		{
			textBox2.Clear();
			String chi = HandlerFiles.AbrirArchivo(Analizador.PATH + "principal.chison");
			if (chi != null)
			{
				if (Analizador.AnalizarChison(chi))
				{
					textBox2.Text = "Finalizado con éxito\n";
				}
				else
				{
					textBox2.Text="Finalizado con errores";
					
				}
			}
		}

		public static void MostrarMensajeAUsuario(string mensaje) {
			textBox2.AppendText(mensaje);
			textBox2.AppendText("\r\n");
		}
	}
}
