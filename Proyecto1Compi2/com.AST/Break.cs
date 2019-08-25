using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;

namespace Proyecto1Compi2.com.AST
{
	class Break : Sentencia
	{
		public Break(int linea, int columna) : base(linea, columna)
		{
		}

		public override object Ejecutar(TablaSimbolos ts)
		{
			return this;
		}
	}
}
