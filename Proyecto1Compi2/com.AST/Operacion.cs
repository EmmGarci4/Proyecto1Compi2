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
		private readonly string valor;
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
			this.valor = null;
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
			this.valor = valor;
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
					return this.tipoOp;
				case TipoOperacion.Suma:
				case TipoOperacion.Resta:
				case TipoOperacion.Division:
				case TipoOperacion.Multiplicacion:
					return izquierda.GetTipo(ts);
			}
			return TipoOperacion.Nulo;
		}

		public override object GetValor(TablaSimbolos ts)
		{
			Object izq = izquierda?.GetValor(ts);
			Object der = derecha?.GetValor(ts);
			//operaciones de tipo binarias
			if (izq != null && der != null)
			{
				switch (tipoOp)
				{
					case TipoOperacion.Suma:
						//numeros
						if (izquierda.GetTipo(ts) == TipoOperacion.Numero && derecha.GetTipo(ts) == TipoOperacion.Numero)
						{
							double valor = double.Parse(izq.ToString()) + double.Parse(der.ToString());
							return valor.ToString();
						}
						else
						{
						//	Analizador.Errores.Add(new Error(TipoError.Semantico,"No se pueden sumar los operandos",linea,columna);
							break;
						}
					case TipoOperacion.Resta:
						//numeros
						if (izquierda.GetTipo(ts) == TipoOperacion.Numero && derecha.GetTipo(ts) == TipoOperacion.Numero)
						{
							double valor = double.Parse(izq.ToString()) - double.Parse(der.ToString());
							return valor.ToString();
						}
						else
						{
							Console.WriteLine("Error de tipos");
							break;
						}
					case TipoOperacion.Multiplicacion:
						//numeros
						if (izquierda.GetTipo(ts) == TipoOperacion.Numero && derecha.GetTipo(ts) == TipoOperacion.Numero)
						{
							double valor = double.Parse(izq.ToString()) * double.Parse(der.ToString());
							return valor.ToString();
						}
						else
						{
							Console.WriteLine("Error de tipos");
							break;
						}
					case TipoOperacion.Division:
						//numeros
						if (izquierda.GetTipo(ts) == TipoOperacion.Numero && derecha.GetTipo(ts) == TipoOperacion.Numero)
						{
							try
							{
								double valor = double.Parse(izq.ToString()) / double.Parse(der.ToString());
								return valor.ToString();
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
								break;
							}
						}
						else
						{
							Console.WriteLine("Error de tipos");
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
				return this.valor;
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
