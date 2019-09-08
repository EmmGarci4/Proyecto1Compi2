using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Cursor
	{
		string nombre;
		Seleccionar select;
		ResultadoConsulta resultado;

		public Cursor(string nombre, Seleccionar select)
		{
			this.nombre = nombre;
			this.select = select;
			this.resultado = null;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		internal Seleccionar Select { get => select; set => select = value; }
		internal ResultadoConsulta Resultado { get => resultado; set => resultado = value; }
	}
}
