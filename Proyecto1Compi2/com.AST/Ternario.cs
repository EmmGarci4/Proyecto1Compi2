using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;

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

		public override TipoOperacion GetTipo(TablaSimbolos ts)
		{
			object cond = condicion.GetValor(ts);
			if (cond.GetType() == typeof(ThrowError))
			{
				return TipoOperacion.Nulo;
			}
			if ((bool)cond)
			{
				//retorna expresion 1
				return exp1.GetTipo(ts);
			}
			else
			{
				//retorna expresion 2
				return exp2.GetTipo(ts);
			}
		}

		public override object GetValor(TablaSimbolos ts)
		{
			object cond = condicion.GetValor(ts);
			if (cond.GetType()==typeof(ThrowError)) {
				return cond;
			}
			if ((bool)cond)
			{
				//retorna expresion 1
				return exp1.GetValor(ts);
			}
			else {
				//retorna expresion 2
				return exp2.GetValor(ts);
			}
		}
	}
}
