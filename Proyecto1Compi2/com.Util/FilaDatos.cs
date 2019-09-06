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

		public FilaDatos()
		{
			this.datos = new List<ParDatos>();
		}

		internal List<ParDatos> Datos { get => datos; set => datos = value; }
	}
}
