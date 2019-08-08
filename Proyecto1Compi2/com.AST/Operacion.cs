using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis;
using com.Analisis.Util;

namespace Proyecto1Compi2.com.AST
{
	class Operacion : Expresion
	{
		TipoOperacion tipoOp;
		private readonly string Valor;
		Expresion izquierda;
		Expresion derecha;
		/*
		 * 
		 *constructor para operaciones binarias
		 * EXPRESION operador EXPRESION
		 * sumas,restas,multiplicaciones,divisiones, y comparaciones
		 * 
		 */
		public Operacion(Expresion operadorIzq, Expresion operadorDer, TipoOperacion tipo,int linea,int columna):base(linea,columna)
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
		public Operacion(Expresion operadorIzq,TipoOperacion tipo, int linea, int columna) : base(linea, columna)
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
		public Operacion(string valor, TipoOperacion tipo, int linea, int columna) : base(linea, columna)
		{
			this.tipoOp = tipo;
			this.Valor = valor;
			this.izquierda = null;
			this.derecha = null;
		}	

		public TipoOperacion TipoOp { get => tipoOp; set => tipoOp = value; }
		internal Expresion Izquierda { get => izquierda; set => izquierda = value; }
		internal Expresion Derecha { get => derecha; set => derecha = value; }

		public override TipoOperacion GetTipo(TablaSimbolos ts)
		{
			switch (tipoOp)
			{
				case TipoOperacion.Numero:
				case TipoOperacion.Cadena:
				case TipoOperacion.Booleano:
				case TipoOperacion.Caracter:
				case TipoOperacion.Identificador:
				case TipoOperacion.Fecha:
				case TipoOperacion.FechaHora:
					return this.tipoOp;
				case TipoOperacion.Suma:
					//BOOLEANO-BOOLEANO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
					{
						return TipoOperacion.Booleano;
					}
					//BOOLEANO-NUMERO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					//BOOLEANO-CADENA
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
					{
						return TipoOperacion.Cadena;
					}
					//NUMERO-BOOLEANO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
					{
						return TipoOperacion.Numero;
					}
					//NUMERO-NUMERO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					//NUMERO-CADENA
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
					{
						return TipoOperacion.Cadena;
					}
					//CADENA-BOOLEANO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Cadena) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
					{
						return TipoOperacion.Cadena;
					}
					//CADENA-NUMERO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Cadena) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Cadena;
					}
					//CADENA-FECHA
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Cadena) && Derecha.GetTipo(ts).Equals(TipoOperacion.Fecha))
					{
						return TipoOperacion.Cadena;
					}
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Cadena) && Derecha.GetTipo(ts).Equals(TipoOperacion.FechaHora))
					{
						return TipoOperacion.Cadena;
					}
					//CADENA-CADENA
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Cadena) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
					{
						return TipoOperacion.Cadena;
					}
					//Fecha-CADENA
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
					{
						return TipoOperacion.Cadena;
					}
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.FechaHora) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
					{
						return TipoOperacion.Cadena;
					}
					break;
				case TipoOperacion.Resta:
					//BOOLEANO-NUMERO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					//NUMERO-BOOLEANO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
					{
						return TipoOperacion.Numero;
					}
					//NUMERO-NUMERO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					break;
				case TipoOperacion.Division:
					//BOOLEANO-NUMERO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					//NUMERO-BOOLEANO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
					{
						return TipoOperacion.Numero;
					}
					//NUMERO-NUMERO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					//Fecha-CADENA
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
					{
						return TipoOperacion.Cadena;
					}
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.FechaHora) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
					{
						return TipoOperacion.Cadena;
					}
					break;
				case TipoOperacion.Multiplicacion:
					//BOOLEANO-BOOLEANO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
					{
						return TipoOperacion.Booleano;
					}
					//BOOLEANO-NUMERO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					//NUMERO-BOOLEANO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
					{
						return TipoOperacion.Numero;
					}
					//NUMERO-NUMERO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					//Fecha-CADENA
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
					{
						return TipoOperacion.Cadena;
					}
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.FechaHora) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
					{
						return TipoOperacion.Cadena;
					}
					break;
				case TipoOperacion.Potencia:
					//BOOLEANO-NUMERO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					//NUMERO-BOOLEANO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
					{
						return TipoOperacion.Numero;
					}
					//NUMERO-NUMERO
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
					{
						return TipoOperacion.Numero;
					}
					//Fecha-CADENA
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
					{
						return TipoOperacion.Cadena;
					}
					if (Izquierda.GetTipo(ts).Equals(TipoOperacion.FechaHora) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
					{
						return TipoOperacion.Cadena;
					}
					break;
			}
			return TipoOperacion.Nulo;
		}

		public override string GetValor(TablaSimbolos ts)
		{
			Object izq = izquierda?.GetValor(ts);
			Object der = derecha?.GetValor(ts);
			//operaciones de tipo binarias
			if (izq != null && der != null)
			{
				switch (tipoOp)
				{
					case TipoOperacion.Suma:
						//BOOLEANO-BOOLEANO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
						{
							bool valor = Boolean.Parse(izq.ToString()) || Boolean.Parse(der.ToString());
							return valor.ToString();
						}
						//BOOLEANO-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							if (Boolean.Parse(izq.ToString()))
							{
								double valor = double.Parse(der.ToString()) + 1;
								return valor.ToString();
							}
							else
							{
								return der.ToString();
							}
						}
						//BOOLEANO-CADENA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
						{
							return izq.ToString() + der.ToString();
						}
						//NUMERO-BOOLEANO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
						{
							if (Boolean.Parse(der.ToString()))
							{
								double valor = double.Parse(izq.ToString()) + 1;
								return valor.ToString();
							}
							else
							{
								return izq.ToString();
							}
						}
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							double valor = double.Parse(izq.ToString()) + double.Parse(der.ToString());
							return valor.ToString();
						}
						//NUMERO-CADENA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
						{
							return izq.ToString() + der.ToString();
						}
						//CADENA-BOOLEANO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Cadena) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
						{
							return izq.ToString() + der.ToString();
						}
						//CADENA-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Cadena) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							return izq.ToString() + der.ToString();
						}
						//CADENA-FECHA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Cadena) && Derecha.GetTipo(ts).Equals(TipoOperacion.Fecha))
						{
							return izq.ToString() + der.ToString();
						}
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Cadena) && Derecha.GetTipo(ts).Equals(TipoOperacion.FechaHora))
						{
							return izq.ToString() + der.ToString();
						}
						//CADENA-CADENA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Cadena) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
						{
							return izq.ToString() + der.ToString();
						}
						//Fecha-CADENA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
						{
							return izq.ToString() + der.ToString();
						}
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.FechaHora) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
						{
							return izq.ToString() + der.ToString();
						}
						else
						{
							Analizador.Errores.Add(new Error(TipoError.Semantico, "No se pueden sumar los operandos de tipo " + izquierda.GetTipo(ts) + " y " + derecha.GetTipo(ts), Linea, Columna));
							break;
						}
					case TipoOperacion.Resta:
						//BOOLEANO-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							if (Boolean.Parse(izq.ToString()))
							{
								double valor = 1-double.Parse(der.ToString());
								return valor.ToString();
							}
							else
							{
								double valor = 0-double.Parse(der.ToString());
								return valor.ToString();
							}
						}
						//NUMERO-BOOLEANO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
						{
							if (Boolean.Parse(der.ToString()))
							{
								double valor = double.Parse(izq.ToString())-1 ;
								return valor.ToString();
							}
							else
							{
								return izq.ToString();
							}
						}
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							double valor = double.Parse(izq.ToString()) - double.Parse(der.ToString());
							return valor.ToString();
						}
						else
						{
							Analizador.Errores.Add(new Error(TipoError.Semantico, "No se pueden restar los operandos de tipo " + izquierda.GetTipo(ts) + " y " + derecha.GetTipo(ts), Linea, Columna));
							break;
						}
					case TipoOperacion.Division:
						//BOOLEANO-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							if (Boolean.Parse(izq.ToString()))
							{
								return izq.ToString();
							}
							else
							{
								Analizador.Errores.Add(new Error(TipoError.Semantico, "No se puede hacer una división por cero", Linea, Columna));
								break;
							}
							
						}
						//NUMERO-BOOLEANO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
						{
							
						}
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							if (double.Parse(der.ToString()) != 0)
							{
								double valor = double.Parse(izq.ToString()) / double.Parse(der.ToString());
								return valor.ToString();
							}
							else {
								Analizador.Errores.Add(new Error(TipoError.Semantico, "No se puede hacer una división por cero", Linea, Columna));
								break;
							}
							
						}
						//Fecha-CADENA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
						{
							return izq.ToString() + der.ToString();
						}
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.FechaHora) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
						{
							return izq.ToString() + der.ToString();
						}
						else
						{
							Analizador.Errores.Add(new Error(TipoError.Semantico, "No se pueden dividir los operandos de tipo " + izquierda.GetTipo(ts) + " y " + derecha.GetTipo(ts), Linea, Columna));
							break;
						}
					case TipoOperacion.Multiplicacion:
						//BOOLEANO-BOOLEANO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
						{
							bool valor = Boolean.Parse(izq.ToString()) && Boolean.Parse(der.ToString());
							return valor.ToString();
						}
						//BOOLEANO-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							if (Boolean.Parse(izq.ToString()))
							{
								double valor = 1 * double.Parse(der.ToString());
								return valor.ToString();
							}
							else
							{
								return "0";
							}
						}
						//NUMERO-BOOLEANO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
						{
							if (Boolean.Parse(der.ToString()))
							{
								double valor = double.Parse(izq.ToString()) *1;
								return valor.ToString();
							}
							else
							{
								return "0";
							}
						}
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							double valor = double.Parse(izq.ToString()) * double.Parse(der.ToString());
							return valor.ToString();
						}
						//Fecha-CADENA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
						{
							return izq.ToString() + der.ToString();
						}
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.FechaHora) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
						{
							return izq.ToString() + der.ToString();
						}
						else
						{
							Analizador.Errores.Add(new Error(TipoError.Semantico, "No se pueden multiplicar los operandos de tipo " + izquierda.GetTipo(ts) + " y " + derecha.GetTipo(ts), Linea, Columna));
							break;
						}
					case TipoOperacion.Potencia:
						//BOOLEANO-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							if (Boolean.Parse(izq.ToString()))
							{
								return "1";
							}
							else
							{
								return "0";
							}
						}
						//NUMERO-BOOLEANO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
						{
							if (Boolean.Parse(der.ToString()))
							{
								return izq.ToString();
							}
							else
							{
								return "0";
							}
						}
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							double valor = Math.Pow(double.Parse(izq.ToString()), double.Parse(der.ToString()));
							return valor.ToString();
						}
						//Fecha-CADENA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
						{
							return izq.ToString() + der.ToString();
						}
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.FechaHora) && Derecha.GetTipo(ts).Equals(TipoOperacion.Cadena))
						{
							return izq.ToString() + der.ToString();
						}
						else
						{
							Analizador.Errores.Add(new Error(TipoError.Semantico, "No se pueden elevar los operandos de tipo " + izquierda.GetTipo(ts) + " y " + derecha.GetTipo(ts), Linea, Columna));
							break;
						}
				}
			}
			//operaciones de tipo unarias
			else if (izq != null)
			{
				if (tipoOp == TipoOperacion.Menos)
				{
					if (izquierda.GetTipo(ts) == TipoOperacion.Numero)
					{
						double valor = double.Parse(izq.ToString()) * -1;
						return valor.ToString();
					}
					else
					{
						Console.WriteLine("Error de tipos");
					}
				}
			}
			//valores
			else {
				return this.Valor;
			}

				return null;
		}
	}
	public enum TipoOperacion {
		//valores
		Numero,
		Booleano,
		Identificador,
		Cadena,
		Caracter,
		Fecha,
		FechaHora,
		Nulo,
		//operaciones
		Suma,
		Resta,
		Multiplicacion,
		Division,
		Menos,
		Potencia,
		//relacional
		Mayor,
		Menor,
		MayorIgual,
		MenorIgual,
		Diferente,
		Igual,
		Not,
		And,
		Or

	}
}
