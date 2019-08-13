using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class EliminarBaseDatos:Sentencia
	{
			string nombre;

			public EliminarBaseDatos( string nombre, int linea, int columna) : base(linea, columna)
			{
				this.Nombre = nombre;
			}

			public string Nombre { get => nombre; set => nombre = value; }

			public override object Ejecutar()
			{
				Console.WriteLine("Eliminar DB..."+ this.Nombre.ToString());
				return null;
			}
		}
}
