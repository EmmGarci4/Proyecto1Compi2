using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Acceso
	{
		Queue<object> objetos;

		public Acceso(Queue<object> objetos)
		{
			this.objetos = objetos;
		}

		public Acceso()
		{
			this.objetos = new Queue<object>();
		}

		public Queue<object> Objetos { get => objetos; set => objetos = value; }
	}
}
