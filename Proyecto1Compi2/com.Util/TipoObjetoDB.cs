using com.Analisis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Util
{
	public class TipoObjetoDB
	{
		TipoDatoDB tipo;
		string nombre;

		public override string ToString()
		{
			switch (tipo) {
				case TipoDatoDB.LISTA_STRING:
					return "List<String>";
				case TipoDatoDB.LISTA_INT:
					return "List<Int>";
				case TipoDatoDB.LISTA_DOUBLE:
					return "List<Double>";
				case TipoDatoDB.LISTA_BOOLEAN:
					return "List<Boolean>";
				case TipoDatoDB.LISTA_DATE:
					return "List<Date>";
				case TipoDatoDB.LISTA_TIME:
					return "List<Time>";
				case TipoDatoDB.LISTA_OBJETO:
					return "List<"+this.nombre+">";
				case TipoDatoDB.SET_STRING:
					return "Set<String>";
				case TipoDatoDB.SET_INT:
					return "Set<Int>";
				case TipoDatoDB.SET_DOUBLE:
					return "Set<Double>";
				case TipoDatoDB.SET_BOOLEAN:
					return "Set<Boolean>";
				case TipoDatoDB.SET_DATE:
					return "Set<Date>";
				case TipoDatoDB.SET_TIME:
					return "Set<Time>";
				case TipoDatoDB.SET_OBJETO:
					return "Set<" + this.nombre + ">";
				case TipoDatoDB.MAP_STRING:
					return "Map<String>";
				case TipoDatoDB.MAP_INT:
					return "Map<Int>";
				case TipoDatoDB.MAP_DOUBLE:
					return "Map<Double>";
				case TipoDatoDB.MAP_BOOLEAN:
					return "Map<Boolean>";
				case TipoDatoDB.MAP_DATE:
					return "Map<Date>";
				case TipoDatoDB.MAP_TIME:
					return "Map<Time>";
				case TipoDatoDB.MAP_OBJETO:
					return "Map<" + this.nombre + ">";
				case TipoDatoDB.OBJETO:
					return this.nombre;
				default:
					return tipo.ToString().ToLower();
			}
		}

		public TipoObjetoDB(TipoDatoDB tipo, string nombre)
		{
			this.tipo = tipo;
			this.nombre = nombre;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		internal TipoDatoDB Tipo { get => tipo; set => tipo = value; }

		

	}

	public enum TipoDatoDB
	{
		STRING,
		INT,
		DOUBLE,
		BOOLEAN,
		DATE,
		TIME,
		OBJETO,
		//ESPECIALES
		COUNTER,
		NULO,
		//LISTAS
		LISTA_STRING,
		LISTA_INT,
		LISTA_DOUBLE,
		LISTA_BOOLEAN,
		LISTA_DATE,
		LISTA_TIME,
		LISTA_OBJETO,
		//SETS
		SET_STRING,
		SET_INT,
		SET_DOUBLE,
		SET_BOOLEAN,
		SET_DATE,
		SET_TIME,
		SET_OBJETO,
		//MAPS
		MAP_STRING,
		MAP_INT,
		MAP_DOUBLE,
		MAP_BOOLEAN,
		MAP_DATE,
		MAP_TIME,
		MAP_OBJETO,
	}
}
