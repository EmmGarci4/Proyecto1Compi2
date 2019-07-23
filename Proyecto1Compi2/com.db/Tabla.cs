using com.Analisis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class Tabla
	{
		String nombre;
		String path;
		List<Columna> columnas;
		private int contadorFilas;

		public string Nombre { get => nombre; set => nombre = value; }
		public string Path { get => path; set => path = value; }
		public List<Columna> Columnas { get => columnas; set => columnas = value; }

		public Tabla(String nombre, String path)
		{
			this.nombre = nombre;
			this.path = path;
			this.columnas = new List<Columna>();
		}

		public void AgregarColumna(Columna columna) {
			columnas.Add(columna);
		}

		public void AgregarColumnas(List<Columna> columna)
		{
			this.columnas.AddRange(columna);
		}

		public void LimpiarColumnas() {
			this.columnas.Clear();
		}

		public void MostrarColumnas()
		{
			Console.Write("|");
			foreach (Columna st in this.columnas) {
				Console.Write(st.TipoDato.ToString().ToLower() + ":" + st.Titulo + "|");
			}
			Console.WriteLine();
			//mostrando valores
			int i;
			for (i = 0; i < contadorFilas; i++) {
				Console.Write("|");
				foreach (Columna st in this.columnas)
				{
					Console.Write(st.Valores[i].Valor + "|");
				}
				Console.WriteLine();
			}
		}

		internal void AgregarFila(List<Celda> cls)
		{
			int contador = 0;
			foreach (Columna col in this.columnas) {
				if (col.TipoDato == cls[contador].TipoDato)
				{
					col.AgregarValor(cls[contador]);

				}
				else {
					col.AgregarValor(new Celda("NULO", TipoDatoDB.NULO));
					Console.WriteLine("ERROR:NO SE PUEDE ASIGNAR EL VALOR TIPOS NO COINCIDEN");
				}
				contador++;
			}
			contadorFilas++;
		}

		internal void AgregarFila(List<Celda> cls, List<string> columnas)
		{
			int contador = 0;
			foreach (Columna col in this.columnas)
			{
				if (columnas.Contains(col.Titulo))
				{
					if (col.TipoDato == cls[contador].TipoDato)
					{
						col.AgregarValor(cls[contador]);
					}
					else
					{
						col.AgregarValor(new Celda("NULO", TipoDatoDB.NULO));
						Console.WriteLine("ERROR:NO SE PUEDE ASIGNAR EL VALOR TIPOS NO COINCIDEN");
					}
				}
				else {
					col.AgregarValor(new Celda("NULO", TipoDatoDB.NULO));
				}
				contador++;
			}
			contadorFilas++;
		}
		internal bool ExistenColumnas(List<string> cls)
		{
			foreach (string colum in cls) {
				if (!ExistenColumna(colum)) {
					return false;
				}

			}
			return true;
		}

		private bool ExistenColumna(string colum)
		{
			foreach (Columna cl in this.columnas) {
				if (cl.Titulo.Equals(colum)) {
					return true;
				}
			}
			return false;
		}

		internal string GetXml()
		{
			StringBuilder codigotabla = new StringBuilder();
			codigotabla.AppendLine("<Tabla>");
			codigotabla.AppendLine("<nombre>" + this.nombre + "</nombre>");
			codigotabla.AppendLine("<path>" + this.path + "</path>");
			codigotabla.AppendLine("<rows>");
			foreach (Columna cl in this.columnas)
			{
				codigotabla.AppendLine("<" + cl.TipoDato.ToString().ToLower() + ">" + cl.Titulo + "</" + cl.TipoDato.ToString().ToLower() + ">");
			}
			codigotabla.AppendLine("</rows>");
			codigotabla.AppendLine("</Tabla>");
			return codigotabla.ToString();
		}

		internal void GenerarArchivo()
		{
			StringBuilder archivoTB = new StringBuilder();
			int contador;
			for (contador=0;contador<contadorFilas;contador++) {
				archivoTB.AppendLine("<Row>");
				foreach (Columna cl in this.columnas)
				{
					archivoTB.AppendLine("<" + cl.Titulo + ">" + cl.Valores[contador].Valor + "</" + cl.Titulo + ">");
				}
				archivoTB.AppendLine("</Row>");
			}
			Console.WriteLine("*****************************************************");
			Console.WriteLine(archivoTB.ToString());

		}
	}
}
