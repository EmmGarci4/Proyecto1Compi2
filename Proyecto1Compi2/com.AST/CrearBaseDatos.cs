using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class CrearBaseDatos : Sentencia
	{
		string nombre;
		bool ifExist;

		public CrearBaseDatos(string nombre,bool ifexist, int linea, int columna) : base(linea, columna)
		{
			this.Nombre = nombre;
			this.IfExist = ifexist;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public bool IfExist { get => ifExist; set => ifExist = value; }

		public override object Ejecutar()
		{
			Console.WriteLine("Creando base de datos..."+this.nombre);
			return null;
		}
	}
}
