using com.Analisis.Util;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class UserType
	{
		string nombre;
		Dictionary<string, TipoObjetoDB> atributos;

		public UserType(string nombre, Dictionary<string, TipoObjetoDB> atributos)
		{
			this.Nombre = nombre;
			this.Atributos = atributos;
		}

		public UserType(string nombre)
		{
			this.Nombre = nombre;
			this.Atributos = new Dictionary<string, TipoObjetoDB>();
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public Dictionary<string, TipoObjetoDB> Atributos { get => atributos; set => atributos = value; }

		public override string ToString()
		{
			StringBuilder cadena = new StringBuilder();
			cadena.Append("\n<\n");
			cadena.Append("\"CQL-TYPE\" = \"OBJECT\",\n");
			cadena.Append("\"NAME\" = \"" + Nombre + "\",\n");
			cadena.Append("\"ATTRS\" = [");
			int i = 0;
			foreach (KeyValuePair<string, TipoObjetoDB> kvp in Atributos)
			{
				cadena.Append("\n<");
				cadena.Append("\"NAME\"=\"" + kvp.Key + "\",");
				cadena.Append("\"TYPE\"=\"" + kvp.Value.ToString() + "\">");


				if (i < Atributos.Count - 1)
				{
					cadena.Append(",");
				}
				i++;
			}
			cadena.Append("]");
			cadena.Append("\n>");

			return cadena.ToString();
		}

		internal bool IsValido()
		{
			return this.atributos != null && this.nombre != null;
		}

		internal bool Contiene(List<string> attrs)
		{
			int contador = 0;
			if (this.atributos.Count==attrs.Count) {
				foreach (KeyValuePair<string,TipoObjetoDB> keys in this.atributos) {
					if (!keys.Key.Equals(attrs.ElementAt(contador))) {
						return false;
					}
					
					contador++;
				}
			}
			return true;
		}
	}
}
