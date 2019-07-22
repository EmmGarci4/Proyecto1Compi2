using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.Analisis.Util
{
	public class Error
	{
		String mensaje;
		int linea;
		int columna;
		TipoError tipo;

		public Error(TipoError tipo, String mensaje,int linea, int columna) {
			this.mensaje = mensaje;
			this.linea = linea+1;
			this.columna = columna+1;
			this.tipo = tipo;
		}

		public string Mensaje { get => mensaje; set => mensaje = value; }
		public int Linea { get => linea; set => linea = value; }
		public int Columna { get => columna; set => columna = value; }
		public TipoError Tipo { get => tipo; set => tipo = value; }
	}
}
