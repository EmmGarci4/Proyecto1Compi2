using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.Analisis;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class Acceso : Expresion
	{

		Queue<AccesoPar> objetos;

		public Acceso(Queue<AccesoPar> objetos, int linea, int columna) : base(linea, columna)
		{
			this.objetos = objetos;
		}

		public Acceso(int linea, int columna) : base(linea, columna)
		{
			this.objetos = new Queue<AccesoPar>();
		}

		public Queue<AccesoPar> Objetos { get => objetos; set => objetos = value; }

		public override object GetValor(TablaSimbolos ts)
		{
			AccesoPar valor = objetos.Dequeue();
			switch (valor.Tipo)
			{
				case TipoAcceso.AccesoArreglo:
					AccesoArreglo acceso = (AccesoArreglo)valor.Value;
					if (acceso.Nombre.StartsWith("@"))
					{

						object respuesta = acceso.GetValor(ts);
						if (respuesta.GetType() == typeof(ThrowError))
						{
							return respuesta;
						}
						TipoObjetoDB tipoRespuesta = Datos.GetTipoObjetoDB(respuesta);
						Simbolo nuevoSimbolo = new Simbolo(acceso.ToString(), respuesta, tipoRespuesta, Linea, Columna);
						return RetornarValorSobreVariable(nuevoSimbolo, ts);
					}
					else {
						//ACCESO A ALGO EN LAS TABLAS 

					}
					break;
				case TipoAcceso.Campo:
				//tablas Y COLUMNAS
				//enviar sobre campo
				case TipoAcceso.LlamadaFuncion:
					//ejecutar llamada de funcion y retornar el valor 
					break;
				case TipoAcceso.Variable:
					if (ts.ExisteSimbolo(valor.Value.ToString()))
					{
						Simbolo sim = ts.GetSimbolo(valor.Value.ToString());
						object respuesta = RetornarValorSobreVariable(sim, ts);

						return respuesta;
					}
					else
					{
						return new ThrowError(Util.TipoThrow.Exception,
							"La variable '" + valor.Value.ToString() + "' no existe",
							Linea, Columna);
					}
			}
			return null;
		}

		private object RetornarValorSobreVariable(Simbolo sim, TablaSimbolos ts)
		{
			if (objetos.Count > 0)
			{
				AccesoPar valor = objetos.Dequeue();
				switch (valor.Tipo)
				{
					case TipoAcceso.AccesoArreglo:
						if (sim.TipoDato.Tipo==TipoDatoDB.OBJETO)
						{
							//VERIFICAR SI EL OBJETO TIENE UNA LISTA ADENTRO

						}
						else {
							return new ThrowError(Util.TipoThrow.Exception,
								"'" + sim.Nombre + "' no contiene un arreglo",
								Linea, Columna);
						}
						break;
					case TipoAcceso.Campo:
						if (sim.TipoDato.Tipo == Util.TipoDatoDB.OBJETO)
						{
							//OBTENER OBJETO Y ACCEDER A PROPIEDADES

						}
						else
						{
							return new ThrowError(Util.TipoThrow.Exception,
								"No se puede acceder a un valor en '" + sim.Nombre + "' por que no contiene la propiedad '" + valor.Value.ToString() + "'",
								Linea, Columna);
						}
						break;
					case TipoAcceso.LlamadaFuncion:
						if (sim.TipoDato.Tipo == Util.TipoDatoDB.STRING)
						{
							#region FuncionesNativasSobreCadenas
							string VALORFINAL = sim.Valor.ToString();
							LlamadaFuncion llamada = (LlamadaFuncion)valor.Value;
							string llaveFuncion = llamada.getLlave(ts);
							switch (llaveFuncion.ToLower())
							{
								case "length()":
									Simbolo s = new Simbolo(sim.Nombre + "." + llaveFuncion, VALORFINAL.Length, new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
									return RetornarValorSobreVariable(s, ts);
								case "touppercase()":
									s = new Simbolo(sim.Nombre + "." + llaveFuncion, VALORFINAL.ToUpper(), new Util.TipoObjetoDB(Util.TipoDatoDB.STRING, "string"), 0, 0);
									return RetornarValorSobreVariable(s, ts);
								case "tolowercase()":
									s = new Simbolo(sim.Nombre + "." + llaveFuncion, VALORFINAL.ToLower(), new Util.TipoObjetoDB(Util.TipoDatoDB.STRING, "string"), 0, 0);
									return RetornarValorSobreVariable(s, ts);
								case "startswith(string)":
									s = new Simbolo(sim.Nombre + "." + llaveFuncion, VALORFINAL.StartsWith(llamada.Parametros.ElementAt(0).GetValor(ts).ToString()),
										new Util.TipoObjetoDB(Util.TipoDatoDB.BOOLEAN, "boolean"), 0, 0);
									return RetornarValorSobreVariable(s, ts);
								case "endswith(string)":
									s = new Simbolo(sim.Nombre + "." + llaveFuncion, VALORFINAL.EndsWith(llamada.Parametros.ElementAt(0).GetValor(ts).ToString()),
										new Util.TipoObjetoDB(Util.TipoDatoDB.BOOLEAN, "boolean"), 0, 0);
									return RetornarValorSobreVariable(s, ts);
								case "substring(int,int)":
									try
									{
										s = new Simbolo(sim.Nombre + "." + llaveFuncion,
										VALORFINAL.Substring(int.Parse(llamada.Parametros.ElementAt(0).GetValor(ts).ToString()), int.Parse(llamada.Parametros.ElementAt(1).GetValor(ts).ToString())),
										new Util.TipoObjetoDB(Util.TipoDatoDB.STRING, "string"), 0, 0);
									}
									catch (ArgumentOutOfRangeException)
									{
										return new ThrowError(Util.TipoThrow.Exception,
										"Los valores ingresados en la función '" + llaveFuncion + "' están fuera de los límites",
										Linea, Columna);
									}
									return RetornarValorSobreVariable(s, ts);
								default:
									return new ThrowError(Util.TipoThrow.Exception,
										"La función '" + llaveFuncion + "' no se puede aplicar sobre '" + sim.Nombre + "'",
										Linea, Columna);
							}
							#endregion
						}
						else if (sim.TipoDato.Tipo == Util.TipoDatoDB.DATE)
						{
							#region FuncionesNativasSobreFecha
							MyDateTime hora = (MyDateTime)sim.Valor;
							LlamadaFuncion llamada = (LlamadaFuncion)valor.Value;
							string llaveFuncion = llamada.getLlave(ts);
							switch (llaveFuncion.ToLower())
							{
								case "getyear()":
									Simbolo s = new Simbolo(sim.Nombre + "." + llaveFuncion, hora.Dato.Year,
										new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
									return RetornarValorSobreVariable(s, ts);
								case "getmonth()":
									s = new Simbolo(sim.Nombre + "." + llaveFuncion, hora.Dato.Month,
									   new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
									return RetornarValorSobreVariable(s, ts);
								case "getday()":
									s = new Simbolo(sim.Nombre + "." + llaveFuncion, hora.Dato.Day,
									   new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
									return RetornarValorSobreVariable(s, ts);
								default:
									return new ThrowError(Util.TipoThrow.Exception,
										"La función '" + llaveFuncion + "' no se puede aplicar sobre '" + sim.Nombre + "'",
										Linea, Columna);
							}
							#endregion
						}
						else if (sim.TipoDato.Tipo == Util.TipoDatoDB.TIME)
						{
							#region FuncionesNativasSobreHora

							MyDateTime hora = (MyDateTime)sim.Valor;
							LlamadaFuncion llamada = (LlamadaFuncion)valor.Value;
							string llaveFuncion = llamada.getLlave(ts);
							switch (llaveFuncion.ToLower())
							{
								case "gethour()":
									Simbolo s = new Simbolo(sim.Nombre + "." + llaveFuncion, hora.Dato.Hour,
										new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
									return RetornarValorSobreVariable(s, ts);
								case "getminuts()":
									s = new Simbolo(sim.Nombre + "." + llaveFuncion, hora.Dato.Minute,
									   new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
									return RetornarValorSobreVariable(s, ts);
								case "getseconds()":
									s = new Simbolo(sim.Nombre + "." + llaveFuncion, hora.Dato.Second,
									   new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
									return RetornarValorSobreVariable(s, ts);
								default:
									return new ThrowError(Util.TipoThrow.Exception,
										"La función '" + llaveFuncion + "' no se puede aplicar sobre '" + sim.Nombre + "'",
										Linea, Columna);
							}
							#endregion
						}
						else if (sim.TipoDato.Tipo == Util.TipoDatoDB.LISTA_OBJETO || sim.TipoDato.Tipo == Util.TipoDatoDB.LISTA_PRIMITIVO ||
						  sim.TipoDato.Tipo == Util.TipoDatoDB.SET_OBJETO || sim.TipoDato.Tipo == Util.TipoDatoDB.SET_PRIMITIVO)
						{
							#region FuncionesNativasSobreListYSet
							LlamadaFuncion llamada = (LlamadaFuncion)valor.Value;
							CollectionListCql collection = (CollectionListCql)sim.Valor;
							string llaveFuncion = llamada.Nombre;
							switch (llaveFuncion.ToLower())
							{
								case "insert":
									if (llamada.Parametros.Count == 1)
									{
										object nuevo = llamada.Parametros.ElementAt(0).GetValor(ts);
										TipoOperacion t = llamada.Parametros.ElementAt(0).GetTipo(ts);
										switch (t)
										{
											case TipoOperacion.Numero:
												if (collection.TipoDato.Tipo == TipoDatoDB.INT)
												{
													if (!nuevo.ToString().Contains("."))
													{
														object posibleError = collection.AddItem(nuevo, Linea, Columna);
														if (posibleError != null)
														{
															if (posibleError.GetType() == typeof(ThrowError))
															{
																return posibleError;
															}
														}
													}
													else
													{
														return new ThrowError(Util.TipoThrow.Exception,
															"No se puede almacenar un valor double en un Collection tipo int",
															Linea, Columna);
													}
												}
												else if (collection.TipoDato.Tipo == TipoDatoDB.DOUBLE)
												{
													object posibleError = collection.AddItem(nuevo, Linea, Columna);
													if (posibleError != null)
													{
														if (posibleError.GetType() == typeof(ThrowError))
														{
															return posibleError;
														}
													}
												}
												else
												{
													return new ThrowError(Util.TipoThrow.Exception,
															"No se puede almacenar un valor numerico en un Collection tipo '" + collection.TipoDato.ToString() + "'",
															Linea, Columna);
												}
												break;
											case TipoOperacion.Booleano:
												if (collection.TipoDato.Tipo == TipoDatoDB.BOOLEAN)
												{
													object posibleError = collection.AddItem(nuevo, Linea, Columna);
													if (posibleError != null)
													{
														if (posibleError.GetType() == typeof(ThrowError))
														{
															return posibleError;
														}
													}
												}
												else
												{
													return new ThrowError(Util.TipoThrow.Exception,
														"No se puede almacenar un valor booleano en un Collection tipo '" + collection.TipoDato.ToString() + "'",
														Linea, Columna);
												}
												break;
											case TipoOperacion.Fecha:
												if (collection.TipoDato.Tipo == TipoDatoDB.DATE)
												{
													object posibleError = collection.AddItem(nuevo, Linea, Columna);
													if (posibleError != null)
													{
														if (posibleError.GetType() == typeof(ThrowError))
														{
															return posibleError;
														}
													}
												}
												else
												{
													return new ThrowError(Util.TipoThrow.Exception,
														"No se puede almacenar un valor de fecha en un Collection tipo '" + collection.TipoDato.ToString() + "'",
														Linea, Columna);
												}
												break;
											case TipoOperacion.Hora:
												if (collection.TipoDato.Tipo == TipoDatoDB.TIME)
												{
													object posibleError = collection.AddItem(nuevo, Linea, Columna);
													if (posibleError != null)
													{
														if (posibleError.GetType() == typeof(ThrowError))
														{
															return posibleError;
														}
													}
												}
												else
												{
													return new ThrowError(Util.TipoThrow.Exception,
														"No se puede almacenar un valor de hora en un Collection tipo '" + collection.TipoDato.ToString() + "'",
														Linea, Columna);
												}
												break;
											case TipoOperacion.Objeto:
												if (collection.TipoDato.Tipo == TipoDatoDB.OBJETO)
												{
													object posibleError = collection.AddItem(nuevo, Linea, Columna);
													if (posibleError != null)
													{
														if (posibleError.GetType() == typeof(ThrowError))
														{
															return posibleError;
														}
													}
												}
												else
												{
													return new ThrowError(Util.TipoThrow.Exception,
														"No se puede almacenar un valor de objeto en un Collection tipo '" + collection.TipoDato.ToString() + "'",
														Linea, Columna);
												}
												break;
											case TipoOperacion.String:
												if (collection.TipoDato.Tipo == TipoDatoDB.STRING)
												{
													object posibleError = collection.AddItem(nuevo, Linea, Columna);
													if (posibleError != null)
													{
														if (posibleError.GetType() == typeof(ThrowError))
														{
															return posibleError;
														}
													}
												}
												else
												{
													return new ThrowError(Util.TipoThrow.Exception,
														"No se puede almacenar un valor string en un Collection tipo '" + collection.TipoDato.ToString() + "'",
														Linea, Columna);
												}
												break;
										}

									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}
									return null;
								case "get":
									if (llamada.Parametros.Count == 1)
									{
										object nuevo = llamada.Parametros.ElementAt(0).GetValor(ts);
										TipoOperacion t = llamada.Parametros.ElementAt(0).GetTipo(ts);
										if (t == TipoOperacion.Numero)
										{
											if (!nuevo.ToString().Contains("."))
											{
												//es entero
												int posicion = (int)nuevo;
												if (posicion >= 0 && posicion < collection.Count)
												{
													object nuevoValor = collection.ElementAt(posicion);
													Simbolo s = new Simbolo(sim.Nombre + "." + llaveFuncion, nuevoValor,
														new Util.TipoObjetoDB(collection.TipoDato.Tipo, collection.TipoDato.Nombre), 0, 0);
													return RetornarValorSobreVariable(s, ts);
												}
												else
												{
													return new ThrowError(Util.TipoThrow.IndexOutException,
														"El valor está fuera de rango",
														Linea, Columna);
												}
											}
											else
											{
												return new ThrowError(Util.TipoThrow.Exception,
													"No se puede acceder a una posición decimal en una lista",
													Linea, Columna);
											}
										}
										else
										{
											return new ThrowError(Util.TipoThrow.Exception,
												"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre un Collection'",
												Linea, Columna);
										}
									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}
								case "set":
									if (llamada.Parametros.Count == 2)
									{
										//PRIMER PARAMETRO = POSICION
										object nuevo = llamada.Parametros.ElementAt(0).GetValor(ts);
										TipoOperacion t = llamada.Parametros.ElementAt(0).GetTipo(ts);
										if (t == TipoOperacion.Numero)
										{
											if (!nuevo.ToString().Contains("."))
											{
												//es entero
												int posicion = (int)nuevo;
												if (posicion >= 0 && posicion < collection.Count)
												{
													//SEGUNDO PARAMETRO= VALOR
													object nuevoValor = llamada.Parametros.ElementAt(1).GetValor(ts);
													TipoOperacion t2 = llamada.Parametros.ElementAt(1).GetTipo(ts);
													switch (t2)
													{
														case TipoOperacion.Numero:
															if (collection.TipoDato.Tipo == TipoDatoDB.INT)
															{
																if (!nuevoValor.ToString().Contains("."))
																{
																	object posibleError = collection.SetItem(posicion, nuevoValor, Linea, Columna);
																	if (posibleError != null)
																	{
																		if (posibleError.GetType() == typeof(ThrowError))
																		{
																			return posibleError;
																		}
																	}
																}
																else
																{
																	return new ThrowError(Util.TipoThrow.Exception,
																		"No se puede almacenar un valor double en un Collection tipo int",
																		Linea, Columna);
																}
															}
															else if (collection.TipoDato.Tipo == TipoDatoDB.DOUBLE)
															{
																object posibleError = collection.SetItem(posicion, nuevoValor, Linea, Columna);
																if (posibleError != null)
																{
																	if (posibleError.GetType() == typeof(ThrowError))
																	{
																		return posibleError;
																	}
																}
															}
															else
															{
																return new ThrowError(Util.TipoThrow.Exception,
																		"No se puede almacenar un valor numerico en un Collection tipo '" + collection.TipoDato.ToString() + "'",
																		Linea, Columna);
															}
															break;
														case TipoOperacion.Booleano:
															if (collection.TipoDato.Tipo == TipoDatoDB.BOOLEAN)
															{
																object posibleError = collection.SetItem(posicion, nuevoValor, Linea, Columna);
																if (posibleError != null)
																{
																	if (posibleError.GetType() == typeof(ThrowError))
																	{
																		return posibleError;
																	}
																}
															}
															else
															{
																return new ThrowError(Util.TipoThrow.Exception,
																	"No se puede almacenar un valor booleano en un Collection tipo '" + collection.TipoDato.ToString() + "'",
																	Linea, Columna);
															}
															break;
														case TipoOperacion.Fecha:
															if (collection.TipoDato.Tipo == TipoDatoDB.DATE)
															{
																object posibleError = collection.SetItem(posicion, nuevoValor, Linea, Columna);
																if (posibleError != null)
																{
																	if (posibleError.GetType() == typeof(ThrowError))
																	{
																		return posibleError;
																	}
																}
															}
															else
															{
																return new ThrowError(Util.TipoThrow.Exception,
																	"No se puede almacenar un valor de fecha en un Collection tipo '" + collection.TipoDato.ToString() + "'",
																	Linea, Columna);
															}
															break;
														case TipoOperacion.Hora:
															if (collection.TipoDato.Tipo == TipoDatoDB.TIME)
															{
																object posibleError = collection.SetItem(posicion, nuevoValor, Linea, Columna);
																if (posibleError != null)
																{
																	if (posibleError.GetType() == typeof(ThrowError))
																	{
																		return posibleError;
																	}
																}
															}
															else
															{
																return new ThrowError(Util.TipoThrow.Exception,
																	"No se puede almacenar un valor de hora en un Collection tipo '" + collection.TipoDato.ToString() + "'",
																	Linea, Columna);
															}
															break;
														case TipoOperacion.Objeto:
															if (collection.TipoDato.Tipo == TipoDatoDB.OBJETO)
															{
																object posibleError = collection.SetItem(posicion, nuevoValor, Linea, Columna);
																if (posibleError != null)
																{
																	if (posibleError.GetType() == typeof(ThrowError))
																	{
																		return posibleError;
																	}
																}
															}
															else
															{
																return new ThrowError(Util.TipoThrow.Exception,
																	"No se puede almacenar un valor de objeto en un Collection tipo '" + collection.TipoDato.ToString() + "'",
																	Linea, Columna);
															}
															break;
														case TipoOperacion.String:
															if (collection.TipoDato.Tipo == TipoDatoDB.STRING)
															{
																object posibleError = collection.SetItem(posicion, nuevoValor, Linea, Columna);
																if (posibleError != null)
																{
																	if (posibleError.GetType() == typeof(ThrowError))
																	{
																		return posibleError;
																	}
																}
															}
															else
															{
																return new ThrowError(Util.TipoThrow.Exception,
																	"No se puede almacenar un valor string en un Collection tipo '" + collection.TipoDato.ToString() + "'",
																	Linea, Columna);
															}
															break;
													}
													//******
												}
												else
												{
													return new ThrowError(Util.TipoThrow.IndexOutException,
														"El valor está fuera de rango",
														Linea, Columna);
												}
											}
											else
											{
												return new ThrowError(Util.TipoThrow.Exception,
													"No se puede acceder a una posición decimal en una lista",
													Linea, Columna);
											}
										}
										else
										{
											return new ThrowError(Util.TipoThrow.Exception,
												"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre un Collection'",
												Linea, Columna);
										}
									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}
									return null;
								case "remove":
									if (llamada.Parametros.Count == 1)
									{
										object nuevo = llamada.Parametros.ElementAt(0).GetValor(ts);
										TipoOperacion t = llamada.Parametros.ElementAt(0).GetTipo(ts);
										if (t == TipoOperacion.Numero)
										{
											if (!nuevo.ToString().Contains("."))
											{
												//es entero
												int posicion = (int)nuevo;
												if (posicion >= 0 && posicion < collection.Count)
												{
													collection.EliminarItem(posicion);
												}
												else
												{
													return new ThrowError(Util.TipoThrow.IndexOutException,
														"El valor está fuera de rango",
														Linea, Columna);
												}
											}
											else
											{
												return new ThrowError(Util.TipoThrow.Exception,
													"No se puede acceder a una posición decimal en una lista",
													Linea, Columna);
											}
										}
										else
										{
											return new ThrowError(Util.TipoThrow.Exception,
												"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre un Collection'",
												Linea, Columna);
										}

									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}
									return null;
								case "size":
									if (llamada.Parametros.Count == 0)
									{
										Simbolo s = new Simbolo(sim.Nombre + "." + llaveFuncion, collection.Count,
											new Util.TipoObjetoDB(TipoDatoDB.INT, "int"), 0, 0);
										return RetornarValorSobreVariable(s, ts);
									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}
								case "clear":
									if (llamada.Parametros.Count == 0)
									{
										collection.Clear();
									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}
									return null;
								case "contains":
									if (llamada.Parametros.Count == 1)
									{
										object nuevo = llamada.Parametros.ElementAt(0).GetValor(ts);
										TipoOperacion t = llamada.Parametros.ElementAt(0).GetTipo(ts);
										bool respuesta = false;
										switch (t)
										{
											case TipoOperacion.Numero:
												if (collection.TipoDato.Tipo == TipoDatoDB.INT)
												{
													if (!nuevo.ToString().Contains("."))
													{
														respuesta = collection.Contains(nuevo);
													}
													else
													{
														return new ThrowError(Util.TipoThrow.Exception,
															"El Collection tipo " + collection.TipoDato.ToString() + " no contiene el valor '" + nuevo + "'",
															Linea, Columna);
													}
												}
												else if (collection.TipoDato.Tipo == TipoDatoDB.DOUBLE)
												{
													respuesta = collection.Contains(nuevo);
												}
												else
												{
													return new ThrowError(Util.TipoThrow.Exception,
															"El Collection tipo " + collection.TipoDato.ToString() + " no contiene el valor '" + nuevo + "'",
															Linea, Columna);
												}
												break;
											case TipoOperacion.Booleano:
												if (collection.TipoDato.Tipo == TipoDatoDB.BOOLEAN)
												{
													respuesta = collection.Contains(nuevo);
												}
												else
												{
													return new ThrowError(Util.TipoThrow.Exception,
														"El Collection tipo " + collection.TipoDato.ToString() + " no contiene el valor '" + nuevo + "'",
														Linea, Columna);
												}
												break;
											case TipoOperacion.Fecha:
												if (collection.TipoDato.Tipo == TipoDatoDB.DATE)
												{
													respuesta = collection.Contains(nuevo);
												}
												else
												{
													return new ThrowError(Util.TipoThrow.Exception,
														"El Collection tipo " + collection.TipoDato.ToString() + " no contiene el valor '" + nuevo + "'",
														Linea, Columna);
												}
												break;
											case TipoOperacion.Hora:
												if (collection.TipoDato.Tipo == TipoDatoDB.TIME)
												{
													respuesta = collection.Contains(nuevo);
												}
												else
												{
													return new ThrowError(Util.TipoThrow.Exception,
														"El Collection tipo " + collection.TipoDato.ToString() + " no contiene el valor '" + nuevo + "'",
														Linea, Columna);
												}
												break;
											case TipoOperacion.Objeto:
												if (collection.TipoDato.Tipo == TipoDatoDB.OBJETO)
												{
													respuesta = collection.Contains(nuevo);
												}
												else
												{
													return new ThrowError(Util.TipoThrow.Exception,
														"El Collection tipo " + collection.TipoDato.ToString() + " no contiene el valor '" + nuevo + "'",
														Linea, Columna);
												}
												break;
											case TipoOperacion.String:
												if (collection.TipoDato.Tipo == TipoDatoDB.STRING)
												{
													respuesta = collection.Contains(nuevo);
												}
												else
												{
													return new ThrowError(Util.TipoThrow.Exception,
														"El Collection tipo " + collection.TipoDato.ToString() + " no contiene el valor '" + nuevo + "'",
														Linea, Columna);
												}

												break;
											default:
												return new ThrowError(Util.TipoThrow.Exception,
										"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
										Linea, Columna);
										}
										//retornando el resultado
										Simbolo s = new Simbolo(sim.Nombre + "." + llaveFuncion, respuesta,
											new Util.TipoObjetoDB(TipoDatoDB.BOOLEAN, "boolean"), 0, 0);
										return RetornarValorSobreVariable(s, ts);
									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}
								default:
									return new ThrowError(Util.TipoThrow.Exception,
										"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
										Linea, Columna);

							}
							#endregion
						}
						else if (sim.TipoDato.Tipo == Util.TipoDatoDB.MAP_OBJETO || sim.TipoDato.Tipo == Util.TipoDatoDB.MAP_PRIMITIVO)
						{
							#region FuncionesNativasSobreMap
							LlamadaFuncion llamada = (LlamadaFuncion)valor.Value;
							CollectionMapCql collection = (CollectionMapCql)sim.Valor;
							string llaveFuncion = llamada.Nombre;
							switch (llaveFuncion.ToLower())
							{
								case "insert":
									if (llamada.Parametros.Count == 2)
									{
										object clave = llamada.Parametros.ElementAt(0).GetValor(ts);
										TipoOperacion tipoClave = llamada.Parametros.ElementAt(0).GetTipo(ts);
										if (Datos.Equivale(collection.TipoLlave, clave.ToString(), tipoClave))
										{

											object valorr = llamada.Parametros.ElementAt(1).GetValor(ts);
											TipoOperacion tipoValorr = llamada.Parametros.ElementAt(1).GetTipo(ts);
											if (Datos.Equivale(collection.TipoValor, valorr.ToString(), tipoValorr))
											{
												object respuesta = collection.AddItem(clave, valorr, Linea, Columna);
												return respuesta;
											}
											else
											{
												return new ThrowError(Util.TipoThrow.Exception,
												"El valor '" + valorr + "' no corresponde con el tipo '" + collection.TipoValor.ToString() + "' de clave del collection map",
												Linea, Columna);
											}
										}
										else
										{
											return new ThrowError(Util.TipoThrow.Exception,
											"El valor '" + clave + "' no corresponde con el tipo '" + collection.TipoLlave.ToString() + "' de clave del collection map",
											Linea, Columna);
										}
									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}
								case "get":
									if (llamada.Parametros.Count == 1)
									{
										object nuevo = llamada.Parametros.ElementAt(0).GetValor(ts);
										TipoOperacion t = llamada.Parametros.ElementAt(0).GetTipo(ts);
										if (Datos.Equivale(collection.TipoLlave,nuevo.ToString(),t))
										{
											object respuesta = collection.GetItem(nuevo,Linea,Columna);
											return respuesta;
										}
										else
										{
											return new ThrowError(Util.TipoThrow.Exception,
												"El valor '" + nuevo.ToString() + "' no corresponde con el tipo '" + collection.TipoValor.ToString() + "' de clave del collection map",
												Linea, Columna);
										}
									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}
								case "set":
									if (llamada.Parametros.Count == 2)
									{
										object clave = llamada.Parametros.ElementAt(0).GetValor(ts);
										TipoOperacion tipoClave = llamada.Parametros.ElementAt(0).GetTipo(ts);
										if (Datos.Equivale(collection.TipoLlave, clave.ToString(), tipoClave))
										{

											object valorr = llamada.Parametros.ElementAt(1).GetValor(ts);
											TipoOperacion tipoValorr = llamada.Parametros.ElementAt(1).GetTipo(ts);
											if (Datos.Equivale(collection.TipoValor, valorr.ToString(), tipoValorr))
											{
												object respuesta = collection.SetItem(clave, valorr, Linea, Columna);
												return respuesta;
											}
											else
											{
												return new ThrowError(Util.TipoThrow.Exception,
												"El valor '" + valorr + "' no corresponde con el tipo '" + collection.TipoValor.ToString() + "' de clave del collection map",
												Linea, Columna);
											}
										}
										else
										{
											return new ThrowError(Util.TipoThrow.Exception,
											"El valor '" + clave + "' no corresponde con el tipo '" + collection.TipoLlave.ToString() + "' de clave del collection map",
											Linea, Columna);
										}
									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}
								case "remove":
									if (llamada.Parametros.Count == 1)
									{
										object nuevo = llamada.Parametros.ElementAt(0).GetValor(ts);
										TipoOperacion t = llamada.Parametros.ElementAt(0).GetTipo(ts);
										if (Datos.Equivale(collection.TipoLlave, nuevo.ToString(), t))
										{
											object respuesta = collection.EliminarItem(nuevo, Linea, Columna);
											return respuesta;
										}
										else
										{
											return new ThrowError(Util.TipoThrow.Exception,
												"El valor '" + nuevo.ToString() + "' no corresponde con el tipo '" + collection.TipoValor.ToString() + "' de clave del collection map",
												Linea, Columna);
										}
									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}
								case "size":
									if (llamada.Parametros.Count == 0)
									{
										Simbolo s = new Simbolo(sim.Nombre + "." + llaveFuncion, collection.Count,
											new Util.TipoObjetoDB(TipoDatoDB.INT, "int"), 0, 0);
										return RetornarValorSobreVariable(s, ts);
									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}
								case "clear":
									if (llamada.Parametros.Count == 0)
									{
										collection.Clear();
									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}
									return null;
								case "contains":
									if (llamada.Parametros.Count == 1)
									{
										object nuevo = llamada.Parametros.ElementAt(0).GetValor(ts);
										TipoOperacion t = llamada.Parametros.ElementAt(0).GetTipo(ts);
										if (Datos.Equivale(collection.TipoLlave, nuevo.ToString(), t))
										{

											Simbolo s = new Simbolo(sim.Nombre + "." + llaveFuncion, collection.ContainsKey(nuevo),
												new Util.TipoObjetoDB(TipoDatoDB.BOOLEAN, "boolean"), 0, 0);
											return RetornarValorSobreVariable(s, ts);
										}
										else
										{
											return new ThrowError(Util.TipoThrow.Exception,
												"El valor '" + nuevo.ToString() + "' no corresponde con el tipo '" + collection.TipoValor.ToString() + "' de clave del collection map",
												Linea, Columna);
										}
									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}
								default:
									return new ThrowError(Util.TipoThrow.Exception,
										"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
										Linea, Columna);

							}
							#endregion
						}
						else
						{
							return new ThrowError(Util.TipoThrow.Exception,
								"No se puede aplicar la función '" + valor.Value.ToString() + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
								Linea, Columna);
						}
				}
			}

			return sim.Valor;
		}

		public override TipoOperacion GetTipo(TablaSimbolos ts)
		{
			return TipoOperacion.Nulo;
		}
	}

}
