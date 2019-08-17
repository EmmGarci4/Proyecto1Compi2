using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	abstract class Sentencia : NodoAST
	{
		protected Sentencia(int linea, int columna) : base(linea, columna)
		{
		}

		public abstract Object Ejecutar(Sesion sesion,TablaSimbolos tb);
	}
}
