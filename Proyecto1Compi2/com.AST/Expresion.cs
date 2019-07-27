using com.Analisis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	abstract class Expresion : NodoAST
	{
		string valor;
		TipoDatoDB tipo;

		protected Expresion(string valor, TipoDatoDB tipo)
		{
			this.valor = valor;
			this.tipo = tipo;
		}

		public abstract Object getValor();
		public abstract TipoDatoDB getTipo();
	}
}
