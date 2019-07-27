using com.Analisis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class LlamadaFuncion : Sentencia
	{
		string nombre;
		List<Simbolo> parametros;

		public LlamadaFuncion(string nombre, List<Simbolo> parametros) 
		{
			this.nombre = nombre;
			this.parametros = parametros;
		}

		public override object Ejecutar()
		{
			Console.WriteLine("Llamando funcion..."+this.nombre);
			foreach (Simbolo s in this.parametros) {
				Console.WriteLine(s.Nombre);
			}
			return null;
		}
	}
}
