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
	class UsarBaseDatos:Sentencia
	{
		string nombre;

		public UsarBaseDatos(string nombre, int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
		}

		public string Nombre { get => nombre; set => nombre = value; }

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			if (Analizador.ExisteDB(nombre))
			{
				BaseDatos db= Analizador.BuscarDB(nombre);
				if (!db.EnUso)
				{
					//liberar base de datos en uso
					if (sesion.DBActual!=null) {
						Analizador.BuscarDB(sesion.DBActual).EnUso = false;
					}
					Usuario usuario = Analizador.BuscarUsuario(sesion.Usuario);
					if (usuario != null)
					{
						if (usuario.ExistePermiso(nombre))
						{
							sesion.DBActual = nombre;
							db.EnUso = true;
						}
						else
						{
							return new ThrowError(TipoThrow.Exception, "No tiene permiso para utilizar la base de datos '" + Nombre + "'", Linea, Columna);
						}
					}
					else {
						return new ThrowError(TipoThrow.UserDontExists, "El usuario '" +sesion.Usuario+ "' no existe", Linea, Columna);
					}
				}
				else {
					if (!sesion.DBActual.Equals(nombre)) {
						return new ThrowError(TipoThrow.Exception, "La base de datos '" + Nombre + "' esta en uso", Linea, Columna);
					}
				}
			}
			else {
				return new ThrowError(TipoThrow.BDDontExists,"La base de datos '"+Nombre+"' no existe",Linea,Columna);
			}
			return null;
		}
	}
}
