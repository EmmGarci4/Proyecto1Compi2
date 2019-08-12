using com.Analisis.Util;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

		public Tabla(String nombre, List<Columna> cls)
		{
			this.nombre = nombre;
			this.columnas = cls;
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
			foreach (Columna cl in this.columnas)
			{
				if (cl.Nombre.Equals(colum))
				{
					return true;
				}
			}
			return false;
		}

		internal void Insertar(Dictionary<string, object> fila)
		{
			//insercion especial
			//se inserta en las filas que corresponden y los demas valores son nulos
			foreach (Columna cl in Columnas) {
				try
				{
					if (!fila.ContainsKey(cl.Nombre))
					{
						if (cl.IsPrimary)
						{
							Console.WriteLine("ERROR LA LLAVE PRIMARIA NO PUEDE SER NULA");
							break;
						}
						else
						{
							this.datos.Add("null");
						}
					}
					else
					{
						if (IsTipoCompatible(cl.Tipo.Tipo, fila[cl.Nombre]))
						{
							this.datos.Add(fila[cl.Nombre]);
						}
						else
						{
							if (cl.IsPrimary)
							{
								Console.WriteLine("ERROR LA LLAVE PRIMARIA NO PUEDE SER NULA");
								break;
							}
							else
							{
								Console.WriteLine("ERROR LOS TIPOS NO SON COMPATIBLES");
							}
						}
					}
				}
				catch (KeyNotFoundException ex) {
					Console.WriteLine("ERROR COLUMNA EN DATOS NO EXISTE EN BASE DE DATOS");
				}
			}
		}

		private static bool IsTipoCompatible(TipoDatoDB tipo, object v)
		{
			switch (tipo)
			{
				case TipoDatoDB.BOOLEAN:
					return v.GetType() == typeof(bool);
				case TipoDatoDB.COUNTER:
					return v.GetType() == typeof(double) || v.GetType() == typeof(int);
				case TipoDatoDB.DOUBLE:
					return v.GetType() == typeof(double);
				case TipoDatoDB.INT:
					return v.GetType() == typeof(int);
				case TipoDatoDB.DATE:
					if (v.GetType() == typeof(string))
					{
						return Regex.IsMatch(v.ToString(), "'[0-9]{4}-[0-9]{2}-[0-9]{2}'");
					}
					return false;
				case TipoDatoDB.NULO:
					if (v.GetType() == typeof(string))
					{
						return v.ToString().ToLower().Equals("null");
					}
					return false;
				case TipoDatoDB.STRING:
					return true;
				case TipoDatoDB.TIME:
					if (v.GetType() == typeof(string))
					{
						return Regex.IsMatch(v.ToString(), "'[0-9]{2}:[0-9]{2}:[0-9]{2}'");
					}
					return false;
				case TipoDatoDB.LISTA_BOOLEAN:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							return list.IsAllBool();
						}
					}
					break;
				case TipoDatoDB.LISTA_DATE:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							return list.IsAllDate();
						}
					}
					break;
				case TipoDatoDB.LISTA_DOUBLE:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							return list.IsAllDouble();
						}
					}
					break;
				case TipoDatoDB.LISTA_INT:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							return list.IsAllInteger();
						}
					}
					break;
				case TipoDatoDB.LISTA_OBJETO:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							return list.IsAllObjeto();
						}
					}
					break;
				case TipoDatoDB.LISTA_STRING:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							return list.IsAllString();
						}
					}
					break;
				case TipoDatoDB.LISTA_TIME:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							return list.IsAllTime();
						}
					}
					break;
				case TipoDatoDB.SET_BOOLEAN:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							list.IsLista = false;
							list.Ordenar();
							return list.IsAllBool();
						}
					}
					break;
				case TipoDatoDB.SET_DATE:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							list.IsLista = false;
							return list.IsAllDate();
						}
					}
					break;
				case TipoDatoDB.SET_DOUBLE:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							list.IsLista = false;
							list.Ordenar();
							return list.IsAllDouble();
						}
					}
					break;
				case TipoDatoDB.SET_INT:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							list.IsLista = false;
							list.Ordenar();
							return list.IsAllInteger();
						}
					}
					break;
				case TipoDatoDB.SET_OBJETO:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							list.IsLista = false;
							return list.IsAllObjeto();
						}
					}
					break;
				case TipoDatoDB.SET_STRING:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							list.IsLista = false;
							list.Ordenar();
							return list.IsAllString();
						}
					}
					break;
				case TipoDatoDB.SET_TIME:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							return list.IsAllTime();
						}
					}
					break;
				case TipoDatoDB.MAP_BOOLEAN:
				case TipoDatoDB.MAP_DATE:
				case TipoDatoDB.MAP_DOUBLE:
				case TipoDatoDB.MAP_INT:
				case TipoDatoDB.MAP_OBJETO:
				case TipoDatoDB.MAP_STRING:
				case TipoDatoDB.MAP_TIME:
				case TipoDatoDB.OBJETO:
					return true;
			}

			return false;
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
			while(indice<this.datos.Count) {
				cadena.Append("\n<\n");
				cont = 0;
				foreach (Columna cl in this.columnas) {
					if (cl.Tipo.Tipo.Equals(TipoDatoDB.STRING))
					{
						cadena.Append("\"" + cl.Nombre + "\"=\"" + this.datos.ElementAt(indice) + "\"");
					}
					else {
						cadena.Append("\"" + cl.Nombre + "\"=" + this.datos.ElementAt(indice));
					}
					
					if (cont<this.columnas.Count-1) {
						cadena.Append(",\n");
					}
					cont++;
					indice++;
				}
				cadena.Append("\n>");
				if (indice<this.datos.Count-1) {
					cadena.Append(",");
				}
			}
			cadena.Append("]\n>");
			return cadena.ToString();
		}

	}
}
