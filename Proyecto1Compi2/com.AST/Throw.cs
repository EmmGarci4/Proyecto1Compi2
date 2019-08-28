using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class Throw:Sentencia
	{
		String nombreExeption;

		public Throw(string nombreExeption,int linea,int columna):base(linea,columna)
		{
			this.nombreExeption = nombreExeption;
		}

		public string NombreExeption { get => nombreExeption; set => nombreExeption = value; }

		public override object Ejecutar(TablaSimbolos ts,Sesion sesion)
		{
			if (Datos.GetExceptcion(nombreExeption)==TipoThrow.Exception) {
				return new ThrowError(TipoThrow.Exception,
							"Excepción '"+nombreExeption+"' no reconocida",
							Linea, Columna);
			}
			return this;
		}
	}
}
