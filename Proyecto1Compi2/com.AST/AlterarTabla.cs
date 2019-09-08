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
	class AlterarTabla : Sentencia
	{
		TipoAccion accion;
		string nombreTabla;
		List<Columna> agregarCols;
		List<string> quitarCols;

		public AlterarTabla(TipoAccion accion, string tabla, List<string> quitarCols, int linea, int columna) : base(linea, columna)
		{
			this.accion = accion;
			this.nombreTabla = tabla;
			this.agregarCols = null;
			this.quitarCols = quitarCols;
		}

		public AlterarTabla(TipoAccion accion, string tabla, List<Columna> agregarCols, int linea, int columna) : base(linea, columna)
		{
			this.accion = accion;
			this.nombreTabla = tabla;
			this.agregarCols = agregarCols;
			this.quitarCols = null;
		}

		public TipoAccion Accion { get => accion; set => accion = value; }
		public string Tabla { get => nombreTabla; set => nombreTabla = value; }
		public List<string> QuitarCols { get => quitarCols; set => quitarCols = value; }
		internal List<Columna> AgregarCols { get => agregarCols; set => agregarCols = value; }

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			//VALIDANDO BASEDATOS
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				//VALLIDANDO TABLA
				if (db.ExisteTabla(nombreTabla))
				{
					Tabla tabla = db.BuscarTabla(nombreTabla);
					if (accion == TipoAccion.Agregar)
					{
						//VALIDANDO
						foreach (Columna cl in agregarCols)
						{
							if (!tabla.ExisteColumna(cl.Nombre))
							{
								//VALIDANDO COUNTER
								if (cl.Tipo.Tipo == Util.TipoDatoDB.COUNTER)
								{
									return new ThrowError(Util.TipoThrow.Exception,
										"No se pueden agregar llaves primarias a la tabla",
										Linea, Columna);
								}
								else
								{
									//VALIDANDO TIPO
									if (!CrearTabla.EsListaDeLista(cl))
									{
										if (cl.IsPrimary)
										{
											//validando que una columna pk no sea agregada
											return new ThrowError(Util.TipoThrow.Exception,
												"No se pueden agregar llaves primarias a la tabla",
												Linea, Columna);
										}
									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se permiten listas de listas como tipo de dato en una columna",
											Linea, Columna);
									}
								}
							}
							else
							{
								return new ThrowError(Util.TipoThrow.Exception,
									"La tabla ya contiene una columna con nombre '" + cl.Nombre + "'",
									Linea, Columna);
							}
						}
						//INSERTANDO
						//TODAS LAS VALIDACIONES FUERON PASADAS
						foreach (Columna cl in agregarCols)
						{
							tabla.AgregarColumnaNueva(cl);
						}
					}
					else
					{
						//VALIDANDO
						foreach (string col in quitarCols)
						{
							if (tabla.ExisteColumna(col))
							{
								if (tabla.BuscarColumna(col).IsPrimary)
								{
									return new ThrowError(Util.TipoThrow.Exception,
										"la columna '" + col + "' no se puede eliminar porque es una llave primaria",
										Linea, Columna);
								}
							}
							else
							{
								return new ThrowError(Util.TipoThrow.Exception,
											"la columna '" + col + "' no existe",
											Linea, Columna);
							}
						}
						//AGREGANDO
						//LAS VALIDACIONES FUERON PASADAS
						foreach (string col in quitarCols)
						{
							tabla.EliminarColumna(col);
						}
					}
				}
				else
				{
					return new ThrowError(Util.TipoThrow.TableDontExists,
						"La tabla '" + nombreTabla + "' no existe",
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
