using com.Analisis.Util;
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

		public List<Columna> Columnas { get => columnas;}
		public int ContadorFilas { get => contadorFilas;}
		public string Nombre { get => nombre; set => nombre = value; }

		public Tabla(String nombre)
		{
			this.Nombre = nombre;
			this.columnas = new List<Columna> ();
		}

		public Tabla(String nombre,List<Columna> tab)
		{
			this.Nombre = nombre;
			this.columnas = tab;
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

		public List<Error> Insertar(Dictionary<string, object> fila,int linea,int columna)
		{
			List<Error> er = new List<Error>();
			////insercion especial
			////se inserta en las filas que corresponden y los demas valores son nulos
			List<object> filaFantasma = new List<object>();
			bool bandera = true;
			foreach (Columna cl in Columnas)
			{
				try
				{
					if (!fila.ContainsKey(cl.Nombre))
					{
						if (cl.IsPrimary)
						{
							if (cl.Tipo.Tipo == TipoDatoDB.COUNTER)
							{
								filaFantasma.Add(ContadorFilas + 1);
							}
							else
							{
								er.Add(new Error(TipoError.Sintactico, "La llave primaria no puede ser nula" + Nombre, linea, columna,
						Datos.GetDate(), Datos.GetTime()));
								bandera = false;
								break;
							}
						}
						else
						{
							if (cl.Tipo.Tipo == TipoDatoDB.COUNTER)
							{
								filaFantasma.Add(ContadorFilas + 1);
							}
							else
							{
								filaFantasma.Add("null");
							}
						}
					}
					else
					{
						if (Datos.IsTipoCompatible(cl.Tipo, fila[cl.Nombre]))
						{
							filaFantasma.Add(fila[cl.Nombre]);
						}
						else
						{
							if (cl.IsPrimary)
							{
								er.Add(new Error(TipoError.Sintactico, "La llave primaria no puede ser nula" + Nombre, linea, columna,
						Datos.GetDate(), Datos.GetTime()));
								bandera = false;
								break;
							}
							else
							{
								filaFantasma.Add("null");
								er.Add(new Error(TipoError.Sintactico, "El tipo de dato de la columna no es compatible con el dato ingresado" + Nombre, linea, columna,
						Datos.GetDate(), Datos.GetTime()));
							}
						}
					}
				}
				catch (KeyNotFoundException ex)
				{
					er.Add(new Error(TipoError.Sintactico, "Error grave al insertar datos en la tabla " + Nombre,linea,columna,
						Datos.GetDate(),Datos.GetTime()));
					break;
				}
			}
			if (Columnas.Count == filaFantasma.Count && bandera)
			{
				int indice = 0;
				foreach (Columna cl in Columnas)
				{
					cl.Datos.Add(filaFantasma.ElementAt(indice));
					indice++;
				}
				contadorFilas++;
			}
			return er;
		}

		internal List<Error> Insertar(List<object> list,int linea,int columna)
		{
			List<Error> err = new List<Error>();
			int i = 0;
			List<object> filaFantasma = new List<object>();
			foreach (Columna cl in Columnas) {
				if (cl.Tipo.Tipo == TipoDatoDB.COUNTER)
				{
					filaFantasma.Add(ContadorFilas+1);
				}
				else {
					if (i < list.Count)
					{
						if (Datos.IsTipoCompatible(cl.Tipo, list.ElementAt(i)))
						{
							filaFantasma.Add(list.ElementAt(i));
						}
						else {
							if (cl.IsPrimary)
							{
								err.Add(new Error(TipoError.Sintactico, "El valor ingresado a la llave primaria no concuerda con el tipo de dato de la columna", linea, columna,
						Datos.GetDate(), Datos.GetTime()));
								break;
							}
							else {
								filaFantasma.Add("null");
							}
						}
						i++;
					}
					else {
						filaFantasma.Add("null");
					}
				}
			}
			if (Columnas.Count==filaFantasma.Count) {
				int indice = 0;
				foreach (Columna cl in Columnas) {
					cl.Datos.Add(filaFantasma.ElementAt(indice));
					indice++;
				}
				contadorFilas++;
			}
			return err;
		}

		internal void Truncar()
		{
			contadorFilas = 0;
			foreach (Columna cl in Columnas) {
				cl.Datos.Clear();
			}
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

		internal void MostrarDatos()
		{
			int i = 0;
			while (i < ContadorFilas)
			{
				foreach (Columna cl in columnas)
				{
					Console.Write("|" + cl.Datos.ElementAt(i) + "|");
				}
				i++;
				Console.WriteLine();
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
						cadena.Append("\"" + cl.Nombre + "\"=\"" + cl.Datos.ElementAt(indice) + "\"");
					}
					else
					{
						cadena.Append("\"" + cl.Nombre + "\"=" + cl.Datos.ElementAt(indice));
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
			cadena.Append("]\n>");
			return cadena.ToString();
		}

	}
}
