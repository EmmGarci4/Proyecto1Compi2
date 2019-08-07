using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class UsarBaseDatos:Sentencia
	{
		string nombre;

		public UsarBaseDatos(string nombre, int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
		}

		public string Nombre { get => nombre; set => nombre = value; }

		public override object Ejecutar()
		{
			Console.WriteLine("Usando base de datos..."+this.nombre);
			return null;
		}
	}
}
