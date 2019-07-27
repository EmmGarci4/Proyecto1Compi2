using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class CrearUsuario:Sentencia
	{
		string nombre;
		string passwd;

		public CrearUsuario(string nombre, string passwd)
		{
			this.nombre = nombre;
			this.passwd = passwd;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public string Passwd { get => passwd; set => passwd = value; }

		public override object Ejecutar()
		{
			Console.WriteLine("Creando usuario..."+this.nombre+":"+this.passwd);
			return null;
		}
	}
}
