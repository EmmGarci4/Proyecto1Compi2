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
					if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.String))
					{
						return TipoOperacion.String;
					}
					//NUMERO-NUMERO
					if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					//NUMERO-CADENA
					if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.String))
					{
						return TipoOperacion.String;
					}
					//CADENA-BOOLEANO
					if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Booleano))
					{
						return TipoOperacion.String;
					}
					//CADENA-NUMERO
					if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.String;
					}
					//CADENA-FECHA
					if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Fecha))
					{
						return TipoOperacion.String;
					}
					//CADENA-HORA
					if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Hora))
					{
						return TipoOperacion.String;
					}
					//CADENA-CADENA
					if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.String))
					{
						return TipoOperacion.String;
					}
					//FECHA-CADENA
					if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.String))
					{
						return TipoOperacion.String;
					}
					//HORA-CADENA
					if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Hora) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.String))
					{
						return TipoOperacion.String;
					}
					break;
				case TipoOperacion.Resta:
					//NUMERO-NUMERO
					if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					break;
				case TipoOperacion.Division:
					//NUMERO-NUMERO
					if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					break;
				case TipoOperacion.Multiplicacion:
					//NUMERO-NUMERO
					if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					break;
				case TipoOperacion.Potencia:
					//NUMERO-NUMERO
					if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					break;
			}
			return TipoOperacion.Nulo;
		}


		public override object GetValor(TablaSimbolos ts, Sesion sesion)
		{
			Object izq = izquierda?.GetValor(ts,sesion);
			if (izq != null)
				if (izq.GetType() == typeof(ThrowError))
				{
					return izq;
				}
			Object der = derecha?.GetValor(ts,sesion);
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
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.String))
						{
							return izq.ToString() + der.ToString();
						}
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
						{
							double valor = double.Parse(izq.ToString()) + double.Parse(der.ToString());
							return valor;
						}
						//NUMERO-CADENA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.String))
						{
							return izq.ToString() + der.ToString();
						}
						//CADENA-BOOLEANO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Booleano))
						{
							return izq.ToString() + der.ToString();
						}
						//CADENA-NUMERO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
						{
							return izq.ToString() + der.ToString();
						}
						//CADENA-FECHA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Fecha))
						{
							return izq.ToString() + der.ToString();
						}
						//CADENA-HORA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Hora))
						{
							return izq.ToString() + der.ToString();
						}
						//CADENA-CADENA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.String))
						{
							return izq.ToString() + der.ToString();
						}
						//FECHA-CADENA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.String))
						{
							return izq.ToString() + der.ToString();
						}
						//HORA-CADENA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Hora) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.String))
						{
							return izq.ToString() + der.ToString();
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
								"No se pueden sumar los operandos de tipo " + izquierda.GetTipo(ts,sesion) + " y " + derecha.GetTipo(ts,sesion),
								Linea, Columna);
						}
					case TipoOperacion.Resta:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
						{
							double valor = double.Parse(izq.ToString()) - double.Parse(der.ToString());
							return valor;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
								"No se pueden restar los operandos de tipo " + izquierda.GetTipo(ts,sesion) + " y " + derecha.GetTipo(ts,sesion),
								Linea, Columna);
						}
					case TipoOperacion.Division:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
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
									"No se pueden dividir los operandos de tipo " + izquierda.GetTipo(ts,sesion) + " y " + derecha.GetTipo(ts,sesion),
								   Linea, Columna);
						}
					case TipoOperacion.Multiplicacion:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
						{
							double valor = double.Parse(izq.ToString()) * double.Parse(der.ToString());
							return valor;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden multiplicar los operandos de tipo " + izquierda.GetTipo(ts,sesion) + " y " + derecha.GetTipo(ts,sesion),
								   Linea, Columna);
						}
					case TipoOperacion.Potencia:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
						{
							double valor = Math.Pow(double.Parse(izq.ToString()), double.Parse(der.ToString()));
							return valor;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden elevar los operandos de tipo " + izquierda.GetTipo(ts,sesion) + " y " + derecha.GetTipo(ts,sesion),
								   Linea, Columna);
						}
					case TipoOperacion.Modulo:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
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
									"No se pueden dividir los operandos de tipo " + izquierda.GetTipo(ts,sesion) + " y " + derecha.GetTipo(ts,sesion),
								   Linea, Columna);
						}
				}
			}
			//operaciones de tipo unarias
			else if (izq != null)
			{
				if (tipoOp == TipoOperacion.Menos)
				{
					if (izquierda.GetTipo(ts,sesion) == TipoOperacion.Numero)
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
				if (this.TipoOp == TipoOperacion.Identificador)
				{
					//buscar en tabla de simbolos
					if (ts.ExisteSimbolo(this.Valor.ToString()))
					{
						Simbolo s = ts.GetSimbolo(this.Valor.ToString());
						object val = s.Valor;
						if (val != null)
						{
							if (val.GetType()==typeof(string)) {
								if (val.ToString().Equals("null") && (s.TipoDato.Tipo!=TipoDatoDB.NULO && s.TipoDato.Tipo!= TipoDatoDB.OBJETO)) {
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
				}
				return this.Valor;
			}

			return new ThrowError(TipoThrow.Exception,
									"Ha ocurrido un error grave al evaluar la expresión",
								   Linea, Columna);
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
