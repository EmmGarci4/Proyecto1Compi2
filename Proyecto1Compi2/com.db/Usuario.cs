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

		public override string ToString()
		{
			StringBuilder cadena = new StringBuilder();
			cadena.Append("\n<\n");
			cadena.Append("\"NAME\"="+"\""+Nombre+"\",\n");
			cadena.Append("\"PASSWORD\"=\""+Password+"\",\n");
			cadena.Append("\"PERMISSIONS\"=[");
			IEnumerator<string> enumerator = Permisos.GetEnumerator();
			bool hasNext = enumerator.MoveNext();
			while (hasNext)
			{
				string i = enumerator.Current;
				cadena.Append("\n<\"NAME\"=\"" + i + "\">");
				hasNext = enumerator.MoveNext();
				if (hasNext)
				{
					cadena.Append(",");
				}
			}
			enumerator.Dispose();

			cadena.Append("]\n>");

			return cadena.ToString();
		}

		internal void Mostrar()
		{
			Console.WriteLine("Usuario:"+Nombre+" Pasword:"+Password);
		}
	}
}
