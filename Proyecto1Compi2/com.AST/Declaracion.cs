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
				//VALIDAR TIPOS
				object nuevoValor = expresion.GetValor(tb);
				if (!Datos.IsTipoCompatibleParaAsignar(tipo,nuevoValor ))
				{
					return new ThrowError(TipoThrow.Exception,
						"Un valor tipo '" +expresion.GetValor(tb) + "' no se puede asignar a una variable tipo '" + tipo.ToString() + "'",
						Linea, Columna);
				}
			}

			//AGREGANDO A TABLA DDE SIMBOLOS
			int contador = 0;
			foreach (string variable in variables)
			{
				if (contador == variables.Count - 1 && expresion!=null)
				{
					object nuevaRespuesta = Datos.CasteoImplicito(tipo.Tipo, expresion.GetValor(tb));

					tb.AgregarSimbolo(new Simbolo(variable, nuevaRespuesta, tipo, Linea, Columna));
				}
				else {
					tb.AgregarSimbolo(new Simbolo(variable, GetValorPredeterminado(), tipo, Linea, Columna));
				}
				contador++;
			}
			return null;
		}

		private object GetValorPredeterminado()
		{
			switch (this.Tipo.Tipo) {
				case TipoDatoDB.BOOLEAN:
					return false;
				case TipoDatoDB.DOUBLE:
					return 0.0;
				case TipoDatoDB.COUNTER:
				case TipoDatoDB.INT:
					return 0;
				case TipoDatoDB.LISTA_OBJETO:
				case TipoDatoDB.LISTA_PRIMITIVO:
				case TipoDatoDB.MAP_OBJETO:
				case TipoDatoDB.MAP_PRIMITIVO:
				case TipoDatoDB.OBJETO:
				case TipoDatoDB.SET_OBJETO:
				case TipoDatoDB.DATE:
				case TipoDatoDB.SET_PRIMITIVO:
				case TipoDatoDB.STRING:
				case TipoDatoDB.TIME:
					return "null";
				default:
					return null;
			}
		}
	}
}
