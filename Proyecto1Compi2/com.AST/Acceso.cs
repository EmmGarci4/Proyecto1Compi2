using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;

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

		internal TipoAcceso getTipo()
		{
			if (this.objetos.Count == 1)
			{
				if (this.objetos.Peek().GetType() == typeof(string))
				{
					return TipoAcceso.Campo;
				}
				else {
					//puede se acceso a arreglo
					return TipoAcceso.Arreglo;
				}
			}
			else {
				if (this.objetos.Peek().GetType() == typeof(string))
				{
					return TipoAcceso.CampoDeCampo;
				}
				else
				{
					//puede se acceso a arreglo
					return TipoAcceso.ArregloDeCampo;
				}
			}
		}

		internal object getValor()
		{
			return this.objetos.Peek();
		}
	}
}
