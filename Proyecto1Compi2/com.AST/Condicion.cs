using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class Condicion : Expresion
	{
		TipoOperacion tipoOp;
		Expresion izquierda;
		Expresion derecha;
		bool valor;

		public Condicion(Expresion izquierda, Expresion derecha, TipoOperacion tipoOp, int linea, int columna) : base(linea, columna)
		{
			this.tipoOp = tipoOp;
			this.izquierda = izquierda;
			this.derecha = derecha;
			this.valor =false;
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

		public override TipoOperacion GetTipo(TablaSimbolos ts)
		{
			return TipoOperacion.Booleano;
		}

		public override object GetValor(TablaSimbolos ts)
		{
			Object izq = izquierda?.GetValor(ts);
			if (izq != null)
				if (izq.GetType() == typeof(ThrowError))
				{
					return izq;
				}
			Object der = derecha?.GetValor(ts);
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
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.String) && Derecha.GetTipo(ts).Equals(TipoOperacion.String))
						{
							bool valor = izq.ToString() != der.ToString();
							return valor;
						}else
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							bool valor = double.Parse(izq.ToString()) != double.Parse(der.ToString());
							return valor;
						}else
						//BOOLEANO-BOOLEANO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
						{
							bool valor = bool.Parse(izq.ToString()) != bool.Parse(der.ToString());
							return valor;
						}else
						//FECHA-FECHA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts).Equals(TipoOperacion.Fecha))
						{
							DateTime d1;
							DateTime d2;
							if (!DateTime.TryParse(izq.ToString().Replace("'", String.Empty), out d1))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La fecha '" + izq.ToString() + "' es incorrecta, el formato debe ser AAAA-MM-DD",
								   Linea, Columna);
							}
							if (!DateTime.TryParse(der.ToString().Replace("'", String.Empty), out d2))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La fecha '" + der.ToString() + "' es incorrecta, el formato debe ser AAAA-MM-DD",
								   Linea, Columna);
							}
							int valor = DateTime.Compare(d1, d2);
							return valor!=0;
						}else
						//HORA-HORA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Hora) && Derecha.GetTipo(ts).Equals(TipoOperacion.Hora))
						{
							DateTime d1;
							DateTime d2;
							if (!DateTime.TryParse(izq.ToString().Replace("'", String.Empty), out d1)) {
								return new ThrowError(TipoThrow.ArithmeticException,
									"La hora '"+izq.ToString()+"' es incorrecta, el formato debe ser HH:MM:SS a 24 horas",
								   Linea, Columna);
							}
							if (!DateTime.TryParse(der.ToString().Replace("'", String.Empty), out d2))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La hora '" + der.ToString() + "' es incorrecta, el formato debe ser HH:MM:SS a 24 horas",
								   Linea, Columna);
							}
							int valor = DateTime.Compare(d1, d2);
							return valor != 0;
						}
						//OBJETO-OBJETO **PENDIENTE
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden comparar los operandos de tipo " + izquierda.GetTipo(ts) + " y " + derecha.GetTipo(ts),
								   Linea, Columna);
						}
					case TipoOperacion.Igual:
						//STRING-STRING
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.String) && Derecha.GetTipo(ts).Equals(TipoOperacion.String))
						{
							bool valor = izq.ToString() == der.ToString();
							return valor;
						}else
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							bool valor = double.Parse(izq.ToString()) == double.Parse(der.ToString());
							return valor;
						}else
						//BOOLEANO-BOOLEANO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
						{
							bool valor = bool.Parse(izq.ToString()) == bool.Parse(der.ToString());
							return valor;
						}else
						//FECHA-FECHA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts).Equals(TipoOperacion.Fecha))
						{
							DateTime d1;
							DateTime d2;
							if (!DateTime.TryParse(izq.ToString().Replace("'", String.Empty), out d1))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La fecha '" + izq.ToString() + "' es incorrecta, el formato debe ser AAAA-MM-DD",
								   Linea, Columna);
							}
							if (!DateTime.TryParse(der.ToString().Replace("'", String.Empty), out d2))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La fecha '" + der.ToString() + "' es incorrecta, el formato debe ser AAAA-MM-DD",
								   Linea, Columna);
							}
							int valor = DateTime.Compare(d1, d2);
							return valor == 0;
						}else
						//HORA-HORA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Hora) && Derecha.GetTipo(ts).Equals(TipoOperacion.Hora))
						{
							DateTime d1;
							DateTime d2;
							if (!DateTime.TryParse(izq.ToString().Replace("'", String.Empty), out d1))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La hora '" + izq.ToString() + "' es incorrecta, el formato debe ser HH:MM:SS a 24 horas",
								   Linea, Columna);
							}
							if (!DateTime.TryParse(der.ToString().Replace("'", String.Empty), out d2))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La hora '" + der.ToString() + "' es incorrecta, el formato debe ser HH:MM:SS a 24 horas",
								   Linea, Columna);
							}
							int valor = DateTime.Compare(d1, d2);
							return valor == 0;
						}
						//OBJETO-OBJETO **PENDIENTE
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden comparar los operandos de tipo " + izquierda.GetTipo(ts) + " y " + derecha.GetTipo(ts),
								   Linea, Columna);
						}
					case TipoOperacion.Mayor:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							bool valor = double.Parse(izq.ToString()) > double.Parse(der.ToString());
							return valor;
						}else
						//FECHA-FECHA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts).Equals(TipoOperacion.Fecha))
						{
							DateTime d1;
							DateTime d2;
							if (!DateTime.TryParse(izq.ToString().Replace("'", String.Empty), out d1))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La fecha '" + izq.ToString() + "' es incorrecta, el formato debe ser AAAA-MM-DD",
								   Linea, Columna);
							}
							if (!DateTime.TryParse(der.ToString().Replace("'", String.Empty), out d2))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La fecha '" + der.ToString() + "' es incorrecta, el formato debe ser AAAA-MM-DD",
								   Linea, Columna);
							}
							int valor = DateTime.Compare(d1, d2);
							return valor > 0;
						}else
						//HORA-HORA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Hora) && Derecha.GetTipo(ts).Equals(TipoOperacion.Hora))
						{
							DateTime d1;
							DateTime d2;
							if (!DateTime.TryParse(izq.ToString().Replace("'", String.Empty), out d1))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La hora '" + izq.ToString() + "' es incorrecta, el formato debe ser HH:MM:SS a 24 horas",
								   Linea, Columna);
							}
							if (!DateTime.TryParse(der.ToString().Replace("'", String.Empty), out d2))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La hora '" + der.ToString() + "' es incorrecta, el formato debe ser HH:MM:SS a 24 horas",
								   Linea, Columna);
							}
							int valor = DateTime.Compare(d1, d2);
							return valor > 0;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden comparar los operandos de tipo " + izquierda.GetTipo(ts) + " y " + derecha.GetTipo(ts),
								   Linea, Columna);
						}
					case TipoOperacion.MayorIgual:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							bool valor = double.Parse(izq.ToString()) >= double.Parse(der.ToString());
							return valor;
						}
						else
						//FECHA-FECHA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts).Equals(TipoOperacion.Fecha))
						{
							DateTime d1;
							DateTime d2;
							if (!DateTime.TryParse(izq.ToString().Replace("'", String.Empty), out d1))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La fecha '" + izq.ToString() + "' es incorrecta, el formato debe ser AAAA-MM-DD",
								   Linea, Columna);
							}
							if (!DateTime.TryParse(der.ToString().Replace("'", String.Empty), out d2))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La fecha '" + der.ToString() + "' es incorrecta, el formato debe ser AAAA-MM-DD",
								   Linea, Columna);
							}
							int valor = DateTime.Compare(d1, d2);
							return valor >= 0;
						}
						else
						//HORA-HORA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Hora) && Derecha.GetTipo(ts).Equals(TipoOperacion.Hora))
						{
							DateTime d1;
							DateTime d2;
							if (!DateTime.TryParse(izq.ToString().Replace("'", String.Empty), out d1))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La hora '" + izq.ToString() + "' es incorrecta, el formato debe ser HH:MM:SS a 24 horas",
								   Linea, Columna);
							}
							if (!DateTime.TryParse(der.ToString().Replace("'", String.Empty), out d2))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La hora '" + der.ToString() + "' es incorrecta, el formato debe ser HH:MM:SS a 24 horas",
								   Linea, Columna);
							}
							int valor = DateTime.Compare(d1, d2);
							return valor >= 0;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden comparar los operandos de tipo " + izquierda.GetTipo(ts) + " y " + derecha.GetTipo(ts),
								   Linea, Columna);
						}
					case TipoOperacion.Menor:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							bool valor = double.Parse(izq.ToString()) < double.Parse(der.ToString());
							return valor;
						}
						else
						//FECHA-FECHA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts).Equals(TipoOperacion.Fecha))
						{
							DateTime d1;
							DateTime d2;
							if (!DateTime.TryParse(izq.ToString().Replace("'", String.Empty), out d1))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La fecha '" + izq.ToString() + "' es incorrecta, el formato debe ser AAAA-MM-DD",
								   Linea, Columna);
							}
							if (!DateTime.TryParse(der.ToString().Replace("'", String.Empty), out d2))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La fecha '" + der.ToString() + "' es incorrecta, el formato debe ser AAAA-MM-DD",
								   Linea, Columna);
							}
							int valor = DateTime.Compare(d1, d2);
							return valor < 0;
						}
						else
						//HORA-HORA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Hora) && Derecha.GetTipo(ts).Equals(TipoOperacion.Hora))
						{
							DateTime d1;
							DateTime d2;
							if (!DateTime.TryParse(izq.ToString().Replace("'", String.Empty), out d1))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La hora '" + izq.ToString() + "' es incorrecta, el formato debe ser HH:MM:SS a 24 horas",
								   Linea, Columna);
							}
							if (!DateTime.TryParse(der.ToString().Replace("'", String.Empty), out d2))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La hora '" + der.ToString() + "' es incorrecta, el formato debe ser HH:MM:SS a 24 horas",
								   Linea, Columna);
							}
							int valor = DateTime.Compare(d1, d2);
							return valor < 0;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden comparar los operandos de tipo " + izquierda.GetTipo(ts) + " y " + derecha.GetTipo(ts),
								   Linea, Columna);
						}
					case TipoOperacion.MenorIgual:
						//NUMERO-NUMERO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Numero) && Derecha.GetTipo(ts).Equals(TipoOperacion.Numero))
						{
							bool valor = double.Parse(izq.ToString()) <=double.Parse(der.ToString());
							return valor;
						}
						else
						//FECHA-FECHA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Fecha) && Derecha.GetTipo(ts).Equals(TipoOperacion.Fecha))
						{
							DateTime d1;
							DateTime d2;
							if (!DateTime.TryParse(izq.ToString().Replace("'", String.Empty), out d1))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La fecha '" + izq.ToString() + "' es incorrecta, el formato debe ser AAAA-MM-DD",
								   Linea, Columna);
							}
							if (!DateTime.TryParse(der.ToString().Replace("'", String.Empty), out d2))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La fecha '" + der.ToString() + "' es incorrecta, el formato debe ser AAAA-MM-DD",
								   Linea, Columna);
							}
							int valor = DateTime.Compare(d1, d2);
							return valor <= 0;
						}
						else
						//HORA-HORA
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Hora) && Derecha.GetTipo(ts).Equals(TipoOperacion.Hora))
						{
							DateTime d1;
							DateTime d2;
							if (!DateTime.TryParse(izq.ToString().Replace("'", String.Empty), out d1))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La hora '" + izq.ToString() + "' es incorrecta, el formato debe ser HH:MM:SS a 24 horas",
								   Linea, Columna);
							}
							if (!DateTime.TryParse(der.ToString().Replace("'", String.Empty), out d2))
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"La hora '" + der.ToString() + "' es incorrecta, el formato debe ser HH:MM:SS a 24 horas",
								   Linea, Columna);
							}
							int valor = DateTime.Compare(d1, d2);
							return valor <= 0;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se pueden comparar los operandos de tipo " + izquierda.GetTipo(ts) + " y " + derecha.GetTipo(ts),
								   Linea, Columna);
						}
					case TipoOperacion.Or:
						//BOOLEANO-BOOLEANO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
						{
							bool valor = bool.Parse(izq.ToString()) || bool.Parse(der.ToString());
							return valor;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se puede aplicar OR a los operandos de tipo " + izquierda.GetTipo(ts) + " y " + derecha.GetTipo(ts),
								   Linea, Columna);
						}
					case TipoOperacion.And:
						//BOOLEANO-BOOLEANO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
						{
							bool valor = bool.Parse(izq.ToString()) && bool.Parse(der.ToString());
							return valor;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se puede aplicar OR a los operandos de tipo " + izquierda.GetTipo(ts) + " y " + derecha.GetTipo(ts),
								   Linea, Columna);
						}
					case TipoOperacion.Xor:
						//BOOLEANO-BOOLEANO
						if (Izquierda.GetTipo(ts).Equals(TipoOperacion.Booleano) && Derecha.GetTipo(ts).Equals(TipoOperacion.Booleano))
						{
							bool valor = bool.Parse(izq.ToString()) == bool.Parse(der.ToString());
							return !valor;
						}
						else
						{
							return new ThrowError(TipoThrow.ArithmeticException,
									"No se puede aplicar OR a los operandos de tipo " + izquierda.GetTipo(ts) + " y " + derecha.GetTipo(ts),
								   Linea, Columna);
						}
				}
			}//operaciones de tipo unarias
			else if (izq != null)
			{
				if (tipoOp == TipoOperacion.Not)
				{
					if (izquierda.GetTipo(ts) == TipoOperacion.Booleano)
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
