using Proyecto1Compi2.com.AST;
using Proyecto1Compi2.com.Util;
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
		string fecha;
		string hora;

		public Error(TipoError tipo, String mensaje,int linea, int columna) {
			this.mensaje = mensaje;
			this.linea = linea+1;
			this.columna = columna+1;
			this.tipo = tipo;
			this.fecha = "";
			this.hora = "";
		}

		public Error(ThrowError error)
		{
			this.mensaje = error.Mensaje;
			this.linea = error.Linea + 1;
			this.columna = error.Columna + 1;
			this.fecha = "";
			this.hora = "";
			tipo = TipoError.Semantico;
		}

		public Error(TipoError tipo, string mensaje, int linea, int columna,  string fecha, string hora)
		{
			this.mensaje = mensaje;
			this.linea = linea;
			this.columna = columna;
			this.tipo = tipo;
			this.fecha = fecha;
			this.hora = hora;
		}

		public string Mensaje { get => mensaje; set => mensaje = value; }
		public int Linea { get => linea; set => linea = value; }
		public int Columna { get => columna; set => columna = value; }
		public TipoError Tipo { get => tipo; set => tipo = value; }
		public string Fecha { get => fecha; set => fecha = value; }
		public string Hora { get => hora; set => hora = value; }

		public override string ToString()
		{
			return "Error "+tipo+":"+mensaje+". En línea:"+linea+" y columna:"+Columna+"\r\n";
		}
	}
}
