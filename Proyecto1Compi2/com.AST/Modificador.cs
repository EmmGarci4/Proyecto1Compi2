using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;

namespace Proyecto1Compi2.com.AST
{
	class Modificador:Sentencia 
	{
		Expresion modificador;

		public Modificador(Expresion modificador):base(modificador.Linea,modificador.Columna)
		{
			this.modificador = modificador;
		}

		internal Expresion Mod { get => modificador; set => modificador = value; }

		public override object Ejecutar(Sesion sesion, TablaSimbolos tb)
		{
			object respuesta = modificador.GetValor(tb);
			if (respuesta!=null) {
				if (respuesta.GetType() == typeof(ThrowError))
				{
					return respuesta;
				}
			}
			return null;
		}
	}
}
