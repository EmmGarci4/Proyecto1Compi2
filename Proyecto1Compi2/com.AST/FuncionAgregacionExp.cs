using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;

namespace Proyecto1Compi2.com.AST
{
	class FuncionAgregacionExp:Expresion
	{
		FuncionAgregacion funcion;
		TipoOperacion tipo;
		public FuncionAgregacionExp(FuncionAgregacion funcion,int linea,int columna):base(linea,columna)
		{
			this.funcion = funcion;
			this.tipo = TipoOperacion.Nulo;
		}

		public override TipoOperacion GetTipo(TablaSimbolos ts, Sesion sesion)
		{
			return tipo;
		}

		public override object GetValor(TablaSimbolos ts, Sesion sesion)
		{
			object posibleError = funcion.Ejecutar(ts, sesion);
			if (posibleError!=null) {
				if (posibleError.GetType()==typeof(ThrowError)) {
					return posibleError;
				}
				this.tipo = TipoOperacion.Numero;
			}
			return posibleError;
		}
	}
}
