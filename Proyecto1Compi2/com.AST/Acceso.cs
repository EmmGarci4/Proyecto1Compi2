using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;

namespace Proyecto1Compi2.com.AST
{
	class Acceso:Expresion
	{
		Queue<object> objetos;

		public Acceso(Queue<object> objetos,int linea,int columna):base(linea,columna)
		{
			this.objetos = objetos;
		}

		public Acceso(int linea,int columna):base(linea,columna)
		{
			this.objetos = new Queue<object>();
		}

		public Queue<object> Objetos { get => objetos; set => objetos = value; }


		public override object GetValor(TablaSimbolos ts)
		{
			return null;
		}

		public override TipoOperacion GetTipo(TablaSimbolos ts)
		{
			if (this.objetos.Count == 1)
			{
				if (this.objetos.Peek().GetType() == typeof(string))
				{
					//return TipoAcceso.Campo;
					return TipoOperacion.Identificador;
				}
				else
				{
					//puede se acceso a arreglo
					//return TipoAcceso.Arreglo;
					return TipoOperacion.Identificador;
				}
			}
			else
			{
				if (this.objetos.Peek().GetType() == typeof(string))
				{
					//return TipoAcceso.CampoDeCampo;
					return TipoOperacion.Identificador;
				}
				else
				{
					//puede se acceso a arreglo
					//return TipoAcceso.ArregloDeCampo;
					return TipoOperacion.Identificador;
				}
			}
		}
	}
}
