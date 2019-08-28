using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;

namespace Proyecto1Compi2.com.AST
{
	class Ternario:Expresion
	{
		Condicion condicion;
		Expresion exp1;
		Expresion exp2;

		public Ternario(Condicion condicion, Expresion exp1, Expresion exp2,int linea,int columna):base(linea,columna)
		{
			this.condicion = condicion;
			this.exp1 = exp1;
			this.exp2 = exp2;
		}

		public override TipoOperacion GetTipo(TablaSimbolos ts,Sesion sesion)
		{
			object cond = condicion.GetValor(ts,sesion);
			if (cond.GetType() == typeof(ThrowError))
			{
				return TipoOperacion.Nulo;
			}
			if ((bool)cond)
			{
				//retorna expresion 1
				return exp1.GetTipo(ts,sesion);
			}
			else
			{
				//retorna expresion 2
				return exp2.GetTipo(ts,sesion);
			}
		}

		public override object GetValor(TablaSimbolos ts,Sesion sesion)
		{
			object cond = condicion.GetValor(ts,sesion);
			if (cond.GetType()==typeof(ThrowError)) {
				return cond;
			}
			if ((bool)cond)
			{
				//retorna expresion 1
				return exp1.GetValor(ts,sesion);
			}
			else {
				//retorna expresion 2
				return exp2.GetValor(ts,sesion);
			}
		}
	}
}
