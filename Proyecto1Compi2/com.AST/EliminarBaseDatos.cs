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
	class EliminarBaseDatos:Sentencia
	{
			string nombre;

			public EliminarBaseDatos( string nombre, int linea, int columna) : base(linea, columna)
			{
				this.Nombre = nombre;
			}

			public string Nombre { get => nombre; set => nombre = value; }

			public override object Ejecutar(Sesion sesion, TablaSimbolos tb)
			{
			if (Analizador.ExisteDB(nombre))
			{
				Analizador.EliminarDB(nombre);
				Analizador.ElminarPermisoDeUsuario(nombre);

				if (sesion.DBActual!=null) {
					if (sesion.DBActual == nombre)
					{
						sesion.DBActual = null;

					}
				}
			}
			else {
				return new ThrowError(TipoThrow.BDDontExists, "La base de datos '" + Nombre + "' no existe", Linea, Columna);
			}
			return null;
			}
		}
}
