using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Util
{
	class Objeto
	{
		Dictionary<string, object> atributos;
		UserType plantilla;

		public Objeto(Dictionary<string, object> atributos,UserType plantilla)
		{
			this.atributos = atributos;
			this.plantilla = plantilla;
		}

		public Objeto( UserType plantilla)
		{
			this.atributos = new Dictionary<string, object>();
			this.plantilla = plantilla;
		}

		internal Dictionary<string, object> Atributos { get => atributos; set => atributos = value; }
		internal UserType Plantilla { get => plantilla; set => plantilla = value; }

		public override string ToString()
		{
			StringBuilder cadena = new StringBuilder();
			cadena.Append("<\n");
			int i = 0;
			foreach (KeyValuePair<string, object> val in atributos)
			{
				cadena.Append("\"" + val.Key + "\"=");
				if (val.Value.GetType() == typeof(string) & !Regex.IsMatch(val.Value.ToString(), "\b'[0-9]{4}-[0-9]{2}-[0-9]{2}'") &&
					!Regex.IsMatch(val.Value.ToString(), "\b'[0-9]{2}:[0-9]{2}:[0-9]{2}'"))
				{
					cadena.Append("\"" + val.Value + "\"");
				}
				else
				{
					cadena.Append(val.Value);
				}

				if (i < atributos.Count - 1)
				{
					cadena.Append(",\n");
				}
				i++;
			}

			cadena.Append("\n>");
			return cadena.ToString();
		}

		internal bool IsObjetoTipo(string nombre)
		{
			return plantilla.Nombre == nombre;
		}
	}
}
