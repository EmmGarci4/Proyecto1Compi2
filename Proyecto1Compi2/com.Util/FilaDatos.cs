using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Util
{
	class FilaDatos
	{
		List<ParDatos> datos;
		int linea;
		int columna;

		public FilaDatos()
		{
			this.datos = new List<ParDatos>();
		}

		public int Linea { get => linea; set => linea = value; }
		public int Columna { get => columna; set => columna = value; }
		internal List<ParDatos> Datos { get => datos; set => datos = value; }
	}
}
