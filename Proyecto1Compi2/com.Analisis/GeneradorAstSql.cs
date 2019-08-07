using Irony.Parsing;
using Proyecto1Compi2.com.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Analisis
{
	static class GeneradorAstSql
	{

		public static NodoAST GetAST(ParseTreeNode raiz)
		{
			return GetExpresion(raiz.ChildNodes.ElementAt(0));
		}

		private static Expresion GetExpresion(ParseTreeNode raiz)
		{
			switch (raiz.ChildNodes.Count) {
				case 1://valor o condicion
					if (raiz.ChildNodes.ElementAt(0).Term.Name.Equals("VALOR")) {
						return GetValor(raiz.ChildNodes.ElementAt(0));
					}
					if (raiz.ChildNodes.ElementAt(0).Term.Name.Equals("CONDICION"))
					{
						return GetCondicion(raiz.ChildNodes.ElementAt(0));
					}
					break;
				case 3: //operaciones 
					return new Operacion(GetExpresion(raiz.ChildNodes.ElementAt(0)),GetExpresion(raiz.ChildNodes.ElementAt(2)),GetTipoOperacion(raiz.ChildNodes.ElementAt(1)),raiz.ChildNodes.ElementAt(1).Token.Location.Line,raiz.ChildNodes.ElementAt(1).Token.Location.Column);
			}
			return null;
		}

		private static Expresion GetCondicion(ParseTreeNode raiz)
		{
			switch (raiz.ChildNodes.Count) {
				case 1://true o false
					return new Operacion(raiz.ChildNodes.ElementAt(0).Token.ValueString.ToLower(), TipoOperacion.Booleano, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
				case 2://not
					return new Condicion(GetCondicion(raiz.ChildNodes.ElementAt(1)),TipoOperacion.Not, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
				case 3://operaciones
					return new Operacion(GetExpresion(raiz.ChildNodes.ElementAt(0)), GetExpresion(raiz.ChildNodes.ElementAt(2)), GetTipoOperacion(raiz.ChildNodes.ElementAt(1)), raiz.ChildNodes.ElementAt(1).Token.Location.Line, raiz.ChildNodes.ElementAt(1).Token.Location.Column);
			}
			return null;
		}

		private static TipoOperacion GetTipoOperacion(ParseTreeNode parseTreeNode)
		{
			switch (parseTreeNode.Token.Value) {
				case "+":return TipoOperacion.Suma;
				case "-": return TipoOperacion.Resta;
				case "*": return TipoOperacion.Multiplicacion;
				case "/": return TipoOperacion.Division;
				case "^": return TipoOperacion.Potencia;
			}
			return TipoOperacion.Nulo;
		}
		
		private static Expresion GetValor(ParseTreeNode raiz)
		{
			switch(raiz.ChildNodes.Count) {
				case 1://valores, o expresion o llamada
					switch (raiz.ChildNodes.ElementAt(0).Term.Name) {
					case "numero":
							return new Operacion(raiz.ChildNodes.ElementAt(0).Token.ValueString, TipoOperacion.Numero,raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
					case "cadena":
							return new Operacion(raiz.ChildNodes.ElementAt(0).Token.ValueString, TipoOperacion.Cadena, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
					case "id":
						return new Operacion(raiz.ChildNodes.ElementAt(0).Token.ValueString, TipoOperacion.Identificador, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
					case "nombre":
						return new Operacion(raiz.ChildNodes.ElementAt(0).Token.ValueString, TipoOperacion.Identificador, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
					case "date":
						return new Operacion(raiz.ChildNodes.ElementAt(0).Token.ValueString, TipoOperacion.Fecha, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
					case "datetime":
						return new Operacion(raiz.ChildNodes.ElementAt(0).Token.ValueString, TipoOperacion.FechaHora, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
					case "EXPRESION":
						return GetExpresion(raiz.ChildNodes.ElementAt(0));
					case "LLAMADAFUNCION":
						break;
					}
					break;
				case 2://menos
					if (raiz.ChildNodes.ElementAt(0).Term.Name.Equals("menos"))
					{
						return new Operacion(GetExpresion(raiz.ChildNodes.ElementAt(0)), TipoOperacion.Menos, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
					}
					else {
						///acceso a propiedad
					}
					break;
					
			}
			return null;
		}
	}
}
