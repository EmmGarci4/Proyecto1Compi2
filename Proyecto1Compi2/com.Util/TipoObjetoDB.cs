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

		public override bool Equals(object obj)
		{
			if (obj.GetType() == typeof(TipoObjetoDB)) {
				TipoObjetoDB t = (TipoObjetoDB)obj;
				return (t.Nombre.Equals(this.nombre) && t.tipo == this.tipo);
			} else if (obj.GetType()==typeof(string)) {
				return this.nombre.Equals((string)obj) && this.tipo.Equals(TipoDatoDB.OBJETO);
			}
			return false;
		}

		public override int GetHashCode()
		{
			var hashCode = -1788553097;
			hashCode = hashCode * -1521134295 + tipo.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(nombre);
			return hashCode;
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
		CURSOR,
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
