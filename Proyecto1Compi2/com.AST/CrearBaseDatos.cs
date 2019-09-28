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

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
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
				if (Analizador.ExisteUsuario(sesion.Usuario)) {
					Usuario usuario = Analizador.BuscarUsuario(sesion.Usuario);

					if (usuario != null)
					{
						usuario.Permisos.Add(Nombre);
					}
					else {
						if (!sesion.Usuario.Equals("admin"))
						{
							if (Analizador.ExisteUsuario("admin"))
							{
								Analizador.BuscarUsuario("admin").Permisos.Add(nombre);
							}
						}
						return new ThrowError(TipoThrow.UserDontExists, "El usuario '" + sesion.Usuario + "' no existe", Linea, Columna);
					}
					if (!sesion.Usuario.Equals("admin")) {
						if (Analizador.ExisteUsuario("admin")) {
							Analizador.BuscarUsuario("admin").Permisos.Add(nombre);
						}
					}
				}
			}
			return null;
		}
	}
}
