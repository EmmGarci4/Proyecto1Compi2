using com.Analisis.Util;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class Columna
	{
		String nombre;
		TipoObjetoDB tipo;
		bool isPrimary;
		private List<object> datos;

		public string Nombre { get => nombre; set => nombre = value; }
		public TipoObjetoDB Tipo { get => tipo; set => tipo = value; }
		public bool IsPrimary { get => isPrimary; set => isPrimary = value; }
		public List<object> Datos { get => datos; set => datos = value; }

		public Columna(String titulo, TipoObjetoDB tipo,bool isp) {
			this.tipo = tipo;
			this.nombre = titulo;
			this.isPrimary = isp;
			this.datos = new List<object>();
		}

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
	}
}
