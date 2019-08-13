using com.Analisis;
using com.Analisis.Util;
using Irony.Parsing;
using Proyecto1Compi2.com.AST;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Analisis
{
	static class GeneradorAstCql
	{

		public static NodoAST GetAST(ParseTreeNode raiz)
		{
			GetSentencias(raiz.ChildNodes.ElementAt(0));
			return null;
		}

		private static List<NodoAST> GetSentencias(ParseTreeNode parseTreeNode)
		{
			List<NodoAST> sentencias = new List<NodoAST>();
			foreach (ParseTreeNode sentencia in parseTreeNode.ChildNodes) {
				switch (sentencia.Term.Name) {
					case "CREAR_DB":
						NodoAST n= GetCrearDB(sentencia);
						if(n!=null)sentencias.Add(n);
						break;
					case "USAR_DB":
						n = GetUsarDB(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "ELIMINAR_DB":
						n = GetEliminarDB(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "ALTERAR_TABLA":
						n = GetModificarTabla(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "ELIMINAR_TABLA":
						n = GetEliminarTabla(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "TRUNCAR_TABLA":
						n = GetTruncarTabla(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "ALTERAR_USERTYPE":
						n = GetModificarUserType(sentencia);
						if (n != null) sentencias.Add(n);
						break;
				}
			}
			return sentencias;
		}

		private static NodoAST GetModificarUserType(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 3)
			{
				//add
				Dictionary<string, TipoObjetoDB> atributos = new Dictionary<string, TipoObjetoDB>();
				foreach (ParseTreeNode nodo in sentencia.ChildNodes.ElementAt(2).ChildNodes) {
					TipoDatoDB tipo = GeneradorDB.GetTipo(nodo.ChildNodes.ElementAt(1));
					string nombreTipo = GeneradorDB.GetNombreTipo(tipo,nodo.ChildNodes.ElementAt(1));
					try {
						atributos.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString, new TipoObjetoDB(tipo,nombreTipo));
					}
					catch (ArgumentException ex) {
						Analizador.ErroresCQL.Add(new Error(TipoError.Semantico,"No se pueden agregar dos atributos con el mismo nombre",
							nodo.ChildNodes.ElementAt(0).Token.Location.Line,
							nodo.ChildNodes.ElementAt(0).Token.Location.Column));
					}
				}
				return new AlterarUserType(TipoAccion.Agregar, sentencia.ChildNodes.ElementAt(0).Token.ValueString,
					atributos, sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else {
				//borrar

			}
			throw new NotImplementedException();
		}

		private static NodoAST GetTruncarTabla(ParseTreeNode sentencia)
		{
			return new TruncarTabla(sentencia.ChildNodes.ElementAt(0).Token.ValueString,
				sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
		}

		private static NodoAST GetEliminarTabla(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 1)
			{
				//sin ifExists
				return new EliminarTabla(sentencia.ChildNodes.ElementAt(0).Token.ValueString, false,
					sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else {
				//con ifExists
				return new EliminarTabla(sentencia.ChildNodes.ElementAt(1).Token.ValueString, true,
					sentencia.ChildNodes.ElementAt(1).Token.Location.Line, sentencia.ChildNodes.ElementAt(1).Token.Location.Column);
			}
		}

		private static NodoAST GetModificarTabla(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 3)
			{
				//add
				List<object> cols = GetColumnasTabla(sentencia.ChildNodes.ElementAt(2));
				List<Columna> columnas = new List<Columna>();
				foreach (object ob in cols) {
					if (ob.GetType()==typeof(Columna)) {
						columnas.Add((Columna)ob);
					}
				}
				if (cols.Count!=columnas.Count) {
					Analizador.ErroresCQL.Add(new Error(TipoError.Semantico,"No se pueden agregar llaves primarias a una tabla existente",
						sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column));
					//RETORNAR NULL SI SE DEBE ALTERAR TODA LA INSTRUCCION
				}
				return new AlterarTabla(TipoAccion.Agregar, sentencia.ChildNodes.ElementAt(0).Token.ValueString, columnas,
					sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else {
				//drop
				List<string> columnas = GetListaStrings(sentencia.ChildNodes.ElementAt(1));

				return new AlterarTabla(TipoAccion.Quitar, sentencia.ChildNodes.ElementAt(0).Token.ValueString, columnas,
					sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
		}

		private static NodoAST GetCrearTabla(ParseTreeNode sentencia)
		{
			Boolean ifexist = sentencia.ChildNodes.Count == 2;
				List<object> cls = GetColumnasTabla(sentencia.ChildNodes.ElementAt(1));
				 return new CrearTabla(sentencia.ChildNodes.ElementAt(1).Token.ValueString, cls, ifexist, sentencia.ChildNodes.ElementAt(1).Token.Location.Line,
					sentencia.ChildNodes.ElementAt(1).Token.Location.Column);
		}

		private static List<object> GetColumnasTabla(ParseTreeNode parseTreeNode)
		{

			List<object> cols = new List<object>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes) {
				object cl = GetColumna(nodo);
				if (cl != null)
				{
					cols.Add(cl);
				}
			}
			return cols;
		}

		private static object GetColumna(ParseTreeNode nodo)
		{
			////no es llave primaria
			if (nodo.ChildNodes.Count == 2)
			{
				//nombre -- tipo
				TipoDatoDB t = GeneradorDB.GetTipo(nodo.ChildNodes.ElementAt(1));
				string nombreTipo = GeneradorDB.GetNombreTipo(t, nodo.ChildNodes.ElementAt(1));
				return new Columna(nodo.ChildNodes.ElementAt(0).Token.ValueString, new TipoObjetoDB(t, nombreTipo), false);
			} else if (nodo.ChildNodes.Count==3) {
				//llave primaria compuesta
				return GetListaStrings(nodo.ChildNodes.ElementAt(2));
			}
			else if (nodo.ChildNodes.Count == 4) {
				//columna con llave primaria
				TipoDatoDB t = GeneradorDB.GetTipo(nodo.ChildNodes.ElementAt(1));
				string nombreTipo = GeneradorDB.GetNombreTipo(t, nodo.ChildNodes.ElementAt(1));
				return new Columna(nodo.ChildNodes.ElementAt(0).Token.ValueString, new TipoObjetoDB(t, nombreTipo), true);
			}

			return null;
		}

		private static List<string> GetListaStrings(ParseTreeNode lista)
		{
			List<string> nombres = new List<string>();
			foreach (ParseTreeNode nodito in lista.ChildNodes)
			{
				nombres.Add(nodito.Token.ValueString);
			}
			return nombres;
		}

		private static NodoAST GetEliminarDB(ParseTreeNode sentencia)
		{
			return new EliminarBaseDatos(sentencia.ChildNodes.ElementAt(0).Token.ValueString, sentencia.ChildNodes.ElementAt(0).Token.Location.Line,
				sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
		}

		private static NodoAST GetUsarDB(ParseTreeNode sentencia)
		{
			return new UsarBaseDatos(sentencia.ChildNodes.ElementAt(0).Token.ValueString, sentencia.ChildNodes.ElementAt(0).Token.Location.Line,
				sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
		}

		private static NodoAST GetCrearDB(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 1)
			{
				return new CrearBaseDatos(sentencia.ChildNodes.ElementAt(0).Token.ValueString, false,
					sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else {
				return new CrearBaseDatos(sentencia.ChildNodes.ElementAt(1).Token.ValueString, true,
					sentencia.ChildNodes.ElementAt(1).Token.Location.Line, sentencia.ChildNodes.ElementAt(1).Token.Location.Column);
			}
		}

		#region Expresion
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
				case "**": return TipoOperacion.Potencia;
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
		#endregion
	}
}
