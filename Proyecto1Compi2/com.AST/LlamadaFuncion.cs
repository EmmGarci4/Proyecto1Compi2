using com.Analisis.Util;
using Proyecto1Compi2.com.db;
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
		List<Expresion> parametros;

		public LlamadaFuncion(string nombre, List<Expresion> parametros, int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
			this.parametros = parametros;
		}

		public override object Ejecutar(Sesion sesion)
		{
			Console.WriteLine("Llamando funcion..."+this.nombre);
			return null;
		}
	}
}
