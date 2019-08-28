using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
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

		public CrearUsuario(string nombre, string passwd, int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
			this.passwd = passwd;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public string Passwd { get => passwd; set => passwd = value; }

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			if (!Analizador.ExisteUsuario(nombre))
			{
				Analizador.AddUsuario(new Usuario(nombre, passwd));
			}
			else {
				return new ThrowError(Util.TipoThrow.UserAleadyExists, "El usuario '" + nombre + "' ya existe", Linea, Columna);
			}
			return null;
		}
	}
}
