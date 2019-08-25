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
	class TruncarTabla:Sentencia
	{
		string nombre;

		public TruncarTabla(string nombre,int linea,int columna):base(linea,columna)
		{
			this.nombre = nombre;
		}

		public string Nombre { get => nombre; set => nombre = value; }

		public override object Ejecutar(TablaSimbolos tb)
		{
			//VALIDANDO BASEDATOS
			if (Analizador.Sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(Analizador.Sesion.DBActual);
				//VALLIDANDO TABLA
				if (db.ExisteTabla(Nombre))
				{
					db.BuscarTabla(nombre).Truncar();
				}
				else
				{
						return new ThrowError(Util.TipoThrow.TableDontExists,
						"La tabla '" + nombre + "' no existe",
						Linea, Columna);
					
				}
			}
			else
			{
				return new ThrowError(Util.TipoThrow.UseBDException,
					"No se puede ejecutar la sentencia porque no hay una base de datos seleccionada",
					Linea, Columna);
			}
			return null;
		}
	}
}
