using com.Analisis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class CrearFuncion : Sentencia
	{
		string nombre;
		List<Simbolo> parametros;
		TipoDatoDB tipo;
		List<Sentencia> sentencias;

		public CrearFuncion(string nombre, List<Simbolo> parametros, TipoDatoDB tipo, List<Sentencia> sentencias, int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
			this.parametros = parametros;
			this.tipo = tipo;
			this.sentencias = sentencias;
		}

		public override object Ejecutar()
		{
			Console.WriteLine("Creando Funcion..." + this.nombre+":"+this.tipo.ToString().ToLower());
			foreach (Simbolo cl in this.parametros)
			{
				Console.WriteLine(cl.Nombre);
			}
			return null;
		}
	}
}
