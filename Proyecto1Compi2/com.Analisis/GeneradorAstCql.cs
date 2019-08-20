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

		public static List<Sentencia> GetAST(ParseTreeNode raiz)
		{
			return GetSentencias(raiz.ChildNodes.ElementAt(0));
		}

		//public static Expresion GetAST(ParseTreeNode raiz)
		//{
		//	return GetExpresion(raiz.ChildNodes.ElementAt(0));
		//}

		private static List<Sentencia> GetSentencias(ParseTreeNode parseTreeNode)
		{
			List<Sentencia> sentencias = new List<Sentencia>();
			foreach (ParseTreeNode sentencia in parseTreeNode.ChildNodes)
			{
				switch (sentencia.Term.Name)
				{
					case "CREAR_DB":
						Sentencia n = GetCrearDB(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "USAR_DB":
						n = GetUsarDB(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "ELIMINAR_DB":
						n = GetEliminarDB(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "CREAR_TABLA":
						n = GetCrearTabla(sentencia);
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
					case "ELIMINAR_USERTYPE":
						n = GetEliminarUserType(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "COMMIT":
						sentencias.Add(new Commit(sentencia.Span.Location.Line,
							sentencia.Span.Location.Column));
						break;
					case "ROLLBACK":
						sentencias.Add(new Rollback(sentencia.Span.Location.Line,
							sentencia.Span.Location.Column));
						break;
					case "CREAR_USUARIO":
						sentencias.Add(GetCrearUsuario(sentencia));
						break;
					case "OTORGAR":
						sentencias.Add(GetOtorgarpermiso(sentencia));
						break;
					case "DENEGAR":
						sentencias.Add(GetDenegarPermiso(sentencia));
						break;
					case "INSERTAR":
						n = GetInsertar(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "ACTUALIZAR":
						n = GetActualizar(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "BORRAR":
						n = GetBorrar(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "SELECCIONAR":
						n = GetSeleccionar(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "BATCH":
						n = GetBatch(sentencia.ChildNodes.ElementAt(0));
						if (n != null) sentencias.Add(n);
						break;
					case "FUNCIONAGREGACION":
						n = GetFuncionAgregacion(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "CREAR_USERTYPE":
						n = GetCrearUserType(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					//SENTENCIAS FCL
					case "IF":
						n = GetIf(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "LOG":
						n = GetLog(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "OPERACIONASIGNACION":
						n = GetOperacionAsignacion(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "MODIFICADORES":
						n =GetModificador(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "WHILE":
						n = GetWhile(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "DOWHILE":
						n = GetDoWhile(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "DECLARACION":
						n = GetDeclaracion(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "ASIGNACION":
						n = GetAsignacion(sentencia);
						if (n != null) sentencias.Add(n);
						break;
				}
			}
			return sentencias;
		}

		private static Sentencia GetAsignacion(ParseTreeNode sentencia)
		{
			Acceso ac2 = new Acceso(sentencia.Span.Location.Line, sentencia.Span.Location.Column);
			Expresion exp = null;
			if (sentencia.ChildNodes.Count == 2)
			{
				ac2.Objetos.Enqueue(new AccesoPar(sentencia.ChildNodes.ElementAt(0).Token.ValueString, TipoAcceso.Variable));
				 exp = GetExpresion(sentencia.ChildNodes.ElementAt(1));
				
			}
			else {
				if (sentencia.ChildNodes.ElementAt(1).Term.Name == "ACCESO")
				{
					Acceso ac = GetAcceso(sentencia.ChildNodes.ElementAt(1));
					ac2.Objetos.Enqueue(new AccesoPar(sentencia.ChildNodes.ElementAt(0).Token.ValueString,TipoAcceso.Variable));
					while (ac.Objetos.Count > 0)
					{
						ac2.Objetos.Enqueue(ac.Objetos.Dequeue());
					}
					exp = GetExpresion(sentencia.ChildNodes.ElementAt(2));
				}
				else {
					//acceso a algo []
					ac2.Objetos.Enqueue(new AccesoPar(
						new AccesoArreglo(GetExpresion(sentencia.ChildNodes.ElementAt(1)), sentencia.ChildNodes.ElementAt(0).Token.ValueString,
						sentencia.ChildNodes.ElementAt(0).Token.Location.Line,
						sentencia.ChildNodes.ElementAt(0).Token.Location.Column),
						TipoAcceso.AccesoArreglo));
					exp = GetExpresion(sentencia.ChildNodes.ElementAt(2));
				}
			}
			return new Asignacion(ac2, exp, sentencia.Span.Location.Line, sentencia.Span.Location.Column);
		}

		private static Sentencia GetDeclaracion(ParseTreeNode sentencia)
		{
			TipoDatoDB tipo = GeneradorDB.GetTipo(sentencia.ChildNodes.ElementAt(0));
			string nombreTipo = GeneradorDB.GetNombreTipo(tipo, sentencia.ChildNodes.ElementAt(0), true);
			List<string> variables = GetListaStrings(sentencia.ChildNodes.ElementAt(1));
			Expresion exp = null;
			if (sentencia.ChildNodes.Count == 3)
			{
				exp = GetExpresion(sentencia.ChildNodes.ElementAt(2));
			}
			return new Declaracion(variables,new TipoObjetoDB(tipo,nombreTipo),exp,sentencia.Span.Location.Line,sentencia.Span.Location.Column);
		}

		private static Sentencia GetDoWhile(ParseTreeNode sentencia)
		{
			return new DoWhile(GetCondicion(sentencia.ChildNodes.ElementAt(1)), GetSentencias(sentencia.ChildNodes.ElementAt(0)),
				sentencia.Span.Location.Line, sentencia.Span.Location.Column);
		}

		private static Sentencia GetWhile(ParseTreeNode sentencia)
		{
			return new While(GetCondicion(sentencia.ChildNodes.ElementAt(0)),GetSentencias(sentencia.ChildNodes.ElementAt(1)),
				sentencia.Span.Location.Line,sentencia.Span.Location.Column); 
		}

		private static Sentencia GetModificador(ParseTreeNode sentencia)
		{
			return new Modificador(GetmodificadorExp(sentencia));
		}

		private static Sentencia GetOperacionAsignacion(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 3)
			{
				//variable
				Acceso ac = new Acceso(sentencia.Span.Location.Line, sentencia.Span.Location.Column);
				ac.Objetos.Enqueue(new AccesoPar(sentencia.ChildNodes.ElementAt(0).Token.ValueString,TipoAcceso.Variable));
				switch (sentencia.ChildNodes.ElementAt(1).Token.ValueString)
				{
					case "+":
						return new OperacionAsignacion(ac, TipoOperacion.Suma, GetExpresion(sentencia.ChildNodes.ElementAt(2)), sentencia.Span.Location.Line, sentencia.Span.Location.Column);
					case "-":
						return new OperacionAsignacion(ac, TipoOperacion.Resta, GetExpresion(sentencia.ChildNodes.ElementAt(2)), sentencia.Span.Location.Line, sentencia.Span.Location.Column);
					case "/":
						return new OperacionAsignacion(ac, TipoOperacion.Division, GetExpresion(sentencia.ChildNodes.ElementAt(2)), sentencia.Span.Location.Line, sentencia.Span.Location.Column);
					case "*":
						return new OperacionAsignacion(ac, TipoOperacion.Multiplicacion, GetExpresion(sentencia.ChildNodes.ElementAt(2)), sentencia.Span.Location.Line, sentencia.Span.Location.Column);
				}
			}
			else
			{
				//variable y acceso
				Acceso ac = GetAcceso(sentencia.ChildNodes.ElementAt(1));

				Acceso ac2 = new Acceso(sentencia.Span.Location.Line, sentencia.Span.Location.Column);
				ac2.Objetos.Enqueue(new AccesoPar(sentencia.ChildNodes.ElementAt(0).Token.ValueString,TipoAcceso.Variable));
				while (ac.Objetos.Count>0) {
					ac2.Objetos.Enqueue(ac.Objetos.Dequeue());
				}
				switch (sentencia.ChildNodes.ElementAt(2).Token.ValueString)
				{
					case "+":
						return new OperacionAsignacion(ac2, TipoOperacion.Suma, GetExpresion(sentencia.ChildNodes.ElementAt(3)), sentencia.Span.Location.Line, sentencia.Span.Location.Column);
					case "-":
						return new OperacionAsignacion(ac2, TipoOperacion.Resta, GetExpresion(sentencia.ChildNodes.ElementAt(3)), sentencia.Span.Location.Line, sentencia.Span.Location.Column);
					case "/":
						return new OperacionAsignacion(ac2, TipoOperacion.Division, GetExpresion(sentencia.ChildNodes.ElementAt(3)), sentencia.Span.Location.Line, sentencia.Span.Location.Column);
					case "*":
						return new OperacionAsignacion(ac2, TipoOperacion.Multiplicacion, GetExpresion(sentencia.ChildNodes.ElementAt(3)), sentencia.Span.Location.Line, sentencia.Span.Location.Column);
				}
			}
			return null;
		}

		private static Sentencia GetLog(ParseTreeNode sentencia)
		{
			return new Log(GetExpresion(sentencia.ChildNodes.ElementAt(0)), sentencia.Span.Location.Line, sentencia.Span.Location.Column);
		}

		private static Sentencia GetIf(ParseTreeNode sentencia)
		{
			switch (sentencia.ChildNodes.Count)
			{
				case 2: //if normal
					Condicion condicion = GetCondicion(sentencia.ChildNodes.ElementAt(0));
					List<Sentencia> sentencias = GetSentencias(sentencia.ChildNodes.ElementAt(1));
					return new If(condicion, null, sentencias, null, sentencia.Span.Location.Line, sentencia.Span.Location.Column);
				case 3:
					//if elseifs sin else 
					condicion = GetCondicion(sentencia.ChildNodes.ElementAt(0));
					sentencias = GetSentencias(sentencia.ChildNodes.ElementAt(1));
					List<ElseIf> elseifs = null;
					List<Sentencia> sentenclasElse = null;
					if (sentencia.ChildNodes.ElementAt(2).Term.Name == "ELSEIFS")
					{
						elseifs = GetElseIfs(sentencia.ChildNodes.ElementAt(2));
					}
					else
					{
						sentenclasElse = GetSentencias(sentencia.ChildNodes.ElementAt(2));
					}
					return new If(condicion, elseifs, sentencias, sentenclasElse, sentencia.Span.Location.Line, sentencia.Span.Location.Column);
				case 4://if elseifs con else 
					condicion = GetCondicion(sentencia.ChildNodes.ElementAt(0));
					sentencias = GetSentencias(sentencia.ChildNodes.ElementAt(1));
					elseifs = GetElseIfs(sentencia.ChildNodes.ElementAt(2));
					sentenclasElse = GetSentencias(sentencia.ChildNodes.ElementAt(3));
					return new If(condicion, elseifs, sentencias, sentenclasElse, sentencia.Span.Location.Line, sentencia.Span.Location.Column);
			}
			return null;
		}

		private static List<ElseIf> GetElseIfs(ParseTreeNode parseTreeNode)
		{
			List<ElseIf> lista = new List<ElseIf>();

			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
			{
				lista.Add(GetElseIf(nodo));
			}
			return lista;
		}

		private static ElseIf GetElseIf(ParseTreeNode nodo)
		{
			Condicion condicion = GetCondicion(nodo.ChildNodes.ElementAt(0));
			List<Sentencia> sentencias = GetSentencias(nodo.ChildNodes.ElementAt(1));
			return new ElseIf(condicion, sentencias, nodo.Span.Location.Line, nodo.Span.Location.Column);
		}

		#region Sentencias DB
		private static Sentencia GetCrearUserType(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 2)
			{
				Dictionary<string, TipoObjetoDB> atributos = new Dictionary<string, TipoObjetoDB>();
				foreach (ParseTreeNode nodo in sentencia.ChildNodes.ElementAt(1).ChildNodes)
				{
					TipoDatoDB tipo = GeneradorDB.GetTipo(nodo.ChildNodes.ElementAt(1));
					string nombreTipo = GeneradorDB.GetNombreTipo(tipo, nodo.ChildNodes.ElementAt(1), true);
					try
					{
						atributos.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString, new TipoObjetoDB(tipo, nombreTipo));
					}
					catch (ArgumentException ex)
					{
						Analizador.ErroresCQL.Add(new Error(TipoError.Semantico, "No se pueden agregar dos atributos con el mismo nombre",
							nodo.ChildNodes.ElementAt(0).Token.Location.Line,
							nodo.ChildNodes.ElementAt(0).Token.Location.Column));
					}
				}
				return new CrearUserType(sentencia.ChildNodes.ElementAt(0).Token.ValueString, atributos, false,
					sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else
			{
				Dictionary<string, TipoObjetoDB> atributos = new Dictionary<string, TipoObjetoDB>();
				foreach (ParseTreeNode nodo in sentencia.ChildNodes.ElementAt(2).ChildNodes)
				{
					TipoDatoDB tipo = GeneradorDB.GetTipo(nodo.ChildNodes.ElementAt(1));
					string nombreTipo = GeneradorDB.GetNombreTipo(tipo, nodo.ChildNodes.ElementAt(1), true);
					try
					{
						atributos.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString, new TipoObjetoDB(tipo, nombreTipo));
					}
					catch (ArgumentException ex)
					{
						Analizador.ErroresCQL.Add(new Error(TipoError.Semantico, "No se pueden agregar dos atributos con el mismo nombre",
							nodo.ChildNodes.ElementAt(0).Token.Location.Line,
							nodo.ChildNodes.ElementAt(0).Token.Location.Column));
					}
				}
				return new CrearUserType(sentencia.ChildNodes.ElementAt(1).Token.ValueString, atributos, true,
					sentencia.ChildNodes.ElementAt(1).Token.Location.Line, sentencia.ChildNodes.ElementAt(1).Token.Location.Column);
			}
		}

		private static Sentencia GetFuncionAgregacion(ParseTreeNode sentencia)
		{
			return new FuncionAgregacion(sentencia.ChildNodes.ElementAt(0).Token.ValueString, (Seleccionar)GetSeleccionar(sentencia.ChildNodes.ElementAt(2)),
				sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
		}

		private static Sentencia GetBatch(ParseTreeNode sentencia)
		{
			Batch bt = new Batch(sentencia.Span.Location.Line, sentencia.Span.Location.Column);
			foreach (ParseTreeNode nodo in sentencia.ChildNodes)
			{
				switch (nodo.Term.Name)
				{
					case "INSERTAR":
						bt.Sentencias.Add((Sentencia)GetInsertar(nodo));
						break;
					case "ACTUALIZAR":
						bt.Sentencias.Add((Sentencia)GetActualizar(nodo));
						break;
					case "BORRAR":
						bt.Sentencias.Add((Sentencia)GetBorrar(nodo));
						break;
					default:
						Analizador.ErroresCQL.Add(new Error(TipoError.Semantico, "No se puede agregar una sentencia tipo SELECT en un bloque Batch",
							nodo.Span.Location.Line, nodo.Span.Location.Column));
						break;
				}
			}
			return bt;
		}

		private static Sentencia GetSeleccionar(ParseTreeNode sentencia)
		{
			List<Acceso> listaAccesos;
			if (sentencia.ChildNodes.ElementAt(0).Term.Name == "LISTA_ACCESOS")
			{
				listaAccesos = GetListaAcceso(sentencia.ChildNodes.ElementAt(0));
			}
			else
			{
				listaAccesos = null;
			}
			Seleccionar seleccionar = new Seleccionar(listaAccesos, sentencia.ChildNodes.ElementAt(1).Token.ValueString,
				sentencia.ChildNodes.ElementAt(1).Token.Location.Line, sentencia.ChildNodes.ElementAt(1).Token.Location.Column);

			GetPropiedadesSeleccionar(seleccionar, sentencia.ChildNodes.ElementAt(2));
			return seleccionar;
		}

		private static void GetPropiedadesSeleccionar(Seleccionar seleccionar, ParseTreeNode parseTreeNode)
		{
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
			{
				switch (nodo.Term.Name)
				{
					case "PROPIEDADDONDE":
						if (seleccionar.PropiedadWhere == null)
						{
							Where wh = GetWhere(nodo);
							if (wh != null) seleccionar.PropiedadWhere = wh;
						}
						else
						{
							Analizador.ErroresCQL.Add(new Error(TipoError.Semantico, "No se puede agregar dos clausulas Where a una sentencia" +
								" de selección", nodo.Span.Location.Line, nodo.Span.Location.Column));
						}
						break;
					case "PROPIEDADORDENAR":
						if (seleccionar.PropiedadOrderBy == null)
						{
							OrderBy wh = GetOrderBy(nodo);
							if (wh != null) seleccionar.PropiedadOrderBy = wh;
						}
						else
						{
							Analizador.ErroresCQL.Add(new Error(TipoError.Semantico, "No se puede agregar dos clausulas OrderBy a una sentencia" +
								" de selección", nodo.Span.Location.Line, nodo.Span.Location.Column));
						}
						break;
					case "PROPIEDADLIMIT":
						if (seleccionar.PropiedadLimit == null)
						{
							Limit wh = GetLimit(nodo);
							if (wh != null) seleccionar.PropiedadLimit = wh;
						}
						else
						{
							Analizador.ErroresCQL.Add(new Error(TipoError.Semantico, "No se puede agregar dos clausulas Limit a una sentencia" +
								" de selección", nodo.Span.Location.Line, nodo.Span.Location.Column));
						}
						break;
				}
			}
		}

		private static Limit GetLimit(ParseTreeNode nodo)
		{
			return new Limit(GetExpresion(nodo.ChildNodes.ElementAt(0)), nodo.ChildNodes.ElementAt(0).Span.Location.Line,
				nodo.ChildNodes.ElementAt(0).Span.Location.Column);
		}

		private static OrderBy GetOrderBy(ParseTreeNode nodo)
		{
			OrderBy order = new OrderBy(nodo.Span.Location.Line, nodo.Span.Location.Column);
			foreach (ParseTreeNode nodito in nodo.ChildNodes)
			{
				bool isAsc = nodito.ChildNodes.ElementAt(1).Token.ValueString.Equals("asc");
				order.Propiedades.Add(new PropOrderBy(GetAcceso(nodito.ChildNodes.ElementAt(0)), isAsc));
			}
			return order;
		}

		private static Sentencia GetBorrar(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 1)
			{
				//eliminar todo de tabla
				return new Borrar(sentencia.ChildNodes.ElementAt(0).Token.ValueString, sentencia.ChildNodes.ElementAt(0).Token.Location.Line,
					sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else if (sentencia.ChildNodes.Count == 2)
			{
				if (sentencia.ChildNodes.ElementAt(0).Term.Name == "AC_CAMPO")
				{// eliminar acCampo de tabla
					return new Borrar(sentencia.ChildNodes.ElementAt(1).Token.ValueString, GetAcCampo(sentencia.ChildNodes.ElementAt(0)),
						sentencia.ChildNodes.ElementAt(1).Token.Location.Line, sentencia.ChildNodes.ElementAt(1).Token.Location.Column);
				}
				else
				{
					//eliminar todo de tabla con where
					return new Borrar(sentencia.ChildNodes.ElementAt(0).Token.ValueString, GetWhere(sentencia.ChildNodes.ElementAt(1)),
						sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);

				}
			}
			else
			{
				//eliminar algo de tabla con where
				return new Borrar(sentencia.ChildNodes.ElementAt(1).Token.ValueString, GetAcCampo(sentencia.ChildNodes.ElementAt(0)),
					GetWhere(sentencia.ChildNodes.ElementAt(2)), sentencia.ChildNodes.ElementAt(1).Token.Location.Line, sentencia.ChildNodes.ElementAt(1).Token.Location.Column);
			}
		}

		private static Sentencia GetActualizar(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 3)
			{
				//sin where
				return new Actualizar(GetListaAsignacionesColumna(sentencia.ChildNodes.ElementAt(2)),
					sentencia.ChildNodes.ElementAt(0).Token.ValueString,
					sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else
			{
				//con where
				return new Actualizar(GetListaAsignacionesColumna(sentencia.ChildNodes.ElementAt(2)),
					sentencia.ChildNodes.ElementAt(0).Token.ValueString,
					GetWhere(sentencia.ChildNodes.ElementAt(2)),
					sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
		}

		private static Where GetWhere(ParseTreeNode parseTreeNode)
		{
			if (parseTreeNode.ChildNodes.Count == 1)
			{
				return new Where(GetCondicion(parseTreeNode.ChildNodes.ElementAt(0)), parseTreeNode.ChildNodes.ElementAt(0).Span.Location.Line,
					parseTreeNode.ChildNodes.ElementAt(0).Span.Location.Column);
			}
			else
			{
				if (parseTreeNode.ChildNodes.ElementAt(1).Term.Name == "LISTAEXPRESIONES")
				{
					//(LISTA DE EXPRESIONES)
					return new Where(GetExpresion(parseTreeNode.ChildNodes.ElementAt(0)), GetListaExpresiones(parseTreeNode.ChildNodes.ElementAt(1)),
											parseTreeNode.ChildNodes.ElementAt(0).Span.Location.Line, parseTreeNode.ChildNodes.ElementAt(0).Span.Location.Column);

				}
				else
				{
					//LISTA EN VARIABLES, FORMATO JSON O CORCHETES
					return new Where(GetExpresion(parseTreeNode.ChildNodes.ElementAt(0)), GetExpresion(parseTreeNode.ChildNodes.ElementAt(1)),
						parseTreeNode.ChildNodes.ElementAt(0).Span.Location.Line, parseTreeNode.ChildNodes.ElementAt(0).Span.Location.Column);
				}
			}
		}

		private static List<AsignacionColumna> GetListaAsignacionesColumna(ParseTreeNode parseTreeNode)
		{
			List<AsignacionColumna> lista = new List<AsignacionColumna>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
			{
				lista.Add(new AsignacionColumna(GetAcceso(nodo.ChildNodes.ElementAt(0)), GetExpresion(nodo.ChildNodes.ElementAt(1))));
			}
			return lista;
		}

		private static Sentencia GetInsertar(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 2)
			{
				//insercion normal
				return new Insertar(sentencia.ChildNodes.ElementAt(0).Token.ValueString,
					GetListaExpresiones(sentencia.ChildNodes.ElementAt(1)), sentencia.ChildNodes.ElementAt(0).Token.Location.Line,
					sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else
			{
				//insercion especial
				return new Insertar(sentencia.ChildNodes.ElementAt(0).Token.ValueString, GetListaStrings(sentencia.ChildNodes.ElementAt(1)),
					GetListaExpresiones(sentencia.ChildNodes.ElementAt(2)), sentencia.ChildNodes.ElementAt(0).Token.Location.Line,
					sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
		}

		private static Sentencia GetDenegarPermiso(ParseTreeNode sentencia)
		{
			return new DenegarPermiso(sentencia.ChildNodes.ElementAt(0).Token.ValueString,
				sentencia.ChildNodes.ElementAt(1).Token.ValueString, sentencia.ChildNodes.ElementAt(1).Token.Location.Line,
				sentencia.ChildNodes.ElementAt(1).Token.Location.Column);
		}

		private static Sentencia GetOtorgarpermiso(ParseTreeNode sentencia)
		{
			return new OtorgarPermiso(sentencia.ChildNodes.ElementAt(0).Token.ValueString,
				sentencia.ChildNodes.ElementAt(1).Token.ValueString, sentencia.ChildNodes.ElementAt(1).Token.Location.Line,
				sentencia.ChildNodes.ElementAt(1).Token.Location.Column);
		}

		private static Sentencia GetCrearUsuario(ParseTreeNode sentencia)
		{
			return new CrearUsuario(sentencia.ChildNodes.ElementAt(0).Token.ValueString,
				sentencia.ChildNodes.ElementAt(1).Token.ValueString, sentencia.ChildNodes.ElementAt(1).Token.Location.Line,
				sentencia.ChildNodes.ElementAt(1).Token.Location.Column);
		}

		private static Sentencia GetEliminarUserType(ParseTreeNode sentencia)
		{
			return new EliminarUserType(sentencia.ChildNodes.ElementAt(0).Token.ValueString,
				sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
		}

		private static Sentencia GetModificarUserType(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 3)
			{
				//add
				Dictionary<string, TipoObjetoDB> atributos = new Dictionary<string, TipoObjetoDB>();
				foreach (ParseTreeNode nodo in sentencia.ChildNodes.ElementAt(2).ChildNodes)
				{
					TipoDatoDB tipo = GeneradorDB.GetTipo(nodo.ChildNodes.ElementAt(1));
					string nombreTipo = GeneradorDB.GetNombreTipo(tipo, nodo.ChildNodes.ElementAt(1), true);
					try
					{
						atributos.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString, new TipoObjetoDB(tipo, nombreTipo));
					}
					catch (ArgumentException ex)
					{
						Analizador.ErroresCQL.Add(new Error(TipoError.Semantico, "No se pueden agregar dos atributos con el mismo nombre",
							nodo.ChildNodes.ElementAt(0).Token.Location.Line,
							nodo.ChildNodes.ElementAt(0).Token.Location.Column));
					}
				}
				return new AlterarUserType(TipoAccion.Agregar, sentencia.ChildNodes.ElementAt(0).Token.ValueString,
					atributos, sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else
			{
				//borrar
				List<Acceso> accesos = GetListaAcceso(sentencia.ChildNodes.ElementAt(1));
				return new AlterarUserType(TipoAccion.Quitar, sentencia.ChildNodes.ElementAt(0).Token.ValueString, accesos,
					sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
		}

		private static List<Acceso> GetListaAcceso(ParseTreeNode parseTreeNode)
		{
			List<Acceso> lista = new List<Acceso>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
			{
				lista.Add(GetAcceso(nodo));
			}
			return lista;
		}

		private static Acceso GetAcceso(ParseTreeNode nodo)
		{
			Acceso accesss = new Acceso(nodo.Span.Location.Line, nodo.Span.Location.Column);
			foreach (ParseTreeNode ac_campo in nodo.ChildNodes)
			{
				AccesoPar ac = GetAcCampo(ac_campo);
				if (ac != null) accesss.Objetos.Enqueue(ac);
			}
			return accesss;
		}

		private static AccesoPar GetAcCampo(ParseTreeNode ac_campo)
		{
			if (ac_campo.ChildNodes.Count == 1)
			{
				//nombre
				return new AccesoPar(ac_campo.ChildNodes.ElementAt(0).Token.ValueString,TipoAcceso.Campo);
			}
			else if (ac_campo.ChildNodes.Count == 2)
			{
				if (ac_campo.ChildNodes.ElementAt(1).Term.Name == "LISTAEXPRESIONES")
				{
					//llamada a funcion
					return new AccesoPar(new LlamadaFuncion(ac_campo.ChildNodes.ElementAt(0).Token.ValueString,
						GetListaExpresiones(ac_campo.ChildNodes.ElementAt(1)), ac_campo.ChildNodes.ElementAt(0).Token.Location.Line,
						ac_campo.ChildNodes.ElementAt(0).Token.Location.Column),
						TipoAcceso.LlamadaFuncion);
				}
				else
				{
					//acceso a arreglo
					return new AccesoPar(new AccesoArreglo(GetExpresion(ac_campo.ChildNodes.ElementAt(1)),
						ac_campo.ChildNodes.ElementAt(0).Token.ValueString,
						ac_campo.ChildNodes.ElementAt(0).Token.Location.Line, ac_campo.ChildNodes.ElementAt(0).Token.Location.Column),
						TipoAcceso.AccesoArreglo);
				}

			}
			return null;
		}

		private static List<Expresion> GetListaExpresiones(ParseTreeNode parseTreeNode)
		{
			List<Expresion> lista = new List<Expresion>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
			{
				lista.Add(GetExpresion(nodo));
			}
			return lista;
		}

		private static Sentencia GetTruncarTabla(ParseTreeNode sentencia)
		{
			return new TruncarTabla(sentencia.ChildNodes.ElementAt(0).Token.ValueString,
				sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
		}

		private static Sentencia GetEliminarTabla(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 1)
			{
				//sin ifExists
				return new EliminarTabla(sentencia.ChildNodes.ElementAt(0).Token.ValueString, false,
					sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else
			{
				//con ifExists
				return new EliminarTabla(sentencia.ChildNodes.ElementAt(1).Token.ValueString, true,
					sentencia.ChildNodes.ElementAt(1).Token.Location.Line, sentencia.ChildNodes.ElementAt(1).Token.Location.Column);
			}
		}

		private static Sentencia GetModificarTabla(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 3)
			{
				//add
				List<object> cols = GetColumnasTabla(sentencia.ChildNodes.ElementAt(2));
				List<Columna> columnas = new List<Columna>();
				foreach (object ob in cols)
				{
					if (ob.GetType() == typeof(Columna))
					{
						columnas.Add((Columna)ob);
					}
				}
				if (cols.Count != columnas.Count)
				{
					Analizador.ErroresCQL.Add(new Error(TipoError.Semantico, "No se pueden agregar llaves primarias a una tabla existente",
						sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column));
					//RETORNAR NULL SI SE DEBE ALTERAR TODA LA INSTRUCCION
				}
				return new AlterarTabla(TipoAccion.Agregar, sentencia.ChildNodes.ElementAt(0).Token.ValueString, columnas,
					sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else
			{
				//drop
				List<string> columnas = GetListaStrings(sentencia.ChildNodes.ElementAt(1));

				return new AlterarTabla(TipoAccion.Quitar, sentencia.ChildNodes.ElementAt(0).Token.ValueString, columnas,
					sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
		}

		private static Sentencia GetCrearTabla(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 2)
			{
				//sin if-exist
				List<object> cls = GetColumnasTabla(sentencia.ChildNodes.ElementAt(1));
				return new CrearTabla(sentencia.ChildNodes.ElementAt(0).Token.ValueString, cls, false,
					sentencia.ChildNodes.ElementAt(0).Token.Location.Line,
				   sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else
			{
				//con if-exist
				List<object> cls = GetColumnasTabla(sentencia.ChildNodes.ElementAt(2));
				return new CrearTabla(sentencia.ChildNodes.ElementAt(1).Token.ValueString, cls, true, sentencia.ChildNodes.ElementAt(1).Token.Location.Line,
				   sentencia.ChildNodes.ElementAt(1).Token.Location.Column);
			}

		}

		private static List<object> GetColumnasTabla(ParseTreeNode parseTreeNode)
		{

			List<object> cols = new List<object>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
			{
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
				string nombreTipo = GeneradorDB.GetNombreTipo(t, nodo.ChildNodes.ElementAt(1), true);
				return new Columna(nodo.ChildNodes.ElementAt(0).Token.ValueString, new TipoObjetoDB(t, nombreTipo), false);
			}
			else if (nodo.ChildNodes.Count == 3)
			{
				//llave primaria compuesta
				return GetListaStrings(nodo.ChildNodes.ElementAt(2));
			}
			else if (nodo.ChildNodes.Count == 4)
			{
				//columna con llave primaria
				TipoDatoDB t = GeneradorDB.GetTipo(nodo.ChildNodes.ElementAt(1));
				string nombreTipo = GeneradorDB.GetNombreTipo(t, nodo.ChildNodes.ElementAt(1), true);
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

		private static Sentencia GetEliminarDB(ParseTreeNode sentencia)
		{
			return new EliminarBaseDatos(sentencia.ChildNodes.ElementAt(0).Token.ValueString, sentencia.ChildNodes.ElementAt(0).Token.Location.Line,
				sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
		}

		private static Sentencia GetUsarDB(ParseTreeNode sentencia)
		{
			return new UsarBaseDatos(sentencia.ChildNodes.ElementAt(0).Token.ValueString, sentencia.ChildNodes.ElementAt(0).Token.Location.Line,
				sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
		}

		private static Sentencia GetCrearDB(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 1)
			{
				return new CrearBaseDatos(sentencia.ChildNodes.ElementAt(0).Token.ValueString, false,
					sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else
			{
				return new CrearBaseDatos(sentencia.ChildNodes.ElementAt(1).Token.ValueString, true,
					sentencia.ChildNodes.ElementAt(1).Token.Location.Line, sentencia.ChildNodes.ElementAt(1).Token.Location.Column);
			}
		}
		#endregion

		#region Expresion
		private static Expresion GetExpresion(ParseTreeNode raiz)
		{
			switch (raiz.ChildNodes.Count)
			{
				case 1://valor o condicion
					if (raiz.ChildNodes.ElementAt(0).Term.Name.Equals("VALOR"))
					{
						return GetValor(raiz.ChildNodes.ElementAt(0));
					}
					if (raiz.ChildNodes.ElementAt(0).Term.Name.Equals("CONDICION"))
					{
						return GetCondicion(raiz.ChildNodes.ElementAt(0));
					}
					break;
				case 3: //operaciones 
					return new Operacion(GetExpresion(raiz.ChildNodes.ElementAt(0)), GetExpresion(raiz.ChildNodes.ElementAt(2)), GetTipoOperacion(raiz.ChildNodes.ElementAt(1)), raiz.ChildNodes.ElementAt(1).Token.Location.Line, raiz.ChildNodes.ElementAt(1).Token.Location.Column);
			}
			return null;
		}

		private static Condicion GetCondicion(ParseTreeNode raiz)
		{
			switch (raiz.ChildNodes.Count)
			{
				case 1://true o false
					if (raiz.ChildNodes.ElementAt(0).Term.Name=="CONDICION") {
						return GetCondicion(raiz.ChildNodes.ElementAt(0));
					}
					return new Condicion(raiz.ChildNodes.ElementAt(0).Token.ValueString.ToLower() == "true", TipoOperacion.Booleano, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
				case 2://not
					return new Condicion(GetCondicion(raiz.ChildNodes.ElementAt(1)), TipoOperacion.Not, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
				case 3://operaciones
					return new Condicion(GetExpresion(raiz.ChildNodes.ElementAt(0)), GetExpresion(raiz.ChildNodes.ElementAt(2)), GetTipoOperacion(raiz.ChildNodes.ElementAt(1)), raiz.ChildNodes.ElementAt(1).Token.Location.Line, raiz.ChildNodes.ElementAt(1).Token.Location.Column);
			}
			return null;
		}

		private static TipoOperacion GetTipoOperacion(ParseTreeNode parseTreeNode)
		{
			switch (parseTreeNode.Token.Value)
			{
				case "+": return TipoOperacion.Suma;
				case "-": return TipoOperacion.Resta;
				case "*": return TipoOperacion.Multiplicacion;
				case "/": return TipoOperacion.Division;
				case "**": return TipoOperacion.Potencia;
				case "%": return TipoOperacion.Modulo;
				case "<": return TipoOperacion.Menor;
				case "<=": return TipoOperacion.MenorIgual;
				case ">": return TipoOperacion.Mayor;
				case ">=": return TipoOperacion.MayorIgual;
				case "==": return TipoOperacion.Igual;
				case "!=": return TipoOperacion.Diferente;
				case "&&": return TipoOperacion.And;
				case "||": return TipoOperacion.Or;
				case "^": return TipoOperacion.Xor;
			}
			return TipoOperacion.Nulo;
		}

		private static Expresion GetValor(ParseTreeNode raiz)
		{
			switch (raiz.ChildNodes.Count)
			{
				case 1://valores, o expresion o llamada
					switch (raiz.ChildNodes.ElementAt(0).Term.Name)
					{
						case "NULL":
							return new Operacion("null", TipoOperacion.Nulo, raiz.Span.Location.Line, raiz.Span.Location.Column);
						case "numero":
							return new Operacion(Datos.GetValor(raiz.ChildNodes.ElementAt(0).Token.ValueString), TipoOperacion.Numero, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
						case "cadena":
							return new Operacion(raiz.ChildNodes.ElementAt(0).Token.ValueString, TipoOperacion.String, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
						case "id":
							return new Operacion(raiz.ChildNodes.ElementAt(0).Token.ValueString, TipoOperacion.Identificador, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
						case "nombre":
							return new Operacion(raiz.ChildNodes.ElementAt(0).Token.ValueString, TipoOperacion.Identificador, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
						case "date":
							return new Operacion(Datos.GetValor(raiz.ChildNodes.ElementAt(0).Token.ValueString), TipoOperacion.Fecha, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
						case "time":
							return new Operacion(Datos.GetValor(raiz.ChildNodes.ElementAt(0).Token.ValueString), TipoOperacion.Hora, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
						case "EXPRESION":
							return GetExpresion(raiz.ChildNodes.ElementAt(0));
						case "ACCESO":
							return GetAcceso(raiz.ChildNodes.ElementAt(0));
						case "LLAMADAFUNCION":
							return GetLlamadaFuncion(raiz.ChildNodes.ElementAt(0));
						case "INFOCOLLECTIONS": //{valor:{}:val:{}}

						case "LISTAEXPRESIONES": //[1112,2,3,3,2]

							break;
						case "MODIFICADORES":
							return GetmodificadorExp(raiz.ChildNodes.ElementAt(0));
						case "FUNCIONAGREGACION":

							break;

					}
					break;
				case 2://menos
					if (raiz.ChildNodes.ElementAt(0).Term.Name.Equals("-"))
					{
						return new Operacion(GetExpresion(raiz.ChildNodes.ElementAt(1)), TipoOperacion.Menos, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
					}
					else if (raiz.ChildNodes.ElementAt(0).Term.Name.Equals("new"))
					{
						//new tipodato
						return new Operacion(raiz.ChildNodes.ElementAt(1).Token.ValueString, TipoOperacion.Objeto, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);

					}
					else
					{
						//id punto acceso
						Acceso acc = new Acceso(raiz.Span.Location.Line, raiz.Span.Location.Column);
						acc.Objetos.Enqueue(new AccesoPar(raiz.ChildNodes.ElementAt(0).Token.ValueString,TipoAcceso.Variable));
						foreach (ParseTreeNode ac_campo in raiz.ChildNodes.ElementAt(1).ChildNodes)
						{
							AccesoPar ac = GetAcCampo(ac_campo);
							if (ac != null) acc.Objetos.Enqueue(ac);
						}
						return acc;
					}
			}
			return null;
		}

		private static Expresion GetmodificadorExp(ParseTreeNode raiz)
		{
			bool op = raiz.ChildNodes.ElementAt(1).Token.ValueString == "++";
			if (raiz.ChildNodes.ElementAt(0).Term.Name != "ACCESO")
			{
				return new ModificadorExp(raiz.ChildNodes.ElementAt(0).Token.ValueString, op, raiz.ChildNodes.ElementAt(0).Token.Location.Line,
												raiz.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else
			{
				return new ModificadorExp(GetAcceso(raiz.ChildNodes.ElementAt(0)), op, raiz.ChildNodes.ElementAt(0).Token.Location.Line,
								raiz.ChildNodes.ElementAt(0).Token.Location.Column);
			}

		}

		private static Expresion GetLlamadaFuncion(ParseTreeNode parseTreeNode)
		{
			return new LlamadaFuncionExp(parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString, GetListaExpresiones(parseTreeNode.ChildNodes.ElementAt(1)),
				parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column);
		}
		#endregion
	}
}
