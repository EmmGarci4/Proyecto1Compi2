using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;
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

		public override object Ejecutar(TablaSimbolos tb)
		{
			if (Analizador.ExisteDB(nombre))
			{
				if (!ifExist) {
					//lanzar mensaje de error
					return new ThrowError(TipoThrow.BDAlreadyExists, "La base de datos '"+Nombre+"' ya existe", Linea, Columna);
				}
			}
			else {
				Analizador.AddBaseDatos(new BaseDatos(Nombre));
				if (Analizador.ExisteUsuario(Analizador.Sesion.Usuario)) {
					Analizador.BuscarUsuario(Analizador.Sesion.Usuario).Permisos.Add(Nombre);
				}
			}
			return null;
		}
	}
}
