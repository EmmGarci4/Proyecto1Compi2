using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.Analisis;
using Proyecto1Compi2.com.AST;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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

		}

		private void EjecutarHanoi()
		{
			Console.WriteLine("Hanoi 1,2,3\n************************");
			Hanoi(4, 1, 2, 3);
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
			Console.WriteLine("m= " + m + " n=" + n);
			if (m == 0)
			{

				return (n + 1);
			}
			else if (n == 0)
			{
				int nn = Ackermann(m - 1, 1);
				Console.WriteLine("nn "+nn);
				return nn;
			}
			else
			{
				int ack1 = Ackermann(m, n - 1);
				Console.WriteLine("ack1 " + ack1);
				int ack2 = Ackermann(m - 1, ack1);
				Console.WriteLine("ack2 " + ack2);
				return ack2;
			}
		}

		String texto = "";
		private void Bt_EjecutarLup_Click_1(object sender, EventArgs e)
		{
			texto = this.richTextBox1.Text;
			richTextBox2.Clear();

			/*ThreadStart delegado = new ThreadStart(ejecutar);
			Thread hilo = new Thread(delegado, 80000000);
			hilo.Start();
			*/
			ejecutar();
		}

		public void ejecutar() {

			//acciones de analisis y ejecucion
			Sesion sesion = new Sesion("admin", null);
			bool res = Analizador.AnalizarCql(texto, sesion);
		//	MethodInvoker action = delegate
		//	{
				//acciones que interactuan con la interface
				//paquete de resultados
				foreach (ResultadoConsulta resultado in Analizador.ResultadosConsultas)
				{
					richTextBox2.AppendText(resultado.ToString());
				}
				//mensajes
				foreach (String mensaje in sesion.Mensajes)
				{
					richTextBox2.AppendText(mensaje);
					richTextBox2.AppendText("\n\r");
				}
				//errores lup
				foreach (Error error in Analizador.ErroresCQL)
				{
					richTextBox2.AppendText(error.ToString());
				}
		//	};

		//	BeginInvoke(action);
		}

		private void Btn_LimpiarDB_Click(object sender, EventArgs e)
		{
			Analizador.Clear();
			richTextBox2.Clear();
		}

		private void Btn_GenerarArchivos_Click(object sender, EventArgs e)
		{
			Analizador.GenerarArchivos("ArchivoPrincipal.txt");
		}

		public static void MostrarMensajeAUsuario(string mensaje)
		{
			//richTextBox2.AppendText(mensaje);
			//richTextBox2.AppendText("\r\n");
		}

		private void toolStripButton1_Click_1(object sender, EventArgs e)
		{
			//Console.WriteLine("Ackermann con 0,1: " + Ackermann(0, 1));
			//Console.WriteLine("Ackermann con 1,0: " + Ackermann(1, 0));
			//Console.WriteLine("Ackermann con 1,3: " + Ackermann(1, 3));
			//Console.WriteLine("Ackermann con 2,4: " + Ackermann(2, 4));
			//Console.WriteLine("Ackermann con 5,5: " + Ackermann(5, 5));
		}

		private void toolStripButton1_Click_2(object sender, EventArgs e)
		{
			richTextBox2.Clear();
			GeneradorLup.Analizar(this.richTextBox1.Text);
			richTextBox2.AppendText(GeneradorLup.Resultado.ToString());
		}

		private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{

		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			//Console.WriteLine("Ackerman con 3,6: " + Ackermann(3, 6));
			int a = -214748364;
			Console.WriteLine("holiiiis");
			Console.WriteLine(a);
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			Analizador.ClearToRollback();

				if (Analizador.AnalizarChison(this.richTextBox1.Text))
				{
					Console.WriteLine("ARCHIVO CARGADO CON EXITO");
				}
				else
				{
					Console.WriteLine("ARCHIVO CARGADO CON ERRORES");
					Analizador.Errors.MostrarCabecera();
					Analizador.Errors.MostrarDatos();
				}
		}
	}
}
