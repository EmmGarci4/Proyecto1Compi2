using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class Sesion
	{
		string usuario;
		string baseDatos;
		List<String> mensajes;

		public Sesion(string usuario, string baseDatos)
		{
			this.Usuario = usuario;
			this.baseDatos = baseDatos;
			this.mensajes = new List<string>();
		}

		public string Usuario { get => usuario; set => usuario = value; }
		public string DBActual { get => baseDatos; set => baseDatos = value; }
		public List<string> Mensajes { get => mensajes; set => mensajes = value; }
	}
}
