using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class EliminarUserType : Sentencia
	{
		string nombre;

		public EliminarUserType(string nombre, int linea, int columna) : base(linea, columna)
		{
			this.Nombre = nombre;
		}

		public string Nombre { get => nombre; set => nombre = value; }

		public override object Ejecutar(Sesion sesion)
		{
			Console.WriteLine("Eliminar Ut..." + this.Nombre.ToString());
			return null;
		}
	}
}
