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
	class Insertar : Sentencia
	{
		string tabla;
		List<Expresion> valores;
		List<string> columnas;

		public Insertar(string tabla, List<string> columnas, List<Expresion> valores, int linea, int columna) : base(linea, columna)
		{
			this.tabla = tabla;
			this.Valores = valores;
			this.Columnas = columnas;
		}

		public Insertar(string tabla, List<Expresion> valores, int linea, int columna) : base(linea, columna)
		{
			this.tabla = tabla;
			this.Valores = valores;
			this.Columnas = null;
		}

		public string NombreTabla { get => tabla; set => tabla = value; }
		public List<Expresion> Valores { get => valores; set => valores = value; }
		public List<string> Columnas { get => columnas; set => columnas = value; }

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			//VALIDANDO BASEDATOS
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				//VALLIDANDO TABLA
				if (db!=null) {
					if (db.ExisteTabla(tabla))
					{
						Tabla tab = db.BuscarTabla(tabla);
						//**************************************************************************
						//INSERSION NORMAL
						if (this.columnas == null)
						{
							int counters = tab.ContarCounters();
							if (counters>0) {
								return new ThrowError(TipoThrow.ValuesException,
											"No se puede insertar los valores por qué hay columnas de tipo counter",
											Linea, Columna);
							}
							if (this.valores.Count == (tab.Columnas.Count - counters))
							{
								//VALIDANDO
								int indiceDatos = 0;
								int indiceColumnas = 0;
								Queue<object> valoresAInsertar = new Queue<object>();
								foreach (Columna cl in tab.Columnas)
								{
									object respuesta = this.valores.ElementAt(indiceDatos).GetValor(tb, sesion);
									if (respuesta.GetType() == typeof(ThrowError))
									{
										return respuesta;
									}
									//no es un error
									if (cl.Tipo.Tipo != TipoDatoDB.COUNTER)
									{
										if (Datos.IsTipoCompatibleParaAsignar(cl.Tipo, respuesta))
										{
											object nuevovalor = Datos.CasteoImplicito(cl.Tipo, respuesta, tb, sesion, Linea, Columna);
											if (cl.IsPrimary) {
												if (cl.Tipo.Tipo == TipoDatoDB.STRING)
												{
													if (respuesta.Equals("$%_null_%$"))
													{
														return new ThrowError(TipoThrow.ValuesException,
														"No se pueden insertar nulos en la columna '" + cl.Nombre + "'",
															Linea, Columna);
													}
												}
												else if (cl.Tipo.Tipo == TipoDatoDB.DATE|| cl.Tipo.Tipo == TipoDatoDB.TIME)
												{
													MyDateTime date = (MyDateTime)respuesta;
													if (date.IsNull) {
														return new ThrowError(TipoThrow.ValuesException,
														"No se pueden insertar nulos en la columna '" + cl.Nombre + "'",
															Linea, Columna);
													}
												}
												
											}
											valoresAInsertar.Enqueue(nuevovalor);
											indiceDatos++;
										}
										else
										{
											if (respuesta.Equals("null")) {
												valoresAInsertar.Enqueue(Datos.CasteoImplicito(cl.Tipo, "null",tb,sesion,Linea,Columna));
												indiceDatos++;
											}
											else
											{
												return new ThrowError(TipoThrow.ValuesException,
											"El valor No." + (indiceDatos + 1) + " no concuerda con el tipo de dato '" + cl.Nombre + "'(" + cl.Tipo.ToString() + ")",
											Linea, Columna);

											}
											
										}
									}
									else
									{
										int UltimoValor = cl.GetUltimoValorCounter();
										UltimoValor++;
										valoresAInsertar.Enqueue(UltimoValor);
									}
									indiceColumnas++;
								}
								//INSERTANDO
								if (tab.Columnas.Count == valoresAInsertar.Count)
								{
									object correcto = tab.ValidarPk(valoresAInsertar, Linea, Columna);
									if (correcto.GetType() == typeof(ThrowError))
									{
										return correcto;
									}
									//LLENANDO TABLA
									tab.AgregarValores(valoresAInsertar);
								}
							}
							else
							{
								if (this.valores.Count == tab.Columnas.Count)
								{
									return new ThrowError(TipoThrow.CounterTypeException,
									"No se pueden insertar valores en las columnas Counter",
									Linea, Columna);
								}
								return new ThrowError(TipoThrow.ValuesException,
									"La cantidad de valores no concuerda con la cantidad de columnas en las que se puede insertar",
									Linea, Columna);
							}
						}
						//INSERSION ESPECIAL
						else
						{
							if (this.columnas.Count == this.valores.Count)
							{
								//VALIDANDO DATOS
								foreach (string nombreColumna in this.columnas)
								{
									if (!tab.ExisteColumna(nombreColumna))
									{
										return new ThrowError(TipoThrow.ColumnException,
									"La columna '" + nombreColumna + "' no existe",
									Linea, Columna);
									}
									else
									{
										Columna cl = tab.BuscarColumna(nombreColumna);
										if (cl.Tipo.Tipo == TipoDatoDB.COUNTER)
										{
											return new ThrowError(TipoThrow.CounterTypeException,
											"No se puede insertar datos en una columna autoincrementable",
											Linea, Columna);
										}
									}
								}
								//VALIDANDO COMPATIBILIDAD DE DATOS
								int indiceDatos = 0;
								Queue<object> valoresAInsertar = new Queue<object>();
								foreach (Columna cl in tab.Columnas)
								{
									if (this.columnas.Contains(cl.Nombre))
									{
										//comparar datos
										indiceDatos = this.columnas.IndexOf(cl.Nombre);
										object respuesta = this.valores.ElementAt(indiceDatos).GetValor(tb, sesion);
										if (respuesta.GetType() == typeof(ThrowError))
										{
											return respuesta;
										}
										//no es un error
										if (cl.Tipo.Tipo != TipoDatoDB.COUNTER)
										{
											if (Datos.IsTipoCompatibleParaAsignar(cl.Tipo, respuesta))
											{
												object nuevovalor = Datos.CasteoImplicito(cl.Tipo, respuesta, tb, sesion, Linea, Columna);
												if (cl.IsPrimary)
												{
													if (cl.Tipo.Tipo == TipoDatoDB.STRING)
													{
														if (respuesta.Equals("$%_null_%$"))
														{
															return new ThrowError(TipoThrow.ValuesException,
															"No se pueden insertar nulos en la columna '" + cl.Nombre + "'",
																Linea, Columna);
														}
													}
													else if (cl.Tipo.Tipo == TipoDatoDB.DATE || cl.Tipo.Tipo == TipoDatoDB.TIME)
													{
														MyDateTime date = (MyDateTime)respuesta;
														if (date.IsNull)
														{
															return new ThrowError(TipoThrow.ValuesException,
															"No se pueden insertar nulos en la columna '" + cl.Nombre + "'",
																Linea, Columna);
														}
													}
													else if (cl.Tipo.Tipo == TipoDatoDB.MAP_OBJETO || cl.Tipo.Tipo == TipoDatoDB.MAP_PRIMITIVO)
													{
														CollectionMapCql date = (CollectionMapCql)respuesta;
														if (date.IsNull)
														{
															return new ThrowError(TipoThrow.ValuesException,
															"No se pueden insertar nulos en la columna '" + cl.Nombre + "'",
																Linea, Columna);
														}
													}
													else if (cl.Tipo.Tipo == TipoDatoDB.LISTA_OBJETO || cl.Tipo.Tipo == TipoDatoDB.LISTA_PRIMITIVO)
													{
														CollectionListCql date = (CollectionListCql)respuesta;
														if (date.IsNull)
														{
															return new ThrowError(TipoThrow.ValuesException,
															"No se pueden insertar nulos en la columna '" + cl.Nombre + "'",
																Linea, Columna);
														}
													}
													else if (cl.Tipo.Tipo == TipoDatoDB.SET_OBJETO || cl.Tipo.Tipo == TipoDatoDB.SET_PRIMITIVO)
													{
														CollectionMapCql date = (CollectionMapCql)respuesta;
														if (date.IsNull)
														{
															return new ThrowError(TipoThrow.ValuesException,
															"No se pueden insertar nulos en la columna '" + cl.Nombre + "'",
																Linea, Columna);
														}
													}
												}
												valoresAInsertar.Enqueue(nuevovalor);
												indiceDatos++;
											}
											else
											{
												if (respuesta.Equals("null"))
												{
													valoresAInsertar.Enqueue(Datos.CasteoImplicito(cl.Tipo, "null",tb,sesion,Linea,Columna));
													indiceDatos++;
												}
												else
												{
													return new ThrowError(TipoThrow.ValuesException,
												"El valor No." + (indiceDatos + 1) + "(" + Datos.GetTipoObjetoDB(respuesta).ToString() + ")" + " no concuerda con el tipo de dato '" + cl.Nombre + "'(" + cl.Tipo.ToString() + ")",
												Linea, Columna);
												}
											}
										}
										else
										{
											int UltimoValor = cl.GetUltimoValorCounter();
											UltimoValor++;
											valoresAInsertar.Enqueue(UltimoValor);
										}
									}
									else
									{
										if (cl.IsPrimary)
										{
											if (cl.Tipo.Tipo == TipoDatoDB.COUNTER)
											{
												int UltimoValor = cl.GetUltimoValorCounter();
												UltimoValor++;
												valoresAInsertar.Enqueue(UltimoValor);
											}
											else
											{
												return new ThrowError(TipoThrow.ValuesException,
												"No se puede insertar un dato nulo en una llave primaria",
												Linea, Columna);
											}
										}
										else
										{
											valoresAInsertar.Enqueue(Datos.CasteoImplicito(cl.Tipo, "null", tb, sesion, Linea, Columna));
										}
									}
								}
								//INSERTANDO
								if (tab.Columnas.Count == valoresAInsertar.Count)
								{
									object correcto = tab.ValidarPk(valoresAInsertar, Linea, Columna);
									if (correcto.GetType() == typeof(ThrowError))
									{
										return correcto;
									}
									//LLENANDO TABLA
									tab.AgregarValores(valoresAInsertar);
								}
							}
							else
							{
								return new ThrowError(TipoThrow.ValuesException,
									"La cantidad de valores no concuerda con la cantidad de columnas en las que se puede insertar",
									Linea, Columna);
							}

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
