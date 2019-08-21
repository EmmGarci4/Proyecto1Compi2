using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class ModificadorExp : Expresion
	{
		string variable;
		bool sumar;
		Acceso acceso;

		public ModificadorExp(string variable, bool sumar, int linea, int columna) : base(linea, columna)
		{
			this.variable = variable;
			this.sumar = sumar;
			this.acceso = null;
		}

		public ModificadorExp(Acceso variable, bool sumar, int linea, int columna) : base(linea, columna)
		{
			this.acceso = variable;
			this.sumar = sumar;
			this.variable = null;
		}

		public string Variable { get => variable; set => variable = value; }
		public bool Sumar { get => sumar; set => sumar = value; }
		internal Acceso Acceso { get => acceso; set => acceso = value; }

		public override TipoOperacion GetTipo(TablaSimbolos ts)
		{
			if (this.variable != null)
			{
				//buscar variable en ts
				if (ts.ExisteSimbolo(this.variable))
				{
					Simbolo s = ts.GetSimbolo(this.variable);
					return Datos.GetTipoDatoDB(s.TipoDato.Tipo);
				}
			}
			else
			{
				//buscar acceso
			}
			return TipoOperacion.Nulo;
		}

		public override object GetValor(TablaSimbolos ts)
		{
			if (this.variable != null)
			{
				//buscar variable en ts
				if (ts.ExisteSimbolo(this.variable))
				{
					Simbolo s = ts.GetSimbolo(this.variable);
					object valor = s.Valor;
					if (valor != null)
					{
						if (s.TipoDato.Tipo == Util.TipoDatoDB.INT)
						{

							if (sumar)
							{
								s.Valor = (int)valor + 1;
							}
							else {
								s.Valor = (int)valor - 1;
							}
							return valor;
						}
						else if (s.TipoDato.Tipo == Util.TipoDatoDB.DOUBLE)
						{
							if (sumar)
							{
								s.Valor = (double)valor + 1;
							}
							else {
								s.Valor = (double)valor - 1;
							}
							return valor;
						}
						else
						{
							return new ThrowError(Util.TipoThrow.ArithmeticException,
											"la variable '" + this.variable + "' no se puede aumentar en valor",
											Linea, Columna);
						}

					}
					else {
						return new ThrowError(Util.TipoThrow.ArithmeticException,
											"la variable '" + this.variable + "' no se ha inicializado",
											Linea, Columna);
					}

				}
				else
				{
					return new ThrowError(Util.TipoThrow.ArithmeticException,
										"la variable '" + this.variable + "' no existe",
										Linea, Columna);
				}
			}
			else
			{
				//buscar acceso
			}
			return null;
		}
	}
}
