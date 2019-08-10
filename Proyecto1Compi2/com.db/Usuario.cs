using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class Usuario
	{
		String nombre;
		String password;
		List<String> permisos;

		public Usuario(string nombre, string password, List<string> permisos)
		{
			this.Nombre = nombre;
			this.Password = password;
			this.Permisos = permisos;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public string Password { get => password; set => password = value; }
		public List<string> Permisos { get => permisos; set => permisos = value; }
	}
}
