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
		List<Columna> columnas;
		private List<object> datos;
		private int contadorFilas;

		public string Nombre { get => nombre; set => nombre = value; }
		public List<Columna> Columnas { get => columnas; set => columnas = value; }

		public Tabla(String nombre)
		{
			this.nombre = nombre;
			this.columnas = new List<Columna>();
			this.datos = new List<object>();
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

		public void MostrarCabecera()
		{
			Console.WriteLine("_____________________________________________________________");
			Console.WriteLine("|  "+Nombre+"                                               |");
			Console.WriteLine("_____________________________________________________________");
			Console.Write("|");
			foreach (Columna st in this.columnas)
			{
				Console.Write(st.Tipo.ToString().ToLower() + ":" + st.Nombre + "|");
			}
			Console.WriteLine();
		}

		internal void AgregarFila(List<object> cls)
		{
			//agreagando fila completa
			this.datos.AddRange(cls);
			contadorFilas++;
		}

		internal void MostrarDatos()
		{
			int i=0;
			while (i<datos.Count) {
				for (int contador=0;contador<columnas.Count;contador++) {
					Console.Write("---");
					Console.Write(datos.ElementAt(i));
					Console.Write("---");
					i++;
				}
				Console.WriteLine();
			}
		}

		//internal void AgregarFila(List<Celda> cls, List<string> columnas)
		//{
		////	int contador = 0;
		////	foreach (Columna col in this.columnas)
		////	{
		////		if (columnas.Contains(col.Titulo))
		////		{
		////			if (col.TipoDato == cls[contador].TipoDato)
		////			{
		////				col.AgregarValor(cls[contador]);
		////			}
		////			else
		////			{
		////				col.AgregarValor(new Celda("NULO", TipoDatoDB.NULO));
		////				Console.WriteLine("ERROR:NO SE PUEDE ASIGNAR EL VALOR TIPOS NO COINCIDEN");
		////			}
		////		}
		////		else {
		////			col.AgregarValor(new Celda("NULO", TipoDatoDB.NULO));
		////		}
		////		contador++;
		////	}
		////	contadorFilas++;
		//}

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
			//foreach (Columna cl in this.columnas) {
			//	if (cl.Titulo.Equals(colum)) {
			//		return true;
			//	}
			//}
			return false;
		}
	}
}
