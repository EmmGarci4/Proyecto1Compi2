using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class AccesoArreglo : Expresion
	{
		Expresion valor;
		string nombre;

		public AccesoArreglo(Expresion valor, string nombre, int linea, int columna) : base(linea, columna)
		{
			this.valor = valor;
			this.nombre = nombre;
		}

		public override string ToString()
		{
			return this.nombre + "[valor]";
		}

		public string Nombre { get => nombre; set => nombre = value; }

		internal Expresion Valor { get => valor; set => valor = value; }

		public override TipoOperacion GetTipo(TablaSimbolos ts)
		{
			if (ts.ExisteSimbolo(nombre))
			{
				Simbolo sim = ts.GetSimbolo(nombre);
				if (Datos.IsLista(sim.TipoDato.Tipo))
				{
					object indice = valor.GetValor(ts);
					TipoOperacion tipoIndice = valor.GetTipo(ts);
					//ES UNA LISTA O UN SET
					if (sim.Valor.GetType() == typeof(CollectionListCql))
					{
						CollectionListCql collection = (CollectionListCql)sim.Valor;
						return Datos.GetTipoDatoDB(collection.TipoDato.Tipo);
					}
					//ES UN MAP
					else if (sim.Valor.GetType() == typeof(CollectionMapCql))
					{
						//map
						CollectionMapCql collection = (CollectionMapCql)sim.Valor;
						return Datos.GetTipoDatoDB(collection.TipoValor.Tipo);
					}

				}
			}
			return TipoOperacion.Nulo;
		}

		public override object GetValor(TablaSimbolos ts)
		{
			if (ts.ExisteSimbolo(nombre))
			{
				Simbolo sim = ts.GetSimbolo(nombre);
				if (Datos.IsLista(sim.TipoDato.Tipo))
				{
					object indice = valor.GetValor(ts);
					TipoOperacion tipoIndice = valor.GetTipo(ts);
					//ES UNA LISTA O UN SET
					if (sim.Valor.GetType() == typeof(CollectionListCql))
					{
						CollectionListCql collection = (CollectionListCql)sim.Valor;
						if (tipoIndice == TipoOperacion.Numero)
						{
							if (!indice.ToString().Contains("."))
							{
								//es entero
								int posicion = (int)indice;
								if (posicion >= 0 && posicion < collection.Count)
								{
									object nuevoValor = collection.ElementAt(posicion);
									return nuevoValor;
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
								"No se puede acceder a un indice no númerico'",
								Linea, Columna);
						}
					}
					//ES UN MAP
					else if (sim.Valor.GetType() == typeof(CollectionMapCql))
					{
						//map
						CollectionMapCql collection = (CollectionMapCql)sim.Valor;
						if (Datos.Equivale(collection.TipoLlave, indice.ToString(), tipoIndice))
						{
							object respuesta = collection.GetItem(indice, Linea, Columna);
							if (respuesta == null)
							{
								return new ThrowError(Util.TipoThrow.Exception,
								"La llave '" + indice.ToString() + "' no existe en el map",
								Linea, Columna);
							}
							return respuesta;
						}
						else
						{
							return new ThrowError(Util.TipoThrow.Exception,
								"El valor '" + indice.ToString() + "' no corresponde con el tipo '" + collection.TipoValor.ToString() + "' de clave del collection map",
								Linea, Columna);
						}
					}
					else
					{
						return new ThrowError(Util.TipoThrow.Exception,
							"'" + nombre + "' no es un arreglo",
							Linea, Columna);
					}
				}
				else
				{
					return new ThrowError(Util.TipoThrow.Exception,
							"'" + nombre + "' no es un arreglo",
							Linea, Columna);
				}
			}
			else
			{
				return new ThrowError(Util.TipoThrow.Exception,
							"El arreglo '" + nombre + "' no existe",
							Linea, Columna);
			}
		}

		internal object SetValor(TablaSimbolos ts, object nuevoValor)
		{
			if (ts.ExisteSimbolo(nombre))
			{
				Simbolo sim = ts.GetSimbolo(nombre);
				if (Datos.IsLista(sim.TipoDato.Tipo))
				{
					object indice = valor.GetValor(ts);
					TipoOperacion tipoIndice = valor.GetTipo(ts);
					//ES UNA LISTA O UN SET
					if (sim.Valor.GetType() == typeof(CollectionListCql))
					{
						CollectionListCql collection = (CollectionListCql)sim.Valor;
						if (tipoIndice == TipoOperacion.Numero)
						{
							if (!indice.ToString().Contains("."))
							{
								//es entero
								int posicion = (int)indice;
								if (posicion >= 0 && posicion < collection.Count)
								{
									if (Datos.IsTipoCompatibleParaAsignar(collection.TipoDato, nuevoValor))
									{
										collection.SetItem(posicion, nuevoValor, Linea, Columna);
									}
									else
									{
										return new ThrowError(Util.TipoThrow.IndexOutException,
										"El valor no se puede asignar porque los tipos no son compatibles",
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
								"No se puede acceder a un indice no númerico'",
								Linea, Columna);
						}
					}
					//ES UN MAP
					else if (sim.Valor.GetType() == typeof(CollectionMapCql))
					{
						//map
						CollectionMapCql collection = (CollectionMapCql)sim.Valor;
						if (Datos.Equivale(collection.TipoLlave, indice.ToString(), tipoIndice))
						{
							object respuesta = null;
							if (Datos.IsTipoCompatibleParaAsignar(collection.TipoValor, nuevoValor))
							{
								respuesta = collection.SetItem(indice, nuevoValor, Linea, Columna);
							}
							else
							{
								return new ThrowError(Util.TipoThrow.IndexOutException,
								"El valor no se puede asignar porque los tipos no son compatibles",
								Linea, Columna);
							}
							if (respuesta == null)
							{
								return new ThrowError(Util.TipoThrow.Exception,
								"La llave '" + indice.ToString() + "' no existe en el map",
								Linea, Columna);
							}
							return respuesta;
						}
						else
						{
							return new ThrowError(Util.TipoThrow.Exception,
								"El valor '" + indice.ToString() + "' no corresponde con el tipo '" + collection.TipoValor.ToString() + "' de clave del collection map",
								Linea, Columna);
						}
					}
					else
					{
						return new ThrowError(Util.TipoThrow.Exception,
							"'" + nombre + "' no es un arreglo",
							Linea, Columna);
					}
				}
				else
				{
					return new ThrowError(Util.TipoThrow.Exception,
							"'" + nombre + "' no es un arreglo",
							Linea, Columna);
				}
			}
			else
			{
				return new ThrowError(Util.TipoThrow.Exception,
							"El arreglo '" + nombre + "' no existe",
							Linea, Columna);
			}
			return null;
		}
	}
}
