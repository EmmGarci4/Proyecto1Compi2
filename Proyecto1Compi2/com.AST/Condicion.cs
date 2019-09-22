using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class Condicion : Expresion
	{
		TipoOperacion tipoOp;
		Expresion izquierda;
		Expresion derecha;
		readonly bool valor;

		public Condicion(Expresion izquierda, Expresion derecha, TipoOperacion tipoOp, int linea, int columna) : base(linea, columna)
		{
			this.tipoOp = tipoOp;
			this.izquierda = izquierda;
			this.derecha = derecha;
			this.valor = false;
		}

		public Condicion(Expresion izquierda, TipoOperacion tipoOp, int linea, int columna) : base(linea, columna)
		{
			this.tipoOp = tipoOp;
			this.izquierda = izquierda;
			this.derecha = null;
			this.valor = false;
		}

		public Condicion(bool izquierda, TipoOperacion tipoOp, int linea, int columna) : base(linea, columna)
		{
			this.tipoOp = tipoOp;
			this.izquierda = null;
			this.derecha = null;
			this.valor = izquierda;
		}

		public TipoOperacion TipoOp { get => tipoOp; set => tipoOp = value; }
		internal Expresion Izquierda { get => izquierda; set => izquierda = value; }
		internal Expresion Derecha { get => derecha; set => derecha = value; }

		public override TipoOperacion GetTipo(TablaSimbolos ts,Sesion sesion)
		{
			return TipoOperacion.Booleano;
		}

		public override object GetValor(TablaSimbolos ts,Sesion sesion)
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

					case TipoOperacion.Diferente:
						//STRING-STRING
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.String))
						{
							bool valor = izq.ToString() != der.ToString();
							return valor;
						}
						else
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
						{
							bool valor = double.Parse(izq.ToString()) != double.Parse(der.ToString());
							return valor;
						}
						else
						//BOOLEANO-BOOLEANO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Booleano))
						{
							bool valor = bool.Parse(izq.ToString()) != bool.Parse(der.ToString());
							return valor;
						}
						else
						//FECHA-FECHA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Fecha))
						{
							MyDateTime d1 = (MyDateTime)izq;
							MyDateTime d2 = (MyDateTime)der;
							int valor = DateTime.Compare(d1.Dato, d2.Dato);
							return valor != 0;
						}
						else
						//HORA-HORA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Hora) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Hora))
						{
							MyDateTime d1 = (MyDateTime)izq;
							MyDateTime d2 = (MyDateTime)der;
							int valor = DateTime.Compare(d1.Dato, d2.Dato);
							return valor != 0;
						}
						else
						//OBJETO - NULL
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Objeto) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Nulo))
						{
							return !((Objeto)izq).IsNull;
						}
						else
						//NULL - OBJETO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Nulo) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Objeto))
						{
							return !((Objeto)der).IsNull;
						}
						else
						//OBJETO-OBJETO 
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Objeto) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Objeto))
						{
							//Console.WriteLine(izq.GetHashCode());
							//Console.WriteLine(der.GetHashCode());
							return !izq.Equals(der);
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden comparar los operandos de tipo " + izquierda.GetTipo(ts,sesion) + " y " + derecha.GetTipo(ts,sesion),
								   Linea, Columna);
						}
					case TipoOperacion.Igual:
						//STRING-STRING
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.String) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.String))
						{
							bool valor = izq.ToString() == der.ToString();
							return valor;
						}
						else
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
						{
							bool valor = double.Parse(izq.ToString()) == double.Parse(der.ToString());
							return valor;
						}
						else
						//BOOLEANO-BOOLEANO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Booleano))
						{
							bool valor = bool.Parse(izq.ToString()) == bool.Parse(der.ToString());
							return valor;
						}
						else
						//FECHA-FECHA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Fecha))
						{
							MyDateTime d1 = (MyDateTime)izq;
							MyDateTime d2 = (MyDateTime)der;
							int valor = DateTime.Compare(d1.Dato, d2.Dato);
							return valor == 0;
						}
						else
						//HORA-HORA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Hora) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Hora))
						{
							MyDateTime d1 = (MyDateTime)izq;
							MyDateTime d2 = (MyDateTime)der;
							int valor = DateTime.Compare(d1.Dato, d2.Dato);
							return valor == 0;
						}else
						//OBJETO - NULL
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Objeto) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Nulo))
						{
							return ((Objeto)izq).IsNull;
						}
						else
						//NULL - OBJETO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Nulo) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Objeto))
						{
							return ((Objeto)der).IsNull;
						}
						else
						//OBJETO-OBJETO 
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Objeto) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Objeto))
						{
							return izq.Equals(der);
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden comparar los operandos de tipo " + izquierda.GetTipo(ts,sesion) + " y " + derecha.GetTipo(ts,sesion),
								   Linea, Columna);
						}
					case TipoOperacion.Mayor:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
						{
							bool valor = double.Parse(izq.ToString()) > double.Parse(der.ToString());
							return valor;
						}
						else
						//FECHA-FECHA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Fecha))
						{
							MyDateTime d1 = (MyDateTime)izq;
							MyDateTime d2 = (MyDateTime)der;
							int valor = DateTime.Compare(d1.Dato, d2.Dato);
							return valor > 0;
						}
						else
						//HORA-HORA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Hora) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Hora))
						{
							MyDateTime d1 = (MyDateTime)izq;
							MyDateTime d2 = (MyDateTime)der;
							int valor = DateTime.Compare(d1.Dato, d2.Dato);
							return valor > 0;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden comparar los operandos de tipo " + izquierda.GetTipo(ts,sesion) + " y " + derecha.GetTipo(ts,sesion),
								   Linea, Columna);
						}
					case TipoOperacion.MayorIgual:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
						{
							bool valor = double.Parse(izq.ToString()) >= double.Parse(der.ToString());
							return valor;
						}
						else
						//FECHA-FECHA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Fecha))
						{
							MyDateTime d1 = (MyDateTime)izq;
							MyDateTime d2 = (MyDateTime)der;
							int valor = DateTime.Compare(d1.Dato, d2.Dato);
							return valor >= 0;
						}
						else
						//HORA-HORA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Hora) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Hora))
						{
							MyDateTime d1 = (MyDateTime)izq;
							MyDateTime d2 = (MyDateTime)der;
							int valor = DateTime.Compare(d1.Dato, d2.Dato);
							return valor >= 0;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden comparar los operandos de tipo " + izquierda.GetTipo(ts,sesion) + " y " + derecha.GetTipo(ts,sesion),
								   Linea, Columna);
						}
					case TipoOperacion.Menor:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
						{
							bool valor = double.Parse(izq.ToString()) < double.Parse(der.ToString());
							return valor;
						}
						else
						//FECHA-FECHA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Fecha))
						{
							MyDateTime d1 = (MyDateTime)izq;
							MyDateTime d2 = (MyDateTime)der;
							int valor = DateTime.Compare(d1.Dato, d2.Dato);
							return valor < 0;
						}
						else
						//HORA-HORA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Hora) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Hora))
						{
							MyDateTime d1 = (MyDateTime)izq;
							MyDateTime d2 = (MyDateTime)der;
							int valor = DateTime.Compare(d1.Dato, d2.Dato);
							return valor < 0;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden comparar los operandos de tipo " + izquierda.GetTipo(ts,sesion) + " y " + derecha.GetTipo(ts,sesion),
								   Linea, Columna);
						}
					case TipoOperacion.MenorIgual:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Numero))
						{
							bool valor = double.Parse(izq.ToString()) <= double.Parse(der.ToString());
							return valor;
						}
						else
						//FECHA-FECHA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Fecha))
						{
							MyDateTime d1 = (MyDateTime)izq;
							MyDateTime d2 = (MyDateTime)der;
							int valor = DateTime.Compare(d1.Dato, d2.Dato);
							return valor <= 0;
						}
						else
						//HORA-HORA
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Hora) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Hora))
						{
							MyDateTime d1 = (MyDateTime)izq;
							MyDateTime d2 = (MyDateTime)der;
							int valor = DateTime.Compare(d1.Dato, d2.Dato);
							return valor <= 0;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden comparar los operandos de tipo " + izquierda.GetTipo(ts,sesion) + " y " + derecha.GetTipo(ts,sesion),
								   Linea, Columna);
						}
					case TipoOperacion.Or:
						//BOOLEANO-BOOLEANO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Booleano))
						{
							bool valor = bool.Parse(izq.ToString()) || bool.Parse(der.ToString());
							return valor;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se puede aplicar OR a los operandos de tipo " + izquierda.GetTipo(ts,sesion) + " y " + derecha.GetTipo(ts,sesion),
								   Linea, Columna);
						}
					case TipoOperacion.And:
						//BOOLEANO-BOOLEANO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Booleano))
						{
							bool valor = bool.Parse(izq.ToString()) && bool.Parse(der.ToString());
							return valor;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se puede aplicar OR a los operandos de tipo " + izquierda.GetTipo(ts,sesion) + " y " + derecha.GetTipo(ts,sesion),
								   Linea, Columna);
						}
					case TipoOperacion.Xor:
						//BOOLEANO-BOOLEANO
						if (Izquierda.GetTipo(ts,sesion).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts,sesion).Equals(TipoOperacion.Booleano))
						{
							bool valor = bool.Parse(izq.ToString()) == bool.Parse(der.ToString());
							return !valor;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se puede aplicar OR a los operandos de tipo " + izquierda.GetTipo(ts,sesion) + " y " + derecha.GetTipo(ts,sesion),
								   Linea, Columna);
						}
				}
			}//operaciones de tipo unarias
			else if (izq != null)
			{
				if (tipoOp == TipoOperacion.Not)
				{
					if (izquierda.GetTipo(ts,sesion) == TipoOperacion.Booleano)
					{
						bool valor = !bool.Parse(izq.ToString());
						return valor;
					}
					else
					{
						return new ThrowError(TipoThrow.ArithmeticException,
									"No se puede negar un valor no booleano",
								   Linea, Columna);
					}
				}
			}
			//valores
			else
			{

				return this.valor;
			}
			return new ThrowError(TipoThrow.Exception,
									"Ha ocurrido un error grave al evaluar la condición",
								   Linea, Columna);
		}
	}
}
