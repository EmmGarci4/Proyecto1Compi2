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
	class Actualizar : Sentencia
	{
		List<AsignacionColumna> asignaciones;
		string tabla;
		Where condicion;

		public Actualizar(List<AsignacionColumna> asignaciones, string tabla, Where condicion, int linea, int columna) : base(linea, columna)
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

		public override object Ejecutar(TablaSimbolos tb, Sesion sesion)
		{
			//VALIDANDO BASE_DATOS
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				//VALLIDANDO TABLA
				if (db.ExisteTabla(tabla))
				{
					Tabla miTabla = db.BuscarTabla(tabla);
					int i = 0;
					for (i = 0; i < miTabla.ContadorFilas; i++)
					{
						List<string> listaNombres = new List<string>();
						//AGREGANDO FILA A LA TABLA DE SIMBOLOS
						TablaSimbolos local = new TablaSimbolos(tb);

						foreach (Columna cl in miTabla.Columnas)
						{
							listaNombres.Add(cl.Nombre);
							object dato = cl.Datos.ElementAt(i);
							Simbolo s = new Simbolo(cl.Nombre, dato, cl.Tipo, Linea, Columna);
							local.AgregarSimbolo(s);

						}
						//**************************************************************************
						foreach (AsignacionColumna asignacion in this.asignaciones)
						{
							if (condicion != null)
							{
								object condicionWhere = condicion.GetValor(local, sesion);
								if (condicionWhere != null)
								{
									if (condicionWhere.GetType() == typeof(ThrowError))
									{
										return condicionWhere;
									}
									if ((bool)condicionWhere)
									{
										object res = asignacion.Ejecutar(local, sesion);
										if (res != null)
										{
											if (res.GetType() == typeof(ThrowError))
											{
												return res;
											}
										}
									}
								}
							}
							else
							{
								object res = asignacion.Ejecutar(local, sesion);
								if (res != null)
								{
									if (res.GetType() == typeof(ThrowError))
									{
										return res;
									}
								}
							}
							//**************************************************************************
						}
						//**************************************************************************
						//reemplazando valores
						Queue<object> valores = new Queue<object>();
						foreach (string columna in listaNombres)
						{
							Simbolo nuevoValor = local.GetSimbolo(columna);
							if (nuevoValor != null)
							{
								valores.Enqueue(nuevoValor.Valor);
							}
						}
						if (valores.Count == miTabla.Columnas.Count)
						{
							object vares = miTabla.ValidarPk(valores, Linea, Columna);
							if (vares != null) {
								if (vares.GetType()==typeof(ThrowError)) {
									return vares;
								}
								if ((bool)vares)
								{
									miTabla.ReemplazarValores(valores, i);
								}
							}
							
						}
					}
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
