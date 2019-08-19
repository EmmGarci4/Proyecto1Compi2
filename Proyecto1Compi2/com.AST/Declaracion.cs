using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Declaracion:Sentencia
	{
		List<string> variables;
		TipoObjetoDB tipo;
		Expresion expresion;

		public Declaracion(List<string> variables, TipoObjetoDB tipo, Expresion expresion,int linea,int columna):base(linea,columna)
		{
			this.variables = variables;
			this.tipo = tipo;
			this.expresion = expresion;
		}

		public List<string> Variables { get => variables; set => variables = value; }
		public TipoObjetoDB Tipo { get => tipo; set => tipo = value; }
		internal Expresion Expresion { get => expresion; set => expresion = value; }

		public override object Ejecutar(Sesion sesion, TablaSimbolos tb)
		{
			//VALIDANDO
			foreach (string variable in variables) {
				if (tb.ExisteSimboloEnAmbito(variable)) {
					return new ThrowError(TipoThrow.Exception,
						"La variable '"+variable+"' ya existe",
						Linea,Columna);
				}
			}
			if (expresion != null)
			{
				if (!Datos.IsTipoCompatible(tipo, expresion.GetValor(tb)))
				{
					return new ThrowError(TipoThrow.Exception,
						"El valor '" + expresion.GetValor(tb) + "' no se puede asignar al tipo '" + tipo.ToString() + "'",
						Linea, Columna);
				}
			}

			//AGREGANDO A TABLA DDE SIMBOLOS
			int contador = 0;
			foreach (string variable in variables)
			{
				if (contador == variables.Count - 1)
				{
					tb.AgregarSimbolo(new Simbolo(variable, expresion.GetValor(tb), tipo, Linea, Columna));
				}
				else {
					tb.AgregarSimbolo(new Simbolo(variable, null, tipo, Linea, Columna));
				}
				contador++;
			}
			return null;
		}
	}
}
