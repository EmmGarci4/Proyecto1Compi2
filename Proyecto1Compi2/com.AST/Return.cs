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

		protected Return(Expresion valor) 
		{
			this.valor = valor;
		}

		public override object Ejecutar()
		{
			Console.WriteLine("Retornando...");
			return null;
		}
	}
}
