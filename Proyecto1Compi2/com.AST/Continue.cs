using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;

namespace Proyecto1Compi2.com.AST
{
	class Continue : Sentencia
	{
		public Continue(int linea, int columna) : base(linea, columna)
		{
		}

		public override object Ejecutar(TablaSimbolos ts)
		{
			return this;
		}
	}
}
