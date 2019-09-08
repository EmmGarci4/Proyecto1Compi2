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
	class Actualizar:Sentencia
	{
		List<AsignacionColumna> asignaciones;
		string tabla;
		Where condicion;

		public Actualizar(List<AsignacionColumna> asignaciones, string tabla, Where condicion,int linea,int columna):base(linea,columna)
		{
			this.asignaciones = asignaciones;
			this.tabla = tabla;
			this.condicion = condicion;
		}

		public Actualizar(List<AsignacionColumna> asignaciones, string tabla, int linea, int columna) : base(linea, columna)
		{
			this.asignaciones = asignaciones;
			this.tabla = tabla;
			this.condicion = null;
		}

		public string Nombre { get => tabla; set => tabla = value; }
		internal List<AsignacionColumna> Asignaciones { get => asignaciones; set => asignaciones = value; }
		internal Where Condicion { get => condicion; set => condicion = value; }

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{

			//VALIDANDO BASE_DATOS
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				//VALLIDANDO TABLA
				if (db.ExisteTabla(tabla))
				{
					Tabla tab = db.BuscarTabla(tabla);
					//**************************************************************************
					foreach (AsignacionColumna asignacion in this.asignaciones) {

					}
					//**************************************************************************
				}
				else
				{
					return new ThrowError(Util.TipoThrow.TableDontExists,
					"La tabla '" + tabla + "' no existe",
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
