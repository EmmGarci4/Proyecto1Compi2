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
			this.Mensaje1 = mensaje;
			this.Linea1 = linea+1;
			this.Columna1 = columna+1;
			this.Tipo1 = tipo;
			this.fecha = "";
			this.hora = "";
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

		public string Mensaje { get => Mensaje1; set => Mensaje1 = value; }
		public int Linea { get => Linea1; set => Linea1 = value; }
		public int Columna { get => Columna1; set => Columna1 = value; }
		public TipoError Tipo { get => Tipo1; set => Tipo1 = value; }
		public string Mensaje1 { get => mensaje; set => mensaje = value; }
		public int Linea1 { get => linea; set => linea = value; }
		public int Columna1 { get => columna; set => columna = value; }
		public TipoError Tipo1 { get => tipo; set => tipo = value; }
		public string Fecha { get => fecha; set => fecha = value; }
		public string Hora { get => hora; set => hora = value; }

		public override string ToString()
		{
			return "Error "+Tipo1+":"+Mensaje1+". En línea:"+Linea1+" y columna:"+Columna+"\n";
		}
	}
}
