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
		protected Expresion(int linea, int columna) : base(linea, columna)
		{
		}

		public abstract Object GetValor(TablaSimbolos ts);
		public abstract TipoOperacion GetTipo(TablaSimbolos ts);
	}
}
