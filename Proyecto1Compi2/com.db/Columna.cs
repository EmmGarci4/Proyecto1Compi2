using com.Analisis.Util;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class Columna : IEquatable<Columna>
	{
		String nombre;
		TipoObjetoDB tipo;
		bool isPrimary;
		List<object> datos;

		public Columna()
		{
			this.tipo = null;
			this.nombre = null;
			this.isPrimary = false;
			this.datos = new List<object>();
		}

		public Columna(String titulo, TipoObjetoDB tipo, bool isp)
		{
			this.tipo = tipo;
			this.nombre = titulo;
			this.isPrimary = isp;
			this.datos = new List<object>();
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public TipoObjetoDB Tipo { get => tipo; set => tipo = value; }
		public bool IsPrimary { get => isPrimary; set => isPrimary = value; }
		public List<object> Datos { get => datos; set => datos = value; }

		public override string ToString()
		{
			StringBuilder cadena = new StringBuilder();
			cadena.Append("\n<\n");
			cadena.Append("\"NAME\" = \""+Nombre+"\",\n");
			cadena.Append("\"TYPE\" = \""+tipo.ToString()+"\",\n");
			
			
			cadena.Append("\"PK\" = "+isPrimary);
			cadena.Append("\n>");
			return cadena.ToString();
		}

		public bool Equals(Columna other)
		{
			if (Nombre==other.nombre) {
				return true;
			}
			return false;
		}

		internal int GetUltimoValorCounter()
		{
			if (datos.Count == 0)
			{
				return 0;
			}
			else {
				return (int)datos.ElementAt(datos.Count-1);
			}
		}

		internal bool ExisteDato(object respuesta)
		{
			return this.datos.Contains(respuesta);
		}

		internal bool isValido()
		{
			return tipo != null && nombre != null;
		}
	}
}
