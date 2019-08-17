using com.Analisis.Util;
using Proyecto1Compi2.com.AST;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Proyecto1Compi2.com.db
{
	class Tabla
	{
		private String nombre;
		List<Columna> columnas;
		private int contadorFilas;
		private List<Fila> datos;

		public List<Columna> Columnas { get => columnas;}
		public int ContadorFilas { get => contadorFilas;}
		public string Nombre { get => nombre; set => nombre = value; }

		public Tabla(String nombre)
		{
			this.Nombre = nombre;
			this.columnas = new List<Columna> ();
			this.datos = new List<Fila>();
		}

		public Tabla(String nombre,List<Columna> tab)
		{
			this.Nombre = nombre;
			this.columnas = tab;
			this.datos = new List<Fila>();
		}

		//*****************************COLUMNAS**************************************************

		public void AgregarColumna(Columna columna) {
			columnas.Add(columna);
		}

		public bool ExisteColumna(string colum)
		{
			foreach (Columna cl in this.columnas)
			{
				if (cl.Nombre.Equals(colum))
				{
					return true;
				}
			}
			return false;
		}

		public void LimpiarColumnas()
		{
			this.columnas.Clear();
		}

		internal void EliminarColumna(string col)
		{
			this.columnas.Remove(BuscarColumna(col));
		}

		internal Columna BuscarColumna(string llave)
		{
			foreach (Columna cl in this.columnas)
			{
				if (cl.Nombre.Equals(llave))
				{
					return cl;
				}
			}
			return null;
		}

		//*****************************OPERACIONES***********************************************

		internal void Truncar()
		{
			contadorFilas = 0;
			this.datos.Clear();
		}

		public void MostrarCabecera()
		{
			Console.WriteLine("_____________________________________________________________");
			Console.WriteLine("|  " + Nombre + "                                               |");
			Console.WriteLine("_____________________________________________________________");
			Console.Write("|");
			foreach (Columna st in this.columnas)
			{
				Console.Write(st.Tipo.ToString().ToLower() + ":" + st.Nombre + "|");
			}
			Console.WriteLine();
			Console.WriteLine("_____________________________________________________________");
		}

		internal void AgregarValores(List<Expresion> valores,TablaSimbolos ts)
		{
			Fila f = new Fila(valores.Count);
			foreach (Expresion ex in valores) {
				f.Add(ex.GetValor(ts));
			}
			this.datos.Add(f);
			contadorFilas++;
		}

		internal void MostrarDatos()
		{
			int i = 0;
			while (i < ContadorFilas)
			{
				foreach (Fila cl in datos)
				{
					foreach (object val in cl) {
						Console.Write("|"+val.ToString()+"|");
					}
					Console.WriteLine();
				}
				i++;
			}
		}

		public override string ToString()
		{
			StringBuilder cadena = new StringBuilder();
			cadena.Append("\n<\n");
			cadena.Append("\"CQL-TYPE\" = \"TABLE\",\n");
			cadena.Append("\"NAME\" = \"" + Nombre + "\",\n");
			cadena.Append("\"COLUMNS\" =[");
			//columnas
			IEnumerator<Columna> enumerator = columnas.GetEnumerator();
			bool hasNext = enumerator.MoveNext();
			while (hasNext)
			{
				Columna i = enumerator.Current;
				cadena.Append(i.ToString());
				hasNext = enumerator.MoveNext();
				if (hasNext)
				{
					cadena.Append(",");
				}
			}
			enumerator.Dispose();
			//********
			cadena.Append("],\n");
			cadena.Append("\"DATA\" =[");
			int indice = 0;
			int cont;
			while (indice <ContadorFilas)
			{
				cadena.Append("\n<\n");
				cont = 0;
				foreach (Columna cl in this.columnas)
				{
					if (cl.Tipo.Tipo.Equals(TipoDatoDB.STRING))
					{
						//cadena.Append("\"" + cl.Nombre + "\"=\"" + cl.Datos.ElementAt(indice) + "\"");
					}
					else
					{
						//cadena.Append("\"" + cl.Nombre + "\"=" + cl.Datos.ElementAt(indice));
					}

					if (cont < this.columnas.Count - 1)
					{
						cadena.Append(",\n");
					}
					cont++;
				}
				cadena.Append("\n>");
				if (indice < ContadorFilas - 1)
				{
					cadena.Append(",");
				}
				indice++;
			}
		//*******
			cadena.Append("]\n>");
			return cadena.ToString();
		}

	}
}
