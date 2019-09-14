using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.Analisis;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class Acceso : Expresion
	{

		List<AccesoPar> accesos;
		private Queue<AccesoPar> objetos;
		private TipoOperacion tipo;

		public Acceso(List<AccesoPar> objetos, int linea, int columna) : base(linea, columna)
		{
			this.accesos = objetos;
			this.tipo = TipoOperacion.Nulo;
		}

		public Acceso(int linea, int columna) : base(linea, columna)
		{
			this.accesos = new List<AccesoPar>();
			this.tipo = TipoOperacion.Nulo;
		}

		public List<AccesoPar> Objetos { get => accesos; set => accesos = value; }

		public override object GetValor(TablaSimbolos ts, Sesion sesion)
		{
			LlenarCola();
			object respuesta = GetSimbolosApilados(ts, sesion);
			Stack<Simbolo> simbolos;
			if (respuesta.GetType() == typeof(ThrowError))
			{
				return respuesta;
			}
			else
			{
				simbolos = (Stack<Simbolo>)respuesta;
			}
			if (simbolos.Count > 0)
			{
				Simbolo s = simbolos.Pop();
				this.tipo = Datos.GetTipoDatoDB(Datos.GetTipoObjetoDB(s.Valor).Tipo);
				return s.Valor;
			}
			return null;
		}

		private void LlenarCola()
		{
			objetos = new Queue<AccesoPar>();
			foreach (AccesoPar acceso in this.accesos)
			{
				objetos.Enqueue(acceso);
			}
		}

		internal object Asignar(object nuevoValor, TipoObjetoDB tipoObjetoDB, TablaSimbolos ts, Sesion sesion)
		{
			LlenarCola();
			object respuesta = GetSimbolosApilados(ts, sesion);
			Stack<Simbolo> simbolos;
			if (respuesta.GetType() == typeof(ThrowError))
			{
				return respuesta;
			}
			else
			{
				simbolos = (Stack<Simbolo>)respuesta;
			}

			if (simbolos.Count > 1)
			{
				Simbolo ant = simbolos.Pop();
				if (ant.Nombre == "INDICELISTA")
				{
					Simbolo sig = simbolos.Pop();
					object indice = ant.Valor;
					if (Datos.IsLista(sig.TipoDato.Tipo))
					{
						if (sig.Valor.GetType()==typeof(CollectionListCql)) {
							CollectionListCql lista = (CollectionListCql)sig.Valor;
							object res = lista.SetItem((int)indice, nuevoValor, Linea, Columna);
							if (res != null)
							{
								if (res.GetType() == typeof(ThrowError))
								{
									return res;
								}
							}
						}
						else {
							CollectionMapCql lista = (CollectionMapCql)sig.Valor;
							object res = lista.SetItem((int)indice, nuevoValor, Linea, Columna);
							if (res != null)
							{
								if (res.GetType() == typeof(ThrowError))
								{
									return res;
								}
							}
						}
					}
					else
					{
						//error
						return new ThrowError(Util.TipoThrow.Exception,
									"'"+sig.Nombre+"' no es una lista ",
									Linea, Columna);
					}
				}
				else {
					Simbolo s = simbolos.Pop();
					if (s.TipoDato.Tipo == TipoDatoDB.OBJETO)
					{
						Objeto objeto = (Objeto)s.Valor;
						if (objeto.Atributos.ContainsKey(ant.Nombre))
						{
							if (Datos.IsTipoCompatibleParaAsignar(objeto.Plantilla.Atributos[ant.Nombre], nuevoValor))
							{
								object nuevoDato = Datos.CasteoImplicito(s.TipoDato, nuevoValor, ts, sesion, Linea, Columna);
								if (nuevoDato != null)
								{
									if (nuevoDato.GetType() == typeof(ThrowError))
									{
										return nuevoDato;
									}
									objeto.Atributos[ant.Nombre] = nuevoDato;
								}
							}
							else
							{
								return new ThrowError(Util.TipoThrow.Exception,
									"No se puede asignar el valor",
									Linea, Columna);
							}
						}
						else
						{
							return new ThrowError(Util.TipoThrow.Exception,
							"No existe el atributo '" + ant.Nombre + "' en '" + s.Nombre + "'",
							Linea, Columna);
						}
					}
				}
			}
			else if(simbolos.Count == 1)
			{
				Simbolo s = simbolos.Pop();
				if (Datos.IsTipoCompatibleParaAsignar(s.TipoDato, nuevoValor))
				{
					if (s.TipoDato.Tipo != TipoDatoDB.COUNTER)
					{
						object nuevoDato = Datos.CasteoImplicito(s.TipoDato, nuevoValor, ts, sesion, Linea, Columna);
						if (nuevoDato != null)
						{
							if (nuevoDato.GetType() == typeof(ThrowError))
							{
								return nuevoDato;
							}
							s.Valor = nuevoDato;
						}
					}
					else {
						return new ThrowError(Util.TipoThrow.Exception,
							"No se puede actualizar una columna tipo counter",
							Linea, Columna);
					}
				}
				else
				{
					return new ThrowError(Util.TipoThrow.Exception,
						"No se puede asignar el valor",
						Linea, Columna);
				}
			}

			return null;
		}

		private object GetSimbolosApilados(TablaSimbolos ts, Sesion sesion)
		{
			Stack<Simbolo> simbolosApilados = new Stack<Simbolo>();
			if (objetos.Count > 0)
			{
				AccesoPar valor = objetos.Dequeue();
				switch (valor.Tipo)
				{
					case TipoAcceso.AccesoArreglo:
						AccesoArreglo acceso = (AccesoArreglo)((AccesoPar)valor).Value;
						if (ts.ExisteSimbolo(acceso.Nombre))
						{
							Simbolo sim = ts.GetSimbolo(acceso.Nombre);
							if (Datos.IsLista(sim.TipoDato.Tipo))
							{
								if (sim.TipoDato.Tipo == TipoDatoDB.MAP_OBJETO || sim.TipoDato.Tipo == TipoDatoDB.MAP_PRIMITIVO)
								{
									CollectionMapCql lista = (CollectionMapCql)sim.Valor;
									object indiceexp = acceso.Valor.GetValor(ts, sesion);
									if (indiceexp != null)
									{
										if (indiceexp.GetType() == typeof(ThrowError))
										{
											return indiceexp;
										}
										TipoObjetoDB tipoIndice = Datos.GetTipoObjetoDB(indiceexp);
										if (tipoIndice.Equals(lista.TipoLlave))
										{
											if (lista.ContainsKey(indiceexp))
											{
												Simbolo simb = new Simbolo("INDICELISTA", indiceexp, tipoIndice, Linea, Columna);
												simbolosApilados.Push(sim);
												simbolosApilados.Push(simb);
												object respuesta = GetSimbolosApilados(simbolosApilados, sim, ts, sesion);
												return respuesta;
											}
											else
											{
												////error
												//return new ThrowError(Util.TipoThrow.IndexOutException,
												//"El indice debe ser del tipo '" + lista.TipoLlave.ToString() + "'",
												//Linea, Columna);
											}
										}
										else
										{
											//error
											return new ThrowError(Util.TipoThrow.IndexOutException,
												"El indice debe ser entero en una lista",
												Linea, Columna);
										}
									}
								}
								else
								{
									CollectionListCql lista = (CollectionListCql)sim.Valor;
									object indiceexp = acceso.Valor.GetValor(ts, sesion);
									if (indiceexp != null)
									{
										if (indiceexp.GetType() == typeof(ThrowError))
										{
											return indiceexp;
										}
										TipoObjetoDB tipoIndice = Datos.GetTipoObjetoDB(indiceexp);
										if (tipoIndice.Tipo == TipoDatoDB.INT)
										{
											int indice = (int)indiceexp;
											if (indice >= 0 && indice < lista.Count)
											{

												Simbolo simb = new Simbolo("INDICELISTA", indiceexp, tipoIndice, Linea, Columna);
												simbolosApilados.Push(sim);
												simbolosApilados.Push(simb);
												object respuesta = GetSimbolosApilados(simbolosApilados, sim, ts, sesion);
												return respuesta;
											}
											else
											{
												//error
												return new ThrowError(Util.TipoThrow.IndexOutException,
												"El indice debe ser mayor a cero y menor que el tamaño de la lista",
												Linea, Columna);
											}
										}
										else
										{
											//error
											return new ThrowError(Util.TipoThrow.IndexOutException,
												"El indice debe ser entero en una lista",
												Linea, Columna);
										}
									}

								}
							}
							else
							{
								return new ThrowError(Util.TipoThrow.Exception,
									"El campo '" + valor.Value.ToString() + "' no es una lista",
									Linea, Columna);
							}
						}
						else
						{
							return new ThrowError(Util.TipoThrow.Exception,
								"El campo '" + valor.Value.ToString() + "' no existe",
								Linea, Columna);
						}
						break;
					case TipoAcceso.Campo:
						//tablas Y COLUMNAS
						if (ts.ExisteSimbolo(valor.Value.ToString()))
						{
							Simbolo sim = ts.GetSimbolo(valor.Value.ToString());
							simbolosApilados.Push(sim);
							object respuesta = GetSimbolosApilados(simbolosApilados, sim, ts, sesion);
							return respuesta;
						}
						else
						{
							return new ThrowError(Util.TipoThrow.Exception,
								"El campo '" + valor.Value.ToString() + "' no existe",
								Linea, Columna);
						}
					//enviar sobre campo
					case TipoAcceso.LlamadaFuncion:
						//ejecutar llamada de funcion y retornar el valor 
						LlamadaFuncion llamada = (LlamadaFuncion)valor.Value;
						object res = llamada.Ejecutar(ts, sesion);
						if (res != null)
						{
							if (res.GetType() == typeof(ThrowError))
							{
								return res;
							}
						}
						Simbolo s = new Simbolo(llamada.getLlave(ts, sesion), res, Datos.GetTipoObjetoDB(res), Linea, Columna);
						simbolosApilados.Push(s);
						return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
					case TipoAcceso.Variable:
						if (ts.ExisteSimbolo(valor.Value.ToString()))
						{
							Simbolo sim = ts.GetSimbolo(valor.Value.ToString());
							simbolosApilados.Push(sim);
							object respuesta = GetSimbolosApilados(simbolosApilados, sim, ts, sesion);
							return respuesta;
						}
						else
						{
							return new ThrowError(Util.TipoThrow.Exception,
								"La variable '" + valor.Value.ToString() + "' no existe",
								Linea, Columna);
						}
				}
			}

			return simbolosApilados;
		}

		private object GetSimbolosApilados(Stack<Simbolo> simbolosApilados, Simbolo sim, TablaSimbolos ts, Sesion sesion)
		{
			if (objetos.Count > 0)
			{
				if (sim.Valor != null)
				{

					AccesoPar valor = objetos.Dequeue();
					switch (valor.Tipo)
					{
						case TipoAcceso.AccesoArreglo:
						case TipoAcceso.Campo:
							if (sim.TipoDato.Tipo != TipoDatoDB.NULO)
							{
								if (sim.Valor.ToString().Equals("null"))
								{
									return new ThrowError(Util.TipoThrow.NullPointerException,
												"la variable '" + sim.Nombre + "' no se ha inicializado",
												Linea, Columna);
								}
							}
							if (sim.TipoDato.Tipo == Util.TipoDatoDB.OBJETO)
							{
								Objeto objeto = (Objeto)sim.Valor;
								if (objeto.Atributos.ContainsKey(valor.Value.ToString()))
								{
									TipoObjetoDB tipo = objeto.Plantilla.Atributos[valor.Value.ToString()];
									object val = objeto.Atributos[valor.Value.ToString()];
									Simbolo s = new Simbolo(valor.Value.ToString(),
											val, tipo, 0, 0);
									simbolosApilados.Push(s);
									return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
								}
								else
								{
									return new ThrowError(Util.TipoThrow.Exception,
										"No existe el atributo '" + valor.Value.ToString() + "' en el objeto '" + sim.TipoDato.ToString() + "'",
										Linea, Columna);
								}
							}
							else
							{
								return new ThrowError(Util.TipoThrow.Exception,
									"No se puede acceder a un valor en '" + sim.Nombre + "' por que no es un objeto",
									Linea, Columna);
							}
						case TipoAcceso.LlamadaFuncion:
							if (sim.TipoDato.Tipo != TipoDatoDB.NULO)
							{
								if (sim.Valor.ToString().Equals("null"))
								{
									return new ThrowError(Util.TipoThrow.NullPointerException,
												"la variable '" + sim.Nombre + "' no se ha inicializado",
												Linea, Columna);
								}
							}
							if (sim.TipoDato.Tipo == Util.TipoDatoDB.STRING)
							{
								#region FuncionesNativasSobreCadenas
								string VALORFINAL = sim.Valor.ToString();
								LlamadaFuncion llamada = (LlamadaFuncion)valor.Value;
								string llaveFuncion = llamada.getLlave(ts, sesion);
								switch (llaveFuncion.ToLower())
								{
									case "length()":
										Simbolo s = new Simbolo(llaveFuncion, VALORFINAL.Length, new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
										simbolosApilados.Push(s);
										return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
									case "touppercase()":
										s = new Simbolo(llaveFuncion, VALORFINAL.ToUpper(), new Util.TipoObjetoDB(Util.TipoDatoDB.STRING, "string"), 0, 0);
										simbolosApilados.Push(s);
										return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
									case "tolowercase()":
										s = new Simbolo(llaveFuncion, VALORFINAL.ToLower(), new Util.TipoObjetoDB(Util.TipoDatoDB.STRING, "string"), 0, 0);
										simbolosApilados.Push(s);
										return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
									case "startswith(string)":
										s = new Simbolo(llaveFuncion, VALORFINAL.StartsWith(llamada.Parametros.ElementAt(0).GetValor(ts, sesion).ToString()),
											new Util.TipoObjetoDB(Util.TipoDatoDB.BOOLEAN, "boolean"), 0, 0);
										simbolosApilados.Push(s);
										return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
									case "endswith(string)":
										s = new Simbolo(llaveFuncion, VALORFINAL.EndsWith(llamada.Parametros.ElementAt(0).GetValor(ts, sesion).ToString()),
											new Util.TipoObjetoDB(Util.TipoDatoDB.BOOLEAN, "boolean"), 0, 0);
										simbolosApilados.Push(s);
										return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
									case "substring(int,int)":
										try
										{
											s = new Simbolo(llaveFuncion,
											VALORFINAL.Substring(int.Parse(llamada.Parametros.ElementAt(0).GetValor(ts, sesion).ToString()), int.Parse(llamada.Parametros.ElementAt(1).GetValor(ts, sesion).ToString())),
											new Util.TipoObjetoDB(Util.TipoDatoDB.STRING, "string"), 0, 0);
										}
										catch (ArgumentOutOfRangeException)
										{
											return new ThrowError(Util.TipoThrow.Exception,
											"Los valores ingresados en la función '" + llaveFuncion + "' están fuera de los límites",
											Linea, Columna);
										}
										simbolosApilados.Push(s);
										return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
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
								string llaveFuncion = llamada.getLlave(ts, sesion);
								switch (llaveFuncion.ToLower())
								{
									case "getyear()":
										Simbolo s = new Simbolo(llaveFuncion, hora.Dato.Year,
											new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
										simbolosApilados.Push(s);
										return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
									case "getmonth()":
										s = new Simbolo(llaveFuncion, hora.Dato.Month,
										   new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
										simbolosApilados.Push(s);
										return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
									case "getday()":
										s = new Simbolo(llaveFuncion, hora.Dato.Day,
										   new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
										simbolosApilados.Push(s);
										return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
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
								string llaveFuncion = llamada.getLlave(ts, sesion);
								switch (llaveFuncion.ToLower())
								{
									case "gethour()":
										Simbolo s = new Simbolo(llaveFuncion, hora.Dato.Hour,
											new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
										simbolosApilados.Push(s);
										return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
									case "getminuts()":
										s = new Simbolo(llaveFuncion, hora.Dato.Minute,
										   new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
										simbolosApilados.Push(s);
										return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
									case "getseconds()":
										s = new Simbolo(llaveFuncion, hora.Dato.Second,
										   new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
										simbolosApilados.Push(s);
										return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
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
											object nuevo = llamada.Parametros.ElementAt(0).GetValor(ts, sesion);
											TipoOperacion t = llamada.Parametros.ElementAt(0).GetTipo(ts, sesion);
											if (Datos.IsTipoCompatibleParaAsignar(Datos.GetTipoObjetoDBPorCadena(collection.TipoDato.Nombre), nuevo))
											{
												object nuevoDato = Datos.CasteoImplicito(collection.TipoDato, nuevo, ts, sesion, Linea, Columna);
												if (nuevoDato != null)
												{
													if (nuevoDato.GetType() == typeof(ThrowError))
													{
														return nuevoDato;
													}

													object posibleError = collection.AddItem(nuevoDato, Linea, Columna);
													if (posibleError != null)
													{
														if (posibleError.GetType() == typeof(ThrowError))
														{
															return posibleError;
														}
													}
												}
											}
											else
											{
												return new ThrowError(Util.TipoThrow.Exception,
													"No se puede almacenar un valor " + Datos.GetTipoObjetoDB(nuevo) + " en un Collection tipo " + collection.TipoDato.ToString(),
													Linea, Columna);
											}
										}
										else
										{
											return new ThrowError(Util.TipoThrow.Exception,
												"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
												Linea, Columna);
										}
										break;
									case "get":
										if (llamada.Parametros.Count == 1)
										{
											object nuevo = llamada.Parametros.ElementAt(0).GetValor(ts, sesion);
											TipoOperacion t = llamada.Parametros.ElementAt(0).GetTipo(ts, sesion);
											if (t == TipoOperacion.Numero)
											{
												if (!nuevo.ToString().Contains("."))
												{
													//es entero
													int posicion = (int)nuevo;
													if (posicion >= 0 && posicion < collection.Count)
													{
														object nuevoValor = collection.ElementAt(posicion);
														if (nuevoValor != null)
														{
															if (nuevoValor.GetType() == typeof(ThrowError))
															{
																return nuevoValor;
															}
														}
														Simbolo s = new Simbolo(llaveFuncion, nuevoValor,
															Datos.GetTipoObjetoDB(nuevoValor), 0, 0);
														simbolosApilados.Push(s);
														return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
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
													"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre un Collection'",
													Linea, Columna);
											}
										}
										else
										{
											return new ThrowError(Util.TipoThrow.Exception,
												"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
												Linea, Columna);
										}
									case "set":
										if (llamada.Parametros.Count == 2)
										{
											//PRIMER PARAMETRO = POSICION
											object nuevo = llamada.Parametros.ElementAt(0).GetValor(ts, sesion);
											TipoOperacion t = llamada.Parametros.ElementAt(0).GetTipo(ts, sesion);
											if (t == TipoOperacion.Numero)
											{
												if (!nuevo.ToString().Contains("."))
												{
													//es entero
													int posicion = (int)nuevo;
													if (posicion >= 0 && posicion < collection.Count)
													{
														//SEGUNDO PARAMETRO= VALOR
														object nuevoValor = llamada.Parametros.ElementAt(1).GetValor(ts, sesion);
														TipoOperacion t2 = llamada.Parametros.ElementAt(1).GetTipo(ts, sesion);

														if (Datos.IsTipoCompatibleParaAsignar(collection.TipoDato, nuevoValor))
														{
															object nuevoDato = Datos.CasteoImplicito(collection.TipoDato, nuevoValor, ts, sesion, Linea, Columna);
															if (nuevoDato != null)
															{
																if (nuevoDato.GetType() == typeof(ThrowError))
																{
																	return nuevoDato;
																}
																object posibleError = collection.SetItem(posicion, nuevoDato, Linea, Columna);
																if (posibleError != null)
																{
																	if (posibleError.GetType() == typeof(ThrowError))
																	{
																		return posibleError;
																	}
																}
															}
														}
														else
														{
															return new ThrowError(Util.TipoThrow.Exception,
																"No se puede almacenar un valor " + Datos.GetTipoObjetoDB(nuevoValor) + " en un Collection tipo " + collection.TipoDato.ToString(),
																Linea, Columna);
														}
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
													"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre un Collection'",
													Linea, Columna);
											}
										}
										else
										{
											return new ThrowError(Util.TipoThrow.Exception,
												"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
												Linea, Columna);
										}
										break;
									case "remove":
										if (llamada.Parametros.Count == 1)
										{
											object nuevo = llamada.Parametros.ElementAt(0).GetValor(ts, sesion);
											TipoOperacion t = llamada.Parametros.ElementAt(0).GetTipo(ts, sesion);
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
													"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre un Collection'",
													Linea, Columna);
											}

										}
										else
										{
											return new ThrowError(Util.TipoThrow.Exception,
												"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
												Linea, Columna);
										}
										break;
									case "size":
										if (llamada.Parametros.Count == 0)
										{
											Simbolo s = new Simbolo(llaveFuncion, collection.Count,
												new Util.TipoObjetoDB(TipoDatoDB.INT, "int"), 0, 0);
											simbolosApilados.Push(s);
											return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
										}
										else
										{
											return new ThrowError(Util.TipoThrow.Exception,
												"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
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
												"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
												Linea, Columna);
										}
										break;
									case "contains":
										if (llamada.Parametros.Count == 1)
										{
											object nuevo = llamada.Parametros.ElementAt(0).GetValor(ts, sesion);
											TipoOperacion t = llamada.Parametros.ElementAt(0).GetTipo(ts, sesion);
											if (Datos.IsTipoCompatibleParaAsignar(Datos.GetTipoObjetoDBPorCadena(collection.TipoDato.Nombre), nuevo))
											{
												object nuevoDato = Datos.CasteoImplicito(collection.TipoDato, nuevo, ts, sesion, Linea, Columna);
												if (nuevoDato != null)
												{
													if (nuevoDato.GetType() == typeof(ThrowError))
													{
														return nuevoDato;
													}
													object val = collection.Contains(nuevoDato);
													Simbolo s = new Simbolo(llaveFuncion, val,
															Datos.GetTipoObjetoDB(val), 0, 0);
													simbolosApilados.Push(s);
													return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
												}
											}
											else
											{
												return new ThrowError(Util.TipoThrow.Exception,
													"No se puede almacenar un valor " + Datos.GetTipoObjetoDB(nuevo) + " en un Collection tipo " + collection.TipoDato.ToString(),
													Linea, Columna);
											}
										}
										else
										{
											return new ThrowError(Util.TipoThrow.Exception,
												"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
												Linea, Columna);
										}
										break;
									default:
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
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
											object clave = llamada.Parametros.ElementAt(0).GetValor(ts, sesion);
											TipoOperacion tipoClave = llamada.Parametros.ElementAt(0).GetTipo(ts, sesion);
											if (Datos.Equivale(collection.TipoLlave, clave.ToString(), tipoClave))
											{

												object nuevo = llamada.Parametros.ElementAt(1).GetValor(ts, sesion);
												TipoOperacion tipoValorr = llamada.Parametros.ElementAt(1).GetTipo(ts, sesion);

												if (Datos.IsTipoCompatibleParaAsignar(collection.TipoValor, nuevo))
												{
													object nuevoDato = Datos.CasteoImplicito(collection.TipoValor, nuevo, ts, sesion, Linea, Columna);
													if (nuevoDato != null)
													{
														if (nuevoDato.GetType() == typeof(ThrowError))
														{
															return nuevoDato;
														}

														object posibleError = collection.AddItem(clave, nuevoDato, Linea, Columna);
														if (posibleError != null)
														{
															if (posibleError.GetType() == typeof(ThrowError))
															{
																return posibleError;
															}
														}
													}
												}
												else
												{
													return new ThrowError(Util.TipoThrow.Exception,
														"No se puede almacenar un valor " + Datos.GetTipoObjetoDB(nuevo) + " en un valor tipo " + collection.TipoValor.ToString(),
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
												"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
												Linea, Columna);
										}
										break;
									case "get":
										if (llamada.Parametros.Count == 1)
										{
											object nuevo = llamada.Parametros.ElementAt(0).GetValor(ts, sesion);
											TipoOperacion t = llamada.Parametros.ElementAt(0).GetTipo(ts, sesion);
											if (Datos.Equivale(collection.TipoLlave, nuevo.ToString(), t))
											{
												object nuevoValor = collection.GetItem(nuevo, Linea, Columna);
												if (nuevoValor != null)
												{
													if (nuevoValor.GetType() == typeof(ThrowError))
													{
														return nuevoValor;
													}
												}
												Simbolo s = new Simbolo(llaveFuncion, nuevoValor,
															Datos.GetTipoObjetoDB(nuevoValor), 0, 0);
												simbolosApilados.Push(s);
												return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
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
												"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
												Linea, Columna);
										}
									case "set":
										if (llamada.Parametros.Count == 2)
										{
											object clave = llamada.Parametros.ElementAt(0).GetValor(ts, sesion);
											TipoOperacion tipoClave = llamada.Parametros.ElementAt(0).GetTipo(ts, sesion);
											if (Datos.Equivale(collection.TipoLlave, clave.ToString(), tipoClave))
											{

												object valorr = llamada.Parametros.ElementAt(1).GetValor(ts, sesion);
												TipoOperacion tipoValorr = llamada.Parametros.ElementAt(1).GetTipo(ts, sesion);
												if (Datos.IsTipoCompatibleParaAsignar(collection.TipoValor, valorr))
												{
													object nuevoDato = Datos.CasteoImplicito(collection.TipoValor, valorr, ts, sesion, Linea, Columna);
													if (nuevoDato != null)
													{
														if (nuevoDato.GetType() == typeof(ThrowError))
														{
															return nuevoDato;
														}

														object posibleError = collection.AddItem(clave, nuevoDato, Linea, Columna);
														if (posibleError != null)
														{
															if (posibleError.GetType() == typeof(ThrowError))
															{
																return posibleError;
															}
														}
													}
												}
												else
												{
													return new ThrowError(Util.TipoThrow.Exception,
														"No se puede almacenar un valor " + Datos.GetTipoObjetoDB(valorr) + " en un valor tipo " + collection.TipoValor.ToString(),
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
												"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
												Linea, Columna);
										}
										break;
									case "remove":
										if (llamada.Parametros.Count == 1)
										{
											object nuevo = llamada.Parametros.ElementAt(0).GetValor(ts, sesion);
											TipoOperacion t = llamada.Parametros.ElementAt(0).GetTipo(ts, sesion);
											if (Datos.Equivale(collection.TipoLlave, nuevo.ToString(), t))
											{
												object respuesta = collection.EliminarItem(nuevo, Linea, Columna);
												if (respuesta != null) return respuesta;
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
												"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
												Linea, Columna);
										}
										break;
									case "size":
										if (llamada.Parametros.Count == 0)
										{
											Simbolo s = new Simbolo(llaveFuncion, collection.Count,
												new Util.TipoObjetoDB(TipoDatoDB.INT, "int"), 0, 0);
											simbolosApilados.Push(s);
											return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
										}
										else
										{
											return new ThrowError(Util.TipoThrow.Exception,
												"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
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
												"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
												Linea, Columna);
										}
										break;
									case "contains":
										if (llamada.Parametros.Count == 1)
										{
											object nuevo = llamada.Parametros.ElementAt(0).GetValor(ts, sesion);
											TipoOperacion t = llamada.Parametros.ElementAt(0).GetTipo(ts, sesion);
											if (Datos.Equivale(collection.TipoLlave, nuevo.ToString(), t))
											{

												Simbolo s = new Simbolo(llaveFuncion, collection.ContainsKey(nuevo),
													new Util.TipoObjetoDB(TipoDatoDB.BOOLEAN, "boolean"), 0, 0);
												simbolosApilados.Push(s);
												return GetSimbolosApilados(simbolosApilados, s, ts, sesion);
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
												"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
												Linea, Columna);
										}
									default:
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);

								}
								#endregion
							}
							else
							{
								return new ThrowError(Util.TipoThrow.Exception,
									"No se puede aplicar la función '" + ((LlamadaFuncion)valor.Value).getLlave(ts, sesion) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
									Linea, Columna);
							}
							break;
					}
				}
				else
				{
					return new ThrowError(Util.TipoThrow.NullPointerException,
										 "la variable '" + sim.Nombre + "' no se ha inicializado",
										 Linea, Columna);
				}

			}
			return simbolosApilados;
		}

		public override TipoOperacion GetTipo(TablaSimbolos ts, Sesion sesion)
		{
			return this.tipo;
		}

	}

}
