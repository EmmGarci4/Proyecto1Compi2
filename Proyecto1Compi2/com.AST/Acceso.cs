using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
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
				//acceso a arreglo
				//enviar sobre variable o campo
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
					case TipoAcceso.Campo:
						if (sim.TipoDato.Tipo == Util.TipoDatoDB.OBJETO)
						{
							//OBTENER OBJETO Y ACCEDER A PROPIEDADES
						}
						else
						{
							return new ThrowError(Util.TipoThrow.Exception,
								"No se puede acceder a un valor en '" + sim.Nombre + "' por que no contiene la propiedad '" + valor.ToString() + "'",
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
							DateTime hora = DateTime.Parse(sim.Valor.ToString().Replace("'", string.Empty));
							LlamadaFuncion llamada = (LlamadaFuncion)valor.Value;
							string llaveFuncion = llamada.getLlave(ts);
							switch (llaveFuncion.ToLower())
							{
								case "getyear()":
									Simbolo s = new Simbolo(sim.Nombre + "." + llaveFuncion, hora.Year,
										new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
									return RetornarValorSobreVariable(s, ts);
								case "getmonth()":
									s = new Simbolo(sim.Nombre + "." + llaveFuncion, hora.Month,
									   new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
									return RetornarValorSobreVariable(s, ts);
								case "getday()":
									s = new Simbolo(sim.Nombre + "." + llaveFuncion, hora.Day,
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
							DateTime hora = DateTime.Parse(sim.Valor.ToString().Replace("'", string.Empty));
							LlamadaFuncion llamada = (LlamadaFuncion)valor.Value;
							string llaveFuncion = llamada.getLlave(ts);
							switch (llaveFuncion.ToLower())
							{
								case "gethour()":
									Simbolo s = new Simbolo(sim.Nombre + "." + llaveFuncion, hora.Hour,
										new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
									return RetornarValorSobreVariable(s, ts);
								case "getminuts()":
									s = new Simbolo(sim.Nombre + "." + llaveFuncion, hora.Minute,
									   new Util.TipoObjetoDB(Util.TipoDatoDB.INT, "int"), 0, 0);
									return RetornarValorSobreVariable(s, ts);
								case "getseconds()":
									s = new Simbolo(sim.Nombre + "." + llaveFuncion, hora.Second,
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
						  sim.TipoDato.Tipo == Util.TipoDatoDB.SET_OBJETO ||sim.TipoDato.Tipo == Util.TipoDatoDB.SET_PRIMITIVO)
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


									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}

									return null;
								case "set":
									if (llamada.Parametros.Count == 1)
									{


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


									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}

									return null;
								case "size":
									if (llamada.Parametros.Count == 1)
									{


									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}

									return null;
								case "clear":
									if (llamada.Parametros.Count == 1)
									{


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


									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
											"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
											Linea, Columna);
									}

									return null;
								default:
									return new ThrowError(Util.TipoThrow.Exception,
										"No se puede aplicar la función '" + llamada.getLlave(ts) + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
										Linea, Columna);

							}
							#endregion
						}
						else if (sim.TipoDato.Tipo == Util.TipoDatoDB.MAP_OBJETO ||sim.TipoDato.Tipo == Util.TipoDatoDB.MAP_PRIMITIVO)
						{
							return null;
						}
						else
						{
							return new ThrowError(Util.TipoThrow.Exception,
								"No se puede aplicar la función '" + valor.ToString() + "' sobre el valor tipo '" + sim.TipoDato.ToString() + "'",
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
