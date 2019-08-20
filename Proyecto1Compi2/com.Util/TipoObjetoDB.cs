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
				case TipoDatoDB.LISTA_PRIMITIVO:
					return "List<"+this.nombre+">";
				case TipoDatoDB.LISTA_OBJETO:
					return "List<"+this.nombre+">";
				case TipoDatoDB.SET_PRIMITIVO:
					return "Set<"+this.nombre+">";
				case TipoDatoDB.SET_OBJETO:
					return "Set<" + this.nombre + ">";
				case TipoDatoDB.MAP_PRIMITIVO:
					return "Map<"+this.nombre+">";
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
		LISTA_PRIMITIVO,
		LISTA_OBJETO,
		//SETS
		SET_PRIMITIVO,
		SET_OBJETO,
		//MAPS
		MAP_PRIMITIVO,
		MAP_OBJETO,
	}
}
