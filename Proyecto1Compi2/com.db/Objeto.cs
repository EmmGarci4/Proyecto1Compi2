using com.Analisis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class Objeto
	{
		Dictionary<string, object> atributos;
		string tipo;

		public override string ToString()
		{
			StringBuilder cadena = new StringBuilder();
			cadena.Append("<\n");
			int i = 0;
			foreach (KeyValuePair<string,object> val in atributos) {
				cadena.Append("\""+val.Key+"\"=");
				if (val.Value.GetType() == typeof(string) & !Regex.IsMatch(val.Value.ToString(), "'[0-9]{4}-[0-9]{2}-[0-9]{2}'") &&
					!Regex.IsMatch(val.Value.ToString(), "'[0-9]{2}:[0-9]{2}:[0-9]{2}'"))
				{
					cadena.Append("\"" + val.Value + "\"");
				}
				else {
					cadena.Append(val.Value);
				}

				if (i<atributos.Count-1) {
					cadena.Append(",\n");
				}
				i++;
			}

			cadena.Append("\n>");
			return cadena.ToString();
		}

		internal Dictionary<string, object> Atributos { get => atributos; set => atributos = value; }
		public string Tipo { get => tipo; set => tipo = value; }

		public Objeto(Dictionary<string, object> atributos) {
			this.atributos = atributos;
			Tipo = "";
		}


	}
}
