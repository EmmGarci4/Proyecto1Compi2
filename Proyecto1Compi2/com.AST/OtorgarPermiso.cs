using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class OtorgarPermiso:Sentencia
	{
		string usuario;
		string baseDatos;

		public OtorgarPermiso(string usuario, string baseDatos,int linea,int columna):base(linea,columna)
		{
			this.usuario = usuario;
			this.baseDatos = baseDatos;
		}

		public string Usuario { get => usuario; set => usuario = value; }
		public string BaseDatos { get => baseDatos; set => baseDatos = value; }

		public override object Ejecutar()
		{
			Console.WriteLine("Otorgar permisos a "+Usuario+" sobre "+baseDatos);
			return null;
		}
	}
}
