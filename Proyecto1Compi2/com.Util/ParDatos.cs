using Proyecto1Compi2.com.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Util
{
	class ParDatos
	{
		string nombre;
		object valor;

		public ParDatos(string nombre, object valor)
		{
			this.nombre = nombre;
			this.valor = valor;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public object Valor { get => valor; set => valor = value; }
	}
}
