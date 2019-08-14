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

		public Sesion(string usuario, string baseDatos)
		{
			this.Usuario = usuario;
			this.baseDatos = baseDatos;
		}

		public string Usuario { get => usuario; set => usuario = value; }
		public string DBActual { get => baseDatos; set => baseDatos = value; }
	}
}
