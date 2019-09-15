using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class Operacion : Expresion
	{
		TipoOperacion tipoOp;
		private readonly object Valor;
		Expresion izquierda;
		Expresion derecha;
		/*
		 * 
		 *constructor para operaciones binarias
		 * EXPRESION operador EXPRESION
		 * sumas,restas,multiplicaciones,divisiones, y comparaciones
		 * 
		 */
		public Operacion(Expresion operadorIzq, Expresion operadorDer, TipoOperacion tipo, int linea, int columna) : base(linea, columna)
		{
			this.tipoOp = tipo;
			this.izquierda = operadorIzq;
			this.derecha = operadorDer;
			this.Valor = null;
		}

		/*
		 * 
		 *constructor para operaciones con un solo signo
		 * operador EXPRESION
		 * negaciones
		 *  
		 */
		public Operacion(Expresion operadorIzq, TipoOperacion tipo, int linea, int columna) : base(linea, columna)
		{
			this.tipoOp = tipo;
			this.izquierda = operadorIzq;
			this.derecha = null;
			this.Valor = null;
		}

		/*
		 * 
		 *constructor para valores
		 * EXPRESION
		 * numeros, identificadores, cadenas o booleanos
		 * 
		 */
		public Operacion(object valor, TipoOperacion tipo, int linea, int columna) : base(linea, columna)
		{
			this.tipoOp = tipo;
			this.Valor = valor;
			this.izquierda = null;
			this.derecha = null;
		}

		public TipoOperacion TipoOp { get => tipoOp; set => tipoOp = value; }
		internal Expresion Izquierda { get => izquierda; set => izquierda = value; }
		internal Expresion Derecha { get => derecha; set => derecha = value; }

		public override TipoOperacion GetTipo(TablaSimbolos ts, Sesion sesion)
		{
			switch (tipoOp)
			{
				case TipoOperacion.Numero:
				case TipoOperacion.String:
				case TipoOperacion.Booleano:
				case TipoOperacion.Caracter:
				case TipoOperacion.Fecha:
				case TipoOperacion.Hora:
				case TipoOperacion.ListaDatos:
				case TipoOperacion.NuevaInstancia:
					return this.tipoOp;
				case TipoOperacion.Identificador:
					if (ts.ExisteSimbolo(this.Valor.ToString()))
					{
						Simbolo s = ts.GetSimbolo(this.Valor.ToString());
						return Datos.GetTipoDatoDB(s.TipoDato.Tipo);
					}
					break;
				case TipoOperacion.Suma:
					//BOOLEANO-CADENA
					if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.String))
					{
						return TipoOperacion.String;
					}
					//NUMERO-NUMERO
					if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					//NUMERO-CADENA
					if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.String))
					{
						return TipoOperacion.String;
					}
					//CADENA-BOOLEANO
					if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Booleano))
					{
						return TipoOperacion.String;
					}
					//CADENA-NUMERO
					if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.String;
					}
					//CADENA-FECHA
					if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Fecha))
					{
						return TipoOperacion.String;
					}
					//CADENA-HORA
					if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Hora))
					{
						return TipoOperacion.String;
					}
					//CADENA-CADENA
					if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.String))
					{
						return TipoOperacion.String;
					}
					//FECHA-CADENA
					if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.String))
					{
						return TipoOperacion.String;
					}
					//HORA-CADENA
					if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Hora) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.String))
					{
						return TipoOperacion.String;
					}
					break;
				case TipoOperacion.Resta:
					//NUMERO-NUMERO
					if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					break;
				case TipoOperacion.Division:
					//NUMERO-NUMERO
					if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					break;
				case TipoOperacion.Multiplicacion:
					//NUMERO-NUMERO
					if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					break;
				case TipoOperacion.Potencia:
					//NUMERO-NUMERO
					if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					break;
			}
			return TipoOperacion.Nulo;
		}


		public override object GetValor(TablaSimbolos ts, Sesion sesion)
		{
			Object izq = izquierda?.GetValor(ts, sesion);
			if (izq != null)
				if (izq.GetType() == typeof(ThrowError))
				{
					return izq;
				}
			Object der = derecha?.GetValor(ts, sesion);
			if (der != null)
				if (der.GetType() == typeof(ThrowError))
				{
					return der;
				}

			//operaciones de tipo binarias
			if (izq != null && der != null)
			{
				switch (tipoOp)
				{
					case TipoOperacion.Suma:
						//BOOLEANO-CADENA
						if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.String))
						{
							return izq.ToString() + der.ToString();
						}
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Numero))
						{
							double valor = double.Parse(izq.ToString()) + double.Parse(der.ToString());
							if (valor < -214748364) {
								return new ThrowError(TipoThrow.ArithmeticException,
									"El número es muy pequeño",
									Linea, Columna);
							}
							return valor;
						}
						//NUMERO-CADENA
						if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.String))
						{
							return izq.ToString() + der.ToString();
						}
						//CADENA-BOOLEANO
						if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Booleano))
						{
							return izq.ToString() + der.ToString();
						}
						//CADENA-NUMERO
						if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Numero))
						{
							return izq.ToString() + der.ToString();
						}
						//CADENA-FECHA
						if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Fecha))
						{
							return izq.ToString() + der.ToString();
						}
						//CADENA-HORA
						if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Hora))
						{
							return izq.ToString() + der.ToString();
						}
						//CADENA-CADENA
						if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.String))
						{
							return izq.ToString() + der.ToString();
						}
						//FECHA-CADENA
						if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.String))
						{
							return izq.ToString() + der.ToString();
						}
						//HORA-CADENA
						if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Hora) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.String))
						{
							return izq.ToString() + der.ToString();
						}
						//LISTA-LISTA == SET-SET
						if (izq.GetType() == typeof(CollectionListCql) && der.GetType() == typeof(CollectionListCql))
						{
							CollectionListCql c1 = (CollectionListCql)izq;
							CollectionListCql c2 = (CollectionListCql)der;
							if (c1.IsLista && c2.IsLista || (!c1.IsLista && !c2.IsLista))
							{
								if (c1.TipoDato.Tipo == c2.TipoDato.Tipo && c1.TipoDato.Nombre == c2.TipoDato.Nombre)
								{
									foreach (object val in c2)
									{
										object posibleError = c1.AddItem(val, Linea, Columna);
										if (posibleError != null)
										{
											if (posibleError.GetType() == typeof(ThrowError))
											{
												return posibleError;
											}
										}
									}
									return c1;
								}
								else
								{
									return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden sumar los operandos de tipo " + c1.TipoDato.ToString() + " y " + c2.TipoDato.ToString(),
									Linea, Columna);
								}
							}
							else
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden sumar los operandos de tipo " + c1.TipoDato.ToString() + " y " + c2.TipoDato.ToString(),
									Linea, Columna);
							}
						}
						//MAP - MAP
						if (izq.GetType() == typeof(CollectionMapCql) && der.GetType() == typeof(CollectionMapCql))
						{
							CollectionMapCql c1 = (CollectionMapCql)izq;
							CollectionMapCql c2 = (CollectionMapCql)der;
							if (c1.TipoLlave.Equals(c2.TipoLlave) && c1.TipoValor.Equals(c2.TipoValor))
							{
								if (c1.TipoLlave.Tipo.Equals(c2.TipoLlave.Tipo) && c1.TipoLlave.Nombre.Equals(c2.TipoLlave.Nombre) &&
									c1.TipoValor.Tipo.Equals(c2.TipoValor.Tipo) && c1.TipoValor.Nombre.Equals(c2.TipoValor.Nombre))
								{
									foreach (KeyValuePair<object,object> val in c2)
									{
										object posibleError = c1.AddItem(val.Key,val.Value, Linea, Columna);
										if (posibleError != null)
										{
											if (posibleError.GetType() == typeof(ThrowError))
											{
												Analizador.ErroresCQL.Add(new Error((ThrowError)posibleError));
											}
										}
									}
									return c1;
								}
								else
								{
									return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden sumar los operandos de tipo " + c1.TipoLlave.ToString()+","+c1.TipoValor.ToString() + " y " +
									c2.TipoLlave.ToString() + "," + c2.TipoValor.ToString(),
									Linea, Columna);
								}
							}
							else
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden sumar los operandos de tipo " + c1.TipoLlave.ToString() + "," + c1.TipoValor.ToString() + " y " +
									c2.TipoLlave.ToString() + "," + c2.TipoValor.ToString(),
									Linea, Columna);
							}
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
								"No se pueden sumar los operandos de tipo " + izquierda.GetTipo(ts, sesion) + " y " + derecha.GetTipo(ts, sesion),
								Linea, Columna);
						}
					case TipoOperacion.Resta:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Numero))
						{
							double valor = double.Parse(izq.ToString()) - double.Parse(der.ToString());
							return valor;
						}//LISTA-LISTA == SET-SET
						if (izq.GetType() == typeof(CollectionListCql) && der.GetType() == typeof(CollectionListCql))
						{
							CollectionListCql c1 = (CollectionListCql)izq;
							CollectionListCql c2 = (CollectionListCql)der;
							if (c1.IsLista && c2.IsLista || (!c1.IsLista && !c2.IsLista))
							{
								if (c1.TipoDato.Tipo == c2.TipoDato.Tipo && c1.TipoDato.Nombre == c2.TipoDato.Nombre)
								{
									foreach (object val in c2)
									{
										try {
											if (c1.Contains(val))
											{
												c1.RemoveAt(c1.IndexOf(val));
											}
											else {
												//Analizador.ErroresCQL.Add( new Error(TipoError.Advertencia,
												//"No se pudo eliminar el valor del Collection por que no existe",
												//Linea, Columna));
											}
										}
										catch (Exception) {
											return new ThrowError(TipoThrow.Exception,
												"No se pudo eliminar el valor del Collection por que no existe",
												Linea, Columna);
										}
									}
									return c1;
								}
								else
								{
									return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden sumar los operandos de tipo " + c1.TipoDato.ToString() + " y " + c2.TipoDato.ToString(),
									Linea, Columna);
								}
							}
							else
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden sumar los operandos de tipo " + c1.TipoDato.ToString() + " y " + c2.TipoDato.ToString(),
									Linea, Columna);
							}
						}
						if (izq.GetType() == typeof(CollectionMapCql)&& der.GetType() == typeof(CollectionListCql))
						{
							CollectionMapCql collection = (CollectionMapCql)izq;
							CollectionListCql datos = (CollectionListCql)der;
							if (!datos.IsLista)
							{
								if (collection.TipoValor.Equals(Datos.GetTipoObjetoDB(datos.TipoDato.Nombre)))
								{
									foreach (object dato in datos)
									{
										object aa=collection.EliminarItem(dato, Linea, Columna);
											if (aa!=null) {
												if (aa.GetType()==typeof(ThrowError)) {
													Analizador.ErroresCQL.Add(new Error((ThrowError)aa));
												}
											}
									}
								}
								else
								{
									Analizador.ErroresCQL.Add(new Error(TipoError.Advertencia,
												"No se puede eliminar datos con clave tipo '" + collection.TipoLlave.ToString() + "' y clave tipo'" + Datos.GetTipoObjetoDB(datos.TipoDato.Nombre) + "'",
												Linea, Columna));
								}
							}
							else {
								return new ThrowError(TipoThrow.Exception,
									"Estructura equivocada, las claves deben ir entre llaves",
									Linea, Columna);
							}
							return collection;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
								"No se pueden restar los operandos de tipo " + izquierda.GetTipo(ts, sesion) + " y " + derecha.GetTipo(ts, sesion),
								Linea, Columna);
						}
					case TipoOperacion.Division:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Numero))
						{
							if (double.Parse(der.ToString()) != 0)
							{
								double valor = double.Parse(izq.ToString()) / double.Parse(der.ToString());
								return valor;
							}
							else
							{
								return new ThrowError(TipoThrow.ArithmeticException,
								 "No se puede hacer una división por cero",
								Linea, Columna);
							}

						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden dividir los operandos de tipo " + izquierda.GetTipo(ts, sesion) + " y " + derecha.GetTipo(ts, sesion),
								   Linea, Columna);
						}
					case TipoOperacion.Multiplicacion:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Numero))
						{
							double valor = double.Parse(izq.ToString()) * double.Parse(der.ToString());
							return valor;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden multiplicar los operandos de tipo " + izquierda.GetTipo(ts, sesion) + " y " + derecha.GetTipo(ts, sesion),
								   Linea, Columna);
						}
					case TipoOperacion.Potencia:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Numero))
						{
							double valor = Math.Pow(double.Parse(izq.ToString()), double.Parse(der.ToString()));
							return valor;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden elevar los operandos de tipo " + izquierda.GetTipo(ts, sesion) + " y " + derecha.GetTipo(ts, sesion),
								   Linea, Columna);
						}
					case TipoOperacion.Modulo:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts, sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts, sesion).Equals(TipoOperacion.Numero))
						{
							if (double.Parse(der.ToString()) != 0)
							{
								double valor = double.Parse(izq.ToString()) % double.Parse(der.ToString());
								return valor;
							}
							else
							{
								return new ThrowError(TipoThrow.ArithmeticException,
								 "No se puede hacer una división por cero",
								Linea, Columna);
							}

						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden dividir los operandos de tipo " + izquierda.GetTipo(ts, sesion) + " y " + derecha.GetTipo(ts, sesion),
								   Linea, Columna);
						}
				}
			}
			//operaciones de tipo unarias
			else if (izq != null)
			{
				if (tipoOp == TipoOperacion.Menos)
				{
					if (izquierda.GetTipo(ts, sesion) == TipoOperacion.Numero)
					{
						double valor = double.Parse(izq.ToString()) * -1;
						return valor;
					}
					else
					{
						return new ThrowError(TipoThrow.ArithmeticException,
									"No se puede negar un valor no numérico",
								   Linea, Columna);
					}
				}
			}
			//valores
			else
			{
				switch (TipoOp)
				{
					case TipoOperacion.Identificador:
						//buscar en tabla de simbolos
						if (ts.ExisteSimbolo(this.Valor.ToString()))
						{
							Simbolo s = ts.GetSimbolo(this.Valor.ToString());
							object val = s.Valor;
							if (val != null)
							{
								if (val.GetType() == typeof(string))
								{
									if (val.ToString().Equals("null") && (s.TipoDato.Tipo != TipoDatoDB.NULO && s.TipoDato.Tipo != TipoDatoDB.OBJETO))
									{
										return new ThrowError(Util.TipoThrow.NullPointerException,
													"la variable '" + this.Valor + "' no se ha inicializado",
													Linea, Columna);
									}
								}
								return val;
							}
							else
							{
								return new ThrowError(Util.TipoThrow.NullPointerException,
													"la variable '" + this.Valor + "' no se ha inicializado",
													Linea, Columna);
							}

						}
						else
						{
							return new ThrowError(Util.TipoThrow.ArithmeticException,
												"la variable '" + this.Valor + "' no existe",
												Linea, Columna);
						}
					case TipoOperacion.NuevaInstancia:
						TipoObjetoDB tipoInstancia = (TipoObjetoDB)this.Valor;
						if (Datos.IsLista(tipoInstancia.Tipo))
						{
							object instanciaLista = GetInstanciaLista(tipoInstancia, sesion);
							if (instanciaLista != null)
							{
								return instanciaLista;
							}
						}
						else
						{
							if (tipoInstancia.Tipo == TipoDatoDB.OBJETO)
							{
								object instanciaObjeto = GetInstanciaObjeto(tipoInstancia, sesion);
								if (instanciaObjeto != null)
								{
									return instanciaObjeto;
								}
							}
							else
							{
								//ERROR NO SE PUEDE INSTANCIAR UN TIPO PRIMITIVO
								return new ThrowError(Util.TipoThrow.Exception,
								"No se puede instanciar un tipo primitivo",
								Linea, Columna);
							}
						}
						break;
					case TipoOperacion.ListaDatos:
						List<Expresion> expresiones = (List<Expresion>)this.Valor;
						if (expresiones.Count > 0)
						{
							Expresion primer_elemento;

							primer_elemento = expresiones.ElementAt(0);
							object respuesta = primer_elemento.GetValor(ts, sesion);
							if (respuesta != null)
							{
								if (respuesta.GetType() == typeof(ThrowError))
								{
									return respuesta;
								}
							}
							TipoObjetoDB tipodato = Datos.GetTipoObjetoDB(respuesta);
							TipoObjetoDB tipoCol = null;
							if (Datos.IsPrimitivo(tipodato.Tipo))
							{
								tipoCol = new TipoObjetoDB(TipoDatoDB.LISTA_PRIMITIVO, tipodato.ToString());
							}
							else
							{
								tipoCol = new TipoObjetoDB(TipoDatoDB.LISTA_OBJETO, tipodato.ToString());
							}
							CollectionListCql collection = new CollectionListCql(tipoCol, true);
							foreach (Expresion exp in expresiones)
							{
								object nuevo = exp.GetValor(ts, sesion);
								if (nuevo != null)
								{
									if (nuevo.GetType() == typeof(ThrowError))
									{
										return nuevo;
									}
								}
								if (Datos.IsTipoCompatible(Datos.GetTipoObjetoDBPorCadena(collection.TipoDato.Nombre), nuevo))
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
										"No se puede almacenar un valor " + Datos.GetTipoObjetoDB(nuevo) + " en un Collection tipo " + collection.TipoDato.ToString(),
										Linea, Columna);
								}

							}
							return collection;
						}
						else
						{
							return new ThrowError(TipoThrow.Exception,
									"No se puede asignar una lista vacía",
								   Linea, Columna);

						}
					case TipoOperacion.SetDatos:
						expresiones = (List<Expresion>)this.Valor;
						if (expresiones.Count > 0)
						{
							Expresion primer_elemento;

							primer_elemento = expresiones.ElementAt(0);
							object respuesta = primer_elemento.GetValor(ts, sesion);
							if (respuesta != null)
							{
								if (respuesta.GetType() == typeof(ThrowError))
								{
									return respuesta;
								}
							}
							TipoObjetoDB tipodato = Datos.GetTipoObjetoDB(respuesta);
							TipoObjetoDB tipoCol = null;
							if (Datos.IsPrimitivo(tipodato.Tipo))
							{
								tipoCol = new TipoObjetoDB(TipoDatoDB.SET_PRIMITIVO, tipodato.ToString());
							}
							else
							{
								tipoCol = new TipoObjetoDB(TipoDatoDB.SET_OBJETO, tipodato.ToString());
							}
							CollectionListCql collection = new CollectionListCql(tipoCol, false);
							foreach (Expresion exp in expresiones)
							{
								object nuevo = exp.GetValor(ts, sesion);
								if (nuevo != null)
								{
									if (nuevo.GetType() == typeof(ThrowError))
									{
										return nuevo;
									}
								}
								if (Datos.IsTipoCompatible(Datos.GetTipoObjetoDBPorCadena(collection.TipoDato.Nombre), nuevo))
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
										"No se puede almacenar un valor " + Datos.GetTipoObjetoDB(nuevo) + " en un Collection tipo " + collection.TipoDato.ToString(),
										Linea, Columna);
								}

							}
							return collection;
						}
						else
						{
							return new ThrowError(TipoThrow.Exception,
									"No se puede asignar una lista vacía",
								   Linea, Columna);

						}
					case TipoOperacion.MapDatos:
						InfoCollection info = (InfoCollection)this.Valor;
						if (info.Count > 0)
						{
							object expClave = info.ElementAt(0).Expresion1.GetValor(ts, sesion);
							if (expClave != null)
							{
								if (expClave.GetType() == typeof(ThrowError))
								{
									return expClave;
								}
							}
							object expValor = info.ElementAt(0).Expresion2.GetValor(ts, sesion);
							if (expValor != null)
							{
								if (expValor.GetType() == typeof(ThrowError))
								{
									return expValor;
								}
							}

							TipoObjetoDB tipoClave = Datos.GetTipoObjetoDB(expClave);
							TipoObjetoDB tipoValor = Datos.GetTipoObjetoDB(expValor);
							if (Datos.IsPrimitivo(tipoClave.Tipo))
							{
								CollectionMapCql collection = new CollectionMapCql(tipoClave, tipoValor);
								foreach (Info valor in info)
								{
									//OBTENIENDO VALORES Y TIPOS
									expClave = valor.Expresion1.GetValor(ts, sesion);
									if (expClave != null)
									{
										if (expClave.GetType() == typeof(ThrowError))
										{
											return expClave;
										}
									}
									expValor = valor.Expresion2.GetValor(ts, sesion);
									if (expValor != null)
									{
										if (expValor.GetType() == typeof(ThrowError))
										{
											return expValor;
										}
									}

									tipoClave = Datos.GetTipoObjetoDB(expClave);
									tipoValor = Datos.GetTipoObjetoDB(expValor);
									if (Datos.IsTipoCompatible(collection.TipoLlave, expClave))
									{
										if (Datos.IsTipoCompatibleParaAsignar(collection.TipoValor, expValor))
										{
											object posibleError = collection.AddItem(expClave, expValor, Linea, Columna);
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
												"No se puede almacenar un valor " + tipoClave.ToString() + " en un valor tipo " + collection.TipoValor.ToString(),
												Linea, Columna);
										}
									}
									else
									{
										return new ThrowError(Util.TipoThrow.Exception,
										"El valor '" + expClave.ToString() + "' no corresponde con el tipo '" + collection.TipoLlave.ToString() + "' de clave del collection map",
										Linea, Columna);
									}
									//****
								}
								return collection;
							}
							else
							{
								return new ThrowError(TipoThrow.Exception,
										"No se puede asignar un valor tipo '" + tipoClave.ToString() + "' como llave",
									   Linea, Columna);
							}
						}
						else
						{
							return new ThrowError(TipoThrow.Exception,
										"No se puede asignar una lista vacía",
									   Linea, Columna);
						}
					default:
						return this.Valor;
				}
			}

			return new ThrowError(TipoThrow.Exception,
									"Ha ocurrido un error grave al evaluar la expresión",
								   Linea, Columna);
		}

		private object GetInstanciaObjeto(TipoObjetoDB tipoInstancia, Sesion sesion)
		{
			//VALIDANDO BASEDATOS
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				if (db.ExisteUserType(tipoInstancia.ToString()))
				{
					UserType ut = db.BuscarUserType(tipoInstancia.ToString());
					Objeto nuevaInstancia = new Objeto(ut);
					foreach (KeyValuePair<string, TipoObjetoDB> atributo in ut.Atributos)
					{
						nuevaInstancia.Atributos.Add(atributo.Key, Declaracion.GetValorPredeterminado(atributo.Value.Tipo));
					}
					return nuevaInstancia;
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

		private object GetInstanciaLista(TipoObjetoDB tipoInstancia, Sesion sesion)
		{
			switch (tipoInstancia.Tipo)
			{
				case TipoDatoDB.LISTA_OBJETO:
				case TipoDatoDB.SET_OBJETO:
					//VALIDAR TIPO
					object re = ValidarInstanciaLista(tipoInstancia, sesion);
					if (re != null)
					{
						if (re.GetType() == typeof(ThrowError))
						{
							return re;
						}
					}
					return new CollectionListCql(tipoInstancia, true);

				case TipoDatoDB.LISTA_PRIMITIVO:
					TipoObjetoDB tipoDatoLista = Datos.GetTipoObjetoDBPorCadena(tipoInstancia.Nombre);
					if (Datos.IsPrimitivo(tipoDatoLista.Tipo))
					{
						return new CollectionListCql(new TipoObjetoDB(TipoDatoDB.LISTA_PRIMITIVO, tipoDatoLista.Nombre), true);
					}
					break;
				case TipoDatoDB.SET_PRIMITIVO:
					tipoDatoLista = Datos.GetTipoObjetoDBPorCadena(tipoInstancia.Nombre);
					if (Datos.IsPrimitivo(tipoDatoLista.Tipo))
					{
						return new CollectionListCql(new TipoObjetoDB(TipoDatoDB.SET_PRIMITIVO, tipoDatoLista.Nombre), false);
					}
					break;
				case TipoDatoDB.MAP_OBJETO:
					string[] tipos = tipoInstancia.Nombre.Split(',');
					TipoObjetoDB t1 = Datos.GetTipoObjetoDBPorCadena(tipos.ElementAt(0));
					TipoObjetoDB t2 = Datos.GetTipoObjetoDBPorCadena(tipos.ElementAt(1));
					if (Datos.IsPrimitivo(t1.Tipo))
					{
						//COMPROBAR TIPO COMO LISTA 
						re = ValidarInstanciaLista(new TipoObjetoDB(TipoDatoDB.LISTA_OBJETO, t2.Nombre), sesion);
						if (re != null)
						{
							if (re.GetType() == typeof(ThrowError))
							{
								return re;
							}
						}
						return new CollectionMapCql(t1, t2);
					}
					else
					{
						return new ThrowError(Util.TipoThrow.Exception,
										"El Map solo puede tener llaves de tipo primitivo",
										Linea, Columna);
					}
				case TipoDatoDB.MAP_PRIMITIVO:
					tipos = tipoInstancia.Nombre.Split(',');
					t1 = Datos.GetTipoObjetoDBPorCadena(tipos.ElementAt(0));
					t2 = Datos.GetTipoObjetoDBPorCadena(tipos.ElementAt(1));
					if (Datos.IsPrimitivo(t1.Tipo))
					{
						return new CollectionMapCql(t1, t2);
					}
					else
					{
						return new ThrowError(Util.TipoThrow.Exception,
										"El Map solo puede tener llaves de tipo primitivo",
										Linea, Columna);
					}

			}
			return null;
		}

		private object ValidarInstanciaLista(TipoObjetoDB tipoInstancia, Sesion sesion)
		{
			switch (tipoInstancia.Tipo)
			{
				case TipoDatoDB.LISTA_OBJETO:
				case TipoDatoDB.SET_OBJETO:
					TipoObjetoDB tipoAdentro = Datos.GetTipoObjetoDBPorCadena(tipoInstancia.Nombre);
					if (Datos.IsLista(tipoAdentro.Tipo))
					{
						object re = ValidarInstanciaLista(tipoAdentro, sesion);
						if (re != null)
						{
							if (re.GetType() == typeof(ThrowError))
							{
								return re;
							}
						}
						return ((bool)re);
					}
					else if (Datos.IsPrimitivo(tipoAdentro.Tipo))
					{
						return true;
					}
					else
					{
						//comprobar que exista el objeto
						object condicion = ExisteObjeto(tipoAdentro, sesion, Linea, Columna);
						if (condicion.GetType() == typeof(ThrowError))
						{
							return condicion;
						}
						return true;
					}
				case TipoDatoDB.LISTA_PRIMITIVO:
				case TipoDatoDB.SET_PRIMITIVO:
					return true;
				case TipoDatoDB.MAP_OBJETO:
				case TipoDatoDB.MAP_PRIMITIVO:
					string[] tipos = tipoInstancia.Nombre.Split(',');
					tipoAdentro = Datos.GetTipoObjetoDBPorCadena(tipos[1]);
					if (Datos.IsLista(tipoAdentro.Tipo))
					{
						object re = ValidarInstanciaLista(tipoAdentro, sesion);
						if (re != null)
						{
							if (re.GetType() == typeof(ThrowError))
							{
								return re;
							}
						}
						return ((bool)re);
					}
					else if (Datos.IsPrimitivo(tipoAdentro.Tipo))
					{
						return true;
					}
					else
					{
						//comprobar que exista el objeto
						object condicion = ExisteObjeto(tipoAdentro, sesion, Linea, Columna);
						if (condicion.GetType() == typeof(ThrowError))
						{
							return condicion;
						}
						return true;
					}
			}
			return false;
		}

		public static object ExisteObjeto(TipoObjetoDB tipoDatoLista, Sesion sesion, int Linea, int Columna)
		{
			//VALIDANDO BASEDATOS
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				if (db.ExisteUserType(tipoDatoLista.Nombre))
				{
					return true;
				}
				else
				{
					return new ThrowError(Util.TipoThrow.TypeDontExists,
				"El user Type '" + tipoDatoLista.Nombre + "' no existe",
				Linea, Columna);
				}
			}
			else
			{
				return new ThrowError(Util.TipoThrow.UseBDException,
					"No se puede ejecutar la sentencia porque no hay una base de datos seleccionada",
					Linea, Columna);
			}
		}

	}
	public enum TipoOperacion
	{
		//valores
		Numero,
		Booleano,
		Identificador,
		String,
		Caracter,
		Fecha,
		Hora,
		Nulo,
		Objeto,
		Nombre,
		NuevaInstancia,
		InstanciaObjeto,
		ListaDatos,
		SetDatos,
		MapDatos,
		//operaciones
		Suma,
		Resta,
		Multiplicacion,
		Division,
		Menos,
		Potencia,
		Modulo,
		//relacional
		Mayor,
		Menor,
		MayorIgual,
		MenorIgual,
		Diferente,
		Igual,
		Not,
		And,
		Or,
		Xor
	}
}
