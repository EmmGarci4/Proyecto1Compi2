using com.Analisis.Util;
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
		List<Simbolo> parametros;


		public string Nombre { get => nombre; set => nombre = value; }
		internal List<Simbolo> Parametros { get => parametros; set => parametros = value; }

		public Procedimiento(string nombre,List<Simbolo> parametros) {
			this.nombre = nombre;
			this.parametros = parametros;
		}

		public string GetCodigoFuente() {
			return "Codigo";
		}

	}
}
