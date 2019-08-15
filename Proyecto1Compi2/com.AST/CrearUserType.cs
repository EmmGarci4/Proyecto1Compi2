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
	class CrearUserType:Sentencia
	{
		string nombre;
		bool ifExist;
		Dictionary<string, TipoObjetoDB> atributos;

		public CrearUserType(string nombre, Dictionary<string, TipoObjetoDB> atributos, bool ifexist, int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
			this.atributos = atributos;
			this.ifExist = ifexist;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public bool IfExist { get => ifExist; set => ifExist = value; }
		internal Dictionary<string, TipoObjetoDB> Atributos { get => atributos; set => atributos = value; }

		public override object Ejecutar(Sesion sesion)
		{
			//VALIDANDO BASEDATOS
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				if (!db.ExisteUserType(nombre))
				{
					db.AgregarUserType(new UserType(nombre, atributos));
				}
				else {
					if (!ifExist) {
						return new ThrowError(Util.TipoThrow.TypeAlreadyExists,
					"El user Type '" + nombre + "' ya existe",
					Linea, Columna);
					}
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
