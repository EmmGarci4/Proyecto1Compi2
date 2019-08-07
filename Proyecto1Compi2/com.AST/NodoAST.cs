using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	abstract class NodoAST
	{
		int linea;
		int columna;

		protected NodoAST(int linea, int columna)
		{
			this.Linea = linea;
			this.Columna = columna;
		}

		public int Linea { get => linea; set => linea = value; }
		public int Columna { get => columna; set => columna = value; }
	}
}
