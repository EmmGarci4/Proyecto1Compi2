using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Commit : Sentencia
	{
		public Commit(int linea, int columna) : base(linea, columna)
		{
		}

		public override object Ejecutar()
		{
			Console.WriteLine("Commit");
			return null;
		}
	}
}
