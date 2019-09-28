using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

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

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			if (Analizador.BuscarUsuario(sesion.Usuario)!=null) {
				if (Analizador.BuscarUsuario(sesion.Usuario).ExistePermiso(baseDatos))
				{
					if (Analizador.ExisteUsuario(usuario))
					{
						if (Analizador.ExisteDB(baseDatos))
						{
							Usuario u = Analizador.BuscarUsuario(usuario);
							if (u.ExistePermiso(baseDatos))
							{
								return new ThrowError(TipoThrow.Exception, "El usuario '" + usuario + "' ya tiene permisos sobre la base de datos '" + baseDatos + "'", Linea, Columna);
							}
							else
							{
								u.Permisos.Add(baseDatos);
							}
						}
						else
						{
							return new ThrowError(TipoThrow.BDDontExists, "La base de datos '" + baseDatos + "' no existe", Linea, Columna);
						}
					}
					else
					{
						return new ThrowError(TipoThrow.UserDontExists, "El usuario '" + usuario + "' no existe", Linea, Columna);
					}
				}
				else
				{
					return new ThrowError(TipoThrow.Exception, "No tiene permisos sobre la base de datos '" + baseDatos + "' para asignar permisos a otros usuarios", Linea, Columna);
				}
			}
			else
			{
				return new ThrowError(TipoThrow.UserDontExists, "El usuario '" + sesion.Usuario + "' no existe", Linea, Columna);
			}
			return null;
		}
	}
}
