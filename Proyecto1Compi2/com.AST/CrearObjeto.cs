using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class CrearObjeto : Sentencia
	{
		string nombre;
		List<Simbolo> atributos;

		public CrearObjeto(string nombre, List<Simbolo> atributos, int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
			this.atributos = atributos;
		}

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			Console.WriteLine("Creando Objeto..." + this.nombre);
			foreach (Simbolo cl in this.atributos)
			{
				Console.WriteLine(cl.Nombre);
			}
			return null;
		}
	}
}
