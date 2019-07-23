using com.Analisis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Objeto
	{
		string nombre;
		List<Simbolo> atributos;

		public string Nombre { get => nombre; set => nombre = value; }
		internal List<Simbolo> Atributos { get => atributos; set => atributos = value; }

		public Objeto(String nombre,List<Simbolo> atributos) {
			this.nombre = nombre;
			this.atributos = atributos;
		}

	}
}
