using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class DenegarPermiso : Sentencia
	{
		string usuario;
		string baseDatos;

		public DenegarPermiso(string usuario, string baseDatos, int linea, int columna) : base(linea, columna)
		{
			this.usuario = usuario;
			this.baseDatos = baseDatos;
		}

		public string Usuario { get => usuario; set => usuario = value; }
		public string BaseDatos { get => baseDatos; set => baseDatos = value; }

		public override object Ejecutar(Usuario usuarioActual)
		{
			if (usuarioActual.ExistePermiso(BaseDatos))
			{


				if (Analizador.ExisteUsuario(usuario))
				{
					if (Analizador.ExisteDB(baseDatos))
					{
						Usuario u = Analizador.BuscarUsuario(usuario);
						if (!u.ExistePermiso(baseDatos))
						{
							return new ThrowError(TipoThrow.Exception, "El usuario '" + usuario + "' no tiene permisos sobre la base de datos '" + baseDatos + "'", Linea, Columna);
						}
						else
						{
							u.Permisos.Remove(baseDatos);
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
			else {
				return new ThrowError(TipoThrow.Exception, "No tiene permisos sobre la base de datos '" + baseDatos + "' para revocar permisos a otros usuarios", Linea, Columna);

			}
			return null;
		}
	}
}
