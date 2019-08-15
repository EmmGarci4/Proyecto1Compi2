using com.Analisis;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class EliminarUserType : Sentencia
	{
		string nombre;

		public EliminarUserType(string nombre, int linea, int columna) : base(linea, columna)
		{
			this.Nombre = nombre;
		}

		public string Nombre { get => nombre; set => nombre = value; }

		public override object Ejecutar(Sesion sesion)
		{
			//VALIDANDO BASEDATOS
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				if (db.ExisteUserType(nombre))
				{
					db.EliminarUserType(nombre);
				}
				else
				{

						return new ThrowError(Util.TipoThrow.TypeDontExists,
					"El user Type '" + nombre + "' no existe",
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
