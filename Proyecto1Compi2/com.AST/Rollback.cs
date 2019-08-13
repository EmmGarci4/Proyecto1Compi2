using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Rollback : Sentencia
	{
		public Rollback(int linea, int columna) : base(linea, columna)
		{
		}

		public override object Ejecutar()
		{
			Console.WriteLine("Rollback...");
			return null;
		}
	}
}
