using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class AbrirCursor:Sentencia
	{
		string nombre;

		public AbrirCursor(string nombre, int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
		}

		public string Nombre { get => nombre; set => nombre = value; }

		public override object Ejecutar(TablaSimbolos ts,Sesion sesion)
		{
			if (ts.ExisteSimbolo(nombre))
			{
				Simbolo s = ts.GetSimbolo(nombre);
				if (s.TipoDato.Tipo == TipoDatoDB.CURSOR)
				{
					Cursor cursor = (Cursor)s.Valor;
					object resultado = cursor.Select.Ejecutar(ts, sesion);
					if (resultado!=null) {
						if (resultado.GetType()==typeof(ThrowError)) {
							return resultado;
						}
						cursor.Resultado = (ResultadoConsulta)resultado;
					}
				}
				else
				{
					return new ThrowError(TipoThrow.Exception,
						"La variable '" + nombre + "' no es un cursor",
						Linea, Columna);
				}
			}
			else {
				return new ThrowError(TipoThrow.Exception,
						"La variable '" + nombre + "' no existe",
						Linea, Columna);
			}
			return null;
		}
	}
}
