using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Return : Sentencia
	{
		Expresion valor;

		protected Return(Expresion valor, int linea, int columna) : base(linea, columna)
		{
			this.valor = valor;
		}

		public override object Ejecutar(Usuario usuario)
		{
			Console.WriteLine("Retornando...");
			return null;
		}
	}
}
