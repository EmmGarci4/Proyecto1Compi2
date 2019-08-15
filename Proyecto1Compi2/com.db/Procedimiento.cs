using com.Analisis.Util;
using Proyecto1Compi2.com.AST;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class Procedimiento
	{
		string nombre;
		Dictionary<string, TipoObjetoDB> parametros;
		Dictionary<string, TipoObjetoDB> retornos;
		string instrucciones;

		public Procedimiento(string nombre, Dictionary<string, TipoObjetoDB> parametros, Dictionary<string,
		TipoObjetoDB> retornos, string inst)
		{
			this.nombre = nombre;
			this.parametros = parametros;
			this.retornos = retornos;
			this.instrucciones = inst;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public Dictionary<string, TipoObjetoDB> Parametros { get => parametros; set => parametros = value; }
		public Dictionary<string, TipoObjetoDB> Retornos { get => retornos; set => retornos = value; }
		internal string Instrucciones { get => instrucciones; set => instrucciones = value; }

		public string GetCodigoFuente()
		{
			return instrucciones;
		}

		public override string ToString()
		{
			StringBuilder cadena = new StringBuilder();
			cadena.Append("\n<\n");
			cadena.Append("\"CQL-TYPE\" = \"PROCEDURE\",\n");
			cadena.Append("\"NAME\" = \"" + Nombre + "\",\n");
			cadena.Append("\"PARAMETERS\" = [");
			int i = 0;
			//PARAMETROS
			foreach (KeyValuePair<string, TipoObjetoDB> kvp in Parametros)
			{
				cadena.Append("\n<");
				cadena.Append("\"NAME\"=\"" + kvp.Key + "\",");
				cadena.Append("\"TYPE\"=\"" + kvp.Value.ToString() + "\",");
				cadena.Append("\"AS\" = IN>");

				if (i < Parametros.Count - 1)
				{
					cadena.Append(",");
				}
				i++;
			}
			//RETORNOS
			if (retornos.Count > 0)
			{
				cadena.Append(",");
			}
			i = 0;
			foreach (KeyValuePair<string, TipoObjetoDB> kvp in Retornos)
			{
				cadena.Append("\n<");
				cadena.Append("\"NAME\"=\"" + kvp.Key + "\",");
				cadena.Append("\"TYPE\"=\"" + kvp.Value.ToString() + "\",");

				cadena.Append("\"AS\" = OUT>");

				if (i < Retornos.Count - 1)
				{
					cadena.Append(",");
				}
				i++;
			}
			cadena.Append("],\n");
			cadena.Append("\"INSTR\" = $\n" + instrucciones + "$\n");
			cadena.Append("\n>");

			return cadena.ToString();
		}

	}
}
