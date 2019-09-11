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
	class Borrar : Sentencia
	{
		string tabla;
		AccesoPar algo; //para eliminar de la tabla
						//puede ser un nombre o un acceso a collection
		Where condicion;

		public Borrar(string tabla, int linea, int columna) : base(linea, columna)
		{
			this.tabla = tabla;
			this.algo = null;
			this.condicion = null;
		}

		public Borrar(string tabla, AccesoPar algo, int linea, int columna) : base(linea, columna)
		{
			this.tabla = tabla;
			this.algo = algo;
			this.condicion = null;
		}
		public Borrar(string tabla, Where where, int linea, int columna) : base(linea, columna)
		{
			this.tabla = tabla;
			this.algo = null;
			this.condicion = where;
		}

		public Borrar(string tabla, AccesoPar algo, Where where, int linea, int columna) : base(linea, columna)
		{
			this.tabla = tabla;
			this.algo = algo;
			this.condicion = where;
		}


		public string Tabla { get => tabla; set => tabla = value; }
		public AccesoPar Algo { get => algo; set => algo = value; }
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
					//**************************************************************************
					if (algo == null)
					{
						if (condicion == null)
						{
							miTabla.Truncar();
						}
						else
						{
							//tabla
							int i = 0;
							for (i = 0; i < miTabla.ContadorFilas; i++)
							{
								//AGREGANDO FILA A LA TABLA DE SIMBOLOS
								TablaSimbolos local = new TablaSimbolos(tb);
								foreach (Columna cl in miTabla.Columnas)
								{
									object dato = cl.Datos.ElementAt(i);
									Simbolo s;
									if (cl.Tipo.Tipo == TipoDatoDB.COUNTER)
									{
										s = new Simbolo(cl.Nombre, dato, new TipoObjetoDB(TipoDatoDB.INT, "int"), Linea, Columna);

									}
									else
									{
										s = new Simbolo(cl.Nombre, dato, cl.Tipo, Linea, Columna);
									}

									local.AgregarSimbolo(s);
								}
								//**************************************************************************
								object condicionWhere = condicion.GetValor(local, sesion);
								if (condicionWhere != null)
								{
									if (condicionWhere.GetType() == typeof(ThrowError))
									{
										return condicionWhere;
									}
									if ((bool)condicionWhere)
									{
										//eliminar valor
										miTabla.EliminarDatos(i);
									}
								}
								//**************************************************************************
							}
						}
					}
					else
					{
						if (algo.Tipo == TipoAcceso.AccesoArreglo)
						{
							AccesoArreglo arreglo = (AccesoArreglo)algo.Value;
							
							if (miTabla.ExisteColumna(arreglo.Nombre))
							{
								Columna col = miTabla.BuscarColumna(arreglo.Nombre);
								int i = 0;
								for (i=0;i<miTabla.ContadorFilas;i++) {
									//AGREGANDO FILA A LA TABLA DE SIMBOLOS
									TablaSimbolos local = new TablaSimbolos(tb);
									foreach (Columna cl in miTabla.Columnas)
									{
										object dato = cl.Datos.ElementAt(i);
										Simbolo s;
										if (cl.Tipo.Tipo == TipoDatoDB.COUNTER)
										{
											s = new Simbolo(cl.Nombre, dato, new TipoObjetoDB(TipoDatoDB.INT, "int"), Linea, Columna);

										}
										else
										{
											s = new Simbolo(cl.Nombre, dato, cl.Tipo, Linea, Columna);
										}

										local.AgregarSimbolo(s);

									}
									//**********************
									if (col.Tipo.Tipo == TipoDatoDB.LISTA_OBJETO || col.Tipo.Tipo == TipoDatoDB.SET_OBJETO ||
										col.Tipo.Tipo == TipoDatoDB.LISTA_PRIMITIVO || col.Tipo.Tipo == TipoDatoDB.SET_PRIMITIVO)
									{
										object vvv = arreglo.Valor.GetValor(local, sesion);
										if (vvv != null)
										{
											if (vvv.GetType() == typeof(ThrowError))
											{
												return vvv;
											}
											if (vvv.GetType() == typeof(int))
											{
												int indice = (int)vvv;
												CollectionListCql collection = (CollectionListCql)col.Datos.ElementAt(i);
												if (indice < collection.Count)
												{
													if (condicion == null)
													{
														collection.RemoveAt(indice);
													}
													else
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
																collection.RemoveAt(indice);
															}
														}
													}
												}
												else
												{
													//error index out 
													return new ThrowError(Util.TipoThrow.IndexOutException,
														"El indice es mayor a la cantidad de elementos en la lista",
														Linea, Columna);
												}
											}
											else
											{
												//error de index
												return new ThrowError(Util.TipoThrow.Exception,
											"El indice de una lista o set debe ser entero",
											Linea, Columna);
											}
										}
									}
									else if (col.Tipo.Tipo == TipoDatoDB.MAP_OBJETO || col.Tipo.Tipo == TipoDatoDB.MAP_PRIMITIVO)
									{

										object vvv = arreglo.Valor.GetValor(local, sesion);
										if (vvv != null)
										{
											if (vvv.GetType() == typeof(ThrowError))
											{
												return vvv;
											}
											CollectionMapCql collection = (CollectionMapCql)col.Datos.ElementAt(i);
											if (Datos.IsTipoCompatible(collection.TipoLlave,vvv))
											{
												int indice = (int)vvv;
												if (indice < collection.Count)
												{
													if (condicion == null)
													{
														collection.Remove(indice);
													}
													else
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
																collection.Remove(indice);
															}
														}
													}
												}
												else
												{
													//error index out 
													return new ThrowError(Util.TipoThrow.IndexOutException,
														"El valor '"+vvv+"' no existe en el Map",
														Linea, Columna);
												}
											}
											else
											{
												//error de index
												return new ThrowError(Util.TipoThrow.Exception,
											"El valor '"+vvv+"' no concuerda con el tipo '"+collection.TipoLlave.ToString()+"'",
											Linea, Columna);
											}
										}

									}
									else
									{
										//error
										return new ThrowError(Util.TipoThrow.Exception,
											"La columna '" + tabla + "' no es un Collection",
											Linea, Columna);
									}
								}
							}
							else
							{
								//error
								return new ThrowError(Util.TipoThrow.ColumnException,
										"La columna '" + tabla + "' no existe",
										Linea, Columna);
							}
						}
						else
						{
							//error
							return new ThrowError(Util.TipoThrow.Exception,
										"No se puede ejecutar la sentencia",
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
