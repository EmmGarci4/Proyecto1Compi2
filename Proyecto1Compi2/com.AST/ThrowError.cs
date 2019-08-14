using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	public class ThrowError
	{
		TipoThrow tipo;
		String mensaje;
		int linea;
		int columna;

		public ThrowError(TipoThrow tipo, string mensaje, int linea, int columna)
		{
			this.tipo = tipo;
			this.mensaje = mensaje;
			this.linea = linea;
			this.columna = columna;
		}

		public string Mensaje { get => mensaje; set => mensaje = value; }
		public int Linea { get => linea; set => linea = value; }
		public int Columna { get => columna; set => columna = value; }
		internal TipoThrow Tipo { get => tipo; set => tipo = value; }
	}
}
