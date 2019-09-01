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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Analisis
{
	static class GeneradorAstCql
	{

		public static List<Sentencia> GetAST(ParseTreeNode raiz)
		{
			if (raiz!=null) {
				GuardarFunciones(raiz.ChildNodes.ElementAt(0));
				return GetSentencias(raiz.ChildNodes.ElementAt(0));
			}
			return new List<Sentencia>();
		}

		public static void GuardarFunciones(ParseTreeNode nodo)
		{
			foreach (ParseTreeNode sentencia in nodo.ChildNodes) {
				if (sentencia.Term.Name.Equals("CREAR_FUNCION"))
				{
					Sentencia n = GetFuncion(sentencia);
					if (n != null)
					{
						String llave = ((Funcion)n).GetLlave();
						if (!Analizador.ExisteFuncion(llave))
						{
							Analizador.AddFuncion((Funcion)n);
						}
						else
						{
							Analizador.ErroresCQL.Add(new Error(
								new ThrowError(TipoThrow.FunctionAlreadyExists, 
								"La función '" + llave + "' ya existe", n.Linea, n.Columna)));
						}
					}
				}
			}
		}

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
					case "CASTEO":
						n = GetCasteo(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "SWITCH":
						n = GetSwitch(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "BREAK":
						n = GetBreak(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "FOR":
						n = GetFor(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "LLAMADAFUNCION":
						n= GetLlamadaFuncionSent(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "RETORNO":
						n = GetRetrun(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "CONTINUE":
						n = GetContinue(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "TRYCATCH":
						n = GetTryCatch(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "THROW":
						n = GetThrow(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "CREAR_PROC":
						n = GetCrearProcedimiento(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "CALLPROC":
						n = GetLlamadaProc(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "ASIGNACIONCALL":
						n = GetAsignacionCall(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "CREAR_CURSOR":
						n = GetCrearCursor(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "FOREACH":
						n = GetForeach(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "OPENCURSOR":
						n = GetOpenCursor(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "CLOSECURSOR":
						n = GetCloseCursor(sentencia);
						if (n != null) sentencias.Add(n);
						break;
					case "ACCESOFUNCION":
						n = GetAcccesoFuncion(sentencia);
						if (n != null) sentencias.Add(n);
						break;
				}
			}
			return sentencias;
		}

		#region Lenguaje_FCL
		private static Sentencia GetAcccesoFuncion(ParseTreeNode sentencia)
		{
			string variable = sentencia.ChildNodes.ElementAt(0).Token.ValueString;
			Acceso acceso = new Acceso(sentencia.Span.Location.Line, sentencia.Span.Location.Column);
			acceso.Objetos.Add(new AccesoPar(variable, TipoAcceso.Variable));
			Acceso acceso2 = GetAcceso(sentencia.ChildNodes.ElementAt(1));
			foreach (AccesoPar par in acceso2.Objetos)
			{
				acceso.Objetos.Add(par);
			}
			if (acceso.Objetos.ElementAt(acceso.Objetos.Count-1).Tipo!=TipoAcceso.LlamadaFuncion) {
				Analizador.ErroresCQL.Add(new Error(TipoError.Semantico,
					"No se está invocando a ninguna función",
					sentencia.Span.Location.Line, 
					sentencia.Span.Location.Column));
				return null;
			}
			return new AccesoFuncion(acceso, sentencia.Span.Location.Line, sentencia.Span.Location.Column);
		}

		private static Sentencia GetOpenCursor(ParseTreeNode sentencia)
		{
			return new AbrirCursor(sentencia.ChildNodes.ElementAt(0).Token.ValueString, sentencia.Span.Location.Line, sentencia.Span.Location.Column);
		}

		private static Sentencia GetCloseCursor(ParseTreeNode sentencia)
		{
			return new CerrarCursor(sentencia.ChildNodes.ElementAt(0).Token.ValueString, sentencia.Span.Location.Line, sentencia.Span.Location.Column);
		}

		private static Sentencia GetForeach(ParseTreeNode sentencia)
		{
			List<Parametro> parametros = GetParametros(sentencia.ChildNodes.ElementAt(0));
			if (parametros.Count==0) {
				Analizador.ErroresCQL.Add(new Error(TipoError.Sintactico,"Debe escribir al menos una variable en el foreach",
					sentencia.Span.Location.Line,sentencia.Span.Location.Column));
				return null;
			}
			
			return new Foreach(parametros, sentencia.ChildNodes.ElementAt(1).Token.ValueString,
				GetSentencias(sentencia.ChildNodes.ElementAt(2)), sentencia.Span.Location.Line, sentencia.Span.Location.Column);
		}

		private static Sentencia GetCrearCursor(ParseTreeNode sentencia)
		{
			return new CrearCursor(sentencia.ChildNodes.ElementAt(0).Token.ValueString,(Seleccionar)GetSeleccionar(sentencia.ChildNodes.ElementAt(1)),
				sentencia.Span.Location.Line,sentencia.Span.Location.Column);
		}

		private static Sentencia GetAsignacionCall(ParseTreeNode sentencia)
		{
			List<Acceso> variables = GetListaAcceso2(sentencia.ChildNodes.ElementAt(0));
			LlamadaProcedimiento callProc = (LlamadaProcedimiento)GetLlamadaProc(sentencia.ChildNodes.ElementAt(1));
			return new AsignacionCall(variables, callProc, sentencia.Span.Location.Line, sentencia.Span.Location.Column);
		}

		private static List<Acceso> GetListaAcceso2(ParseTreeNode parseTreeNode)
		{
			List<Acceso> lista = new List<Acceso>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
			{
				lista.Add(GetAccesoID(nodo));
			}
			return lista;
		}

		private static Acceso GetAccesoID(ParseTreeNode nodo)
		{
			Acceso accesss = new Acceso(nodo.Span.Location.Line, nodo.Span.Location.Column);
			accesss.Objetos.Add(new AccesoPar(nodo.ChildNodes.ElementAt(0).Token.ValueString, TipoAcceso.Variable));
			if (nodo.ChildNodes.Count > 1)
			{
				Acceso derecha = GetAcceso(nodo.ChildNodes.ElementAt(1));
				accesss.Objetos.AddRange(derecha.Objetos);
			}

			return accesss;
		}

		private static Sentencia GetLlamadaProc(ParseTreeNode sentencia)
		{
			return new LlamadaProcedimiento(sentencia.ChildNodes.ElementAt(0).Token.ValueString,
				GetListaExpresiones(sentencia.ChildNodes.ElementAt(1)),
				sentencia.Span.Location.Line, sentencia.Span.Location.Column);
		}

		private static Sentencia GetCrearProcedimiento(ParseTreeNode sentencia)
		{
			String codigo = Analizador.CodigoAnalizado.Substring(sentencia.ChildNodes.ElementAt(3).Span.Location.Position,
				sentencia.ChildNodes.ElementAt(3).Span.Length);
			return new CrearProcedimiento(sentencia.ChildNodes.ElementAt(0).Token.ValueString, GetParametros(sentencia.ChildNodes.ElementAt(1)),
				GetParametros(sentencia.ChildNodes.ElementAt(2)), GetSentencias(sentencia.ChildNodes.ElementAt(3)), codigo,
				sentencia.Span.Location.Line, sentencia.Span.Location.Column);
		}

		private static Sentencia GetThrow(ParseTreeNode sentencia)
		{
			return new Throw(sentencia.ChildNodes.ElementAt(2).Token.ValueString, sentencia.Span.Location.Line, sentencia.Span.Location.Column);
		}

		private static Sentencia GetTryCatch(ParseTreeNode sentencia)
		{
			return new TryCatch(GetSentencias(sentencia.ChildNodes.ElementAt(0)), GetSentencias(sentencia.ChildNodes.ElementAt(3)),
				sentencia.ChildNodes.ElementAt(1).Token.ValueString, sentencia.ChildNodes.ElementAt(2).Token.ValueString,
				sentencia.Span.Location.Line, sentencia.Span.Location.Column);
		}

		private static Sentencia GetContinue(ParseTreeNode sentencia)
		{
			return new Continue(sentencia.Span.Location.Line,sentencia.Span.Location.Column);
		}

		private static Sentencia GetRetrun(ParseTreeNode sentencia)
		{
			return new Return(GetListaExpresiones(sentencia.ChildNodes.ElementAt(1)),sentencia.Span.Location.Line,sentencia.Span.Location.Column);
		}

		private static Sentencia GetLlamadaFuncionSent(ParseTreeNode parseTreeNode)
		{
			return new LlamadaFuncion(parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString, GetListaExpresiones(parseTreeNode.ChildNodes.ElementAt(1)),
				parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column);
		}

		private static Sentencia GetFuncion(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 4)
			{
				//funcion con tipo
				TipoDatoDB tipo = GetTipo(sentencia.ChildNodes.ElementAt(0));
				string nombreTipo = GetNombreTipo(tipo,sentencia.ChildNodes.ElementAt(0),true);
				string nombreFuncion = sentencia.ChildNodes.ElementAt(1).Token.ValueString;
				List<Parametro> parametros = GetParametros(sentencia.ChildNodes.ElementAt(2));
				List<Sentencia> sentencias = GetSentencias(sentencia.ChildNodes.ElementAt(3));
					return new Funcion(nombreFuncion, parametros, new TipoObjetoDB(tipo, nombreTipo), sentencias,
						sentencia.Span.Location.Line, sentencia.Span.Location.Column);
			
			}
			else {
				//funcion void
				string nombreFuncion = sentencia.ChildNodes.ElementAt(0).Token.ValueString;
					List < Parametro > parametros = GetParametros(sentencia.ChildNodes.ElementAt(1));
				List<Sentencia> sentencias = GetSentencias(sentencia.ChildNodes.ElementAt(2));
					return new Funcion(nombreFuncion, parametros, null, sentencias,
						sentencia.Span.Location.Line, sentencia.Span.Location.Column);				
			}
		}

		private static List<Parametro> GetParametros(ParseTreeNode parseTreeNode)
		{
			List<Parametro> param = new List<Parametro>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes) {
				TipoDatoDB tipo = GetTipo(nodo.ChildNodes.ElementAt(0));
				string nombreTipo = GetNombreTipo(tipo, nodo.ChildNodes.ElementAt(0), true);
					param.Add(new Parametro(nodo.ChildNodes.ElementAt(1).Token.ValueString, new TipoObjetoDB(tipo, nombreTipo)));
			}
			return param;
		}

		private static Sentencia GetFor(ParseTreeNode sentencia)
		{
			Asignacion asignacion=null;
			Declaracion dec=null;
			Expresion op=GetExpresion(sentencia.ChildNodes.ElementAt(2));
			Condicion cond=GetCondicion(sentencia.ChildNodes.ElementAt(1));
			List<Sentencia> lista = GetSentencias(sentencia.ChildNodes.ElementAt(3));
			if (sentencia.ChildNodes.ElementAt(0).ChildNodes.Count == 3)
			{
				Sentencia n=GetDeclaracion(sentencia.ChildNodes.ElementAt(0));
				if (n != null) dec = (Declaracion)n;
			}
			else {
			Sentencia n=GetAsignacion(sentencia.ChildNodes.ElementAt(0));
				if (n != null) asignacion = (Asignacion)n;
			}
			if (asignacion!=null || dec!=null) {
				return new For(asignacion, dec, cond, op, lista, sentencia.Span.Location.Line, sentencia.Span.Location.Column);
			}
			return null;
		}

		private static Sentencia GetBreak(ParseTreeNode sentencia)
		{
			return new Break(sentencia.Span.Location.Line,sentencia.Span.Location.Column);
		}

		private static Sentencia GetSwitch(ParseTreeNode sentencia)
		{
			
				Expresion expresion = GetExpresion(sentencia.ChildNodes.ElementAt(0));
				List<Case> cases = GetListaCases(sentencia.ChildNodes.ElementAt(1));
			List<Sentencia> sentenciasDef = null;
			if (sentencia.ChildNodes.Count == 3)
			{
				 sentenciasDef = GetSentencias(sentencia.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0));
			}
			return new Switch(expresion, cases, sentenciasDef, sentencia.Span.Location.Line, sentencia.Span.Location.Column);
		}

		private static List<Case> GetListaCases(ParseTreeNode parseTreeNode)
		{
			List<Case> lista = new List<Case>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes) {
				Expresion ex = GetExpresion(nodo.ChildNodes.ElementAt(0));
				List<Sentencia> sentencias = GetSentencias(nodo.ChildNodes.ElementAt(1));
				lista.Add(new Case(ex,sentencias,nodo.Span.Location.Line,nodo.Span.Location.Column));
			}
			return lista;
		}

		private static Sentencia GetCasteo(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 3)
			{
				Acceso acceso = new Acceso(sentencia.Span.Location.Line, sentencia.Span.Location.Column);
				acceso.Objetos.Add(new AccesoPar(sentencia.ChildNodes.ElementAt(0).Token.ValueString,TipoAcceso.Variable));
				TipoDatoDB tipo = GetTipo(sentencia.ChildNodes.ElementAt(1));
				string nombreTipo = GetNombreTipo(tipo,sentencia.ChildNodes.ElementAt(1),true);
				Expresion expresion = GetExpresion(sentencia.ChildNodes.ElementAt(2));
				return new AsignacionCasteo(acceso, new TipoObjetoDB(tipo, nombreTipo),expresion,sentencia.Span.Location.Line, sentencia.Span.Location.Column);
			}
			else {
				if (sentencia.ChildNodes.ElementAt(1).Term.Name.Equals("EXPRESION"))
				{
					Acceso acceso = new Acceso(sentencia.Span.Location.Line, sentencia.Span.Location.Column);
					acceso.Objetos.Add(new AccesoPar(
						new AccesoArreglo(GetExpresion(sentencia.ChildNodes.ElementAt(1)), sentencia.ChildNodes.ElementAt(0).Token.ValueString,
						sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column), 
						TipoAcceso.AccesoArreglo));
					TipoDatoDB tipo = GetTipo(sentencia.ChildNodes.ElementAt(2));
					string nombreTipo = GetNombreTipo(tipo, sentencia.ChildNodes.ElementAt(2), true);
					Expresion expresion = GetExpresion(sentencia.ChildNodes.ElementAt(3));
					return new AsignacionCasteo(acceso, new TipoObjetoDB(tipo, nombreTipo), expresion, sentencia.Span.Location.Line, sentencia.Span.Location.Column);
				}
				else {
					Acceso acceso = new Acceso(sentencia.Span.Location.Line, sentencia.Span.Location.Column);
					acceso.Objetos.Add(new AccesoPar(sentencia.ChildNodes.ElementAt(0).Token.ValueString, TipoAcceso.Variable));
					Acceso acceso2 = GetAcceso(sentencia.ChildNodes.ElementAt(1));
					foreach (AccesoPar par in acceso2.Objetos) {
						acceso.Objetos.Add(par);
					}
					TipoDatoDB tipo = GetTipo(sentencia.ChildNodes.ElementAt(2));
					string nombreTipo = GetNombreTipo(tipo, sentencia.ChildNodes.ElementAt(2), true);
					Expresion expresion = GetExpresion(sentencia.ChildNodes.ElementAt(3));
					return new AsignacionCasteo(acceso, new TipoObjetoDB(tipo, nombreTipo), expresion, sentencia.Span.Location.Line, sentencia.Span.Location.Column);
				}
			}
		}

		private static Sentencia GetAsignacion(ParseTreeNode sentencia)
		{
			Acceso ac2 = new Acceso(sentencia.Span.Location.Line, sentencia.Span.Location.Column);
			Expresion exp = null;
			if (sentencia.ChildNodes.Count == 2)
			{
				ac2.Objetos.Add(new AccesoPar(sentencia.ChildNodes.ElementAt(0).Token.ValueString, TipoAcceso.Variable));
				 exp = GetExpresion(sentencia.ChildNodes.ElementAt(1));
				
			}
			else {
				if (sentencia.ChildNodes.ElementAt(1).Term.Name == "ACCESO")
				{
					Acceso ac = GetAcceso(sentencia.ChildNodes.ElementAt(1));
					ac2.Objetos.Add(new AccesoPar(sentencia.ChildNodes.ElementAt(0).Token.ValueString,TipoAcceso.Variable));
					ac2.Objetos.AddRange(ac.Objetos);
					exp = GetExpresion(sentencia.ChildNodes.ElementAt(2));
				}
				else {
					//acceso a algo []
					ac2.Objetos.Add(new AccesoPar(
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
			TipoDatoDB tipo = GetTipo(sentencia.ChildNodes.ElementAt(0));
			string nombreTipo = GetNombreTipo(tipo, sentencia.ChildNodes.ElementAt(0), true);
			if (tipo == TipoDatoDB.LISTA_OBJETO || tipo == TipoDatoDB.SET_OBJETO || tipo == TipoDatoDB.MAP_OBJETO ||
				tipo == TipoDatoDB.LISTA_PRIMITIVO || tipo == TipoDatoDB.SET_PRIMITIVO || tipo == TipoDatoDB.MAP_PRIMITIVO)
			{
				if (!nombreTipo.Equals("null")) {
					Analizador.ErroresCQL.Add(new Error(
				TipoError.Semantico, "No se puede declarar una lista con tipos definidos", sentencia.Span.Location.Line, sentencia.Span.Location.Column));

				}
			}
			List<string> variables;
			if (sentencia.ChildNodes.ElementAt(1).Term.Name.Equals("LISTAVARIABLES")) {
				variables = GetListaStrings(sentencia.ChildNodes.ElementAt(1));
			}
			else {
				variables = new List<string>();
				variables.Add(sentencia.ChildNodes.ElementAt(1).Token.ValueString);
			}
				Expresion exp = null;
				if (sentencia.ChildNodes.Count == 3)
				{
					exp = GetExpresion(sentencia.ChildNodes.ElementAt(2));
				}
				return new Declaracion(variables, new TipoObjetoDB(tipo, nombreTipo), exp, sentencia.Span.Location.Line, sentencia.Span.Location.Column);
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

		private static Sentencia GetModificador(ParseTreeNode raiz)
		{
			return new Modificador((ModificadorExp)GetmodificadorExp(raiz));
		}

		private static Sentencia GetOperacionAsignacion(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 3)
			{
				//variable
				Acceso ac = new Acceso(sentencia.Span.Location.Line, sentencia.Span.Location.Column);
				ac.Objetos.Add(new AccesoPar(sentencia.ChildNodes.ElementAt(0).Token.ValueString,TipoAcceso.Variable));
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
				ac2.Objetos.Add(new AccesoPar(sentencia.ChildNodes.ElementAt(0).Token.ValueString,TipoAcceso.Variable));
				ac2.Objetos.AddRange(ac.Objetos);
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
		#endregion

		#region Sentencias DB
		private static Sentencia GetCrearUserType(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 2)
			{
				Dictionary<string, TipoObjetoDB> atributos = new Dictionary<string, TipoObjetoDB>();
				foreach (ParseTreeNode nodo in sentencia.ChildNodes.ElementAt(1).ChildNodes)
				{
					TipoDatoDB tipo = GetTipo(nodo.ChildNodes.ElementAt(1));
					string nombreTipo = GetNombreTipo(tipo, nodo.ChildNodes.ElementAt(1), true);
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
					TipoDatoDB tipo = GetTipo(nodo.ChildNodes.ElementAt(1));
					string nombreTipo = GetNombreTipo(tipo, nodo.ChildNodes.ElementAt(1), true);
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
					TipoDatoDB tipo = GetTipo(nodo.ChildNodes.ElementAt(1));
					string nombreTipo = GetNombreTipo(tipo, nodo.ChildNodes.ElementAt(1), true);
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
				if (ac != null) accesss.Objetos.Add(ac);
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
				TipoDatoDB t = GetTipo(nodo.ChildNodes.ElementAt(1));
				string nombreTipo = GetNombreTipo(t, nodo.ChildNodes.ElementAt(1), true);
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
				TipoDatoDB t = GetTipo(nodo.ChildNodes.ElementAt(1));
				string nombreTipo = GetNombreTipo(t, nodo.ChildNodes.ElementAt(1), true);
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
						object ob = GetValor(raiz.ChildNodes.ElementAt(0));
						if (ob != null)
							return (Expresion)ob;
					}
					if (raiz.ChildNodes.ElementAt(0).Term.Name.Equals("CONDICION"))
					{
						return GetCondicion(raiz.ChildNodes.ElementAt(0));
					}
					if (raiz.ChildNodes.ElementAt(0).Term.Name.Equals("TERNARIO"))
					{
						return GetTernario(raiz.ChildNodes.ElementAt(0));
					}
					break;
				case 3: //operaciones 
					return new Operacion(GetExpresion(raiz.ChildNodes.ElementAt(0)), GetExpresion(raiz.ChildNodes.ElementAt(2)), GetTipoOperacion(raiz.ChildNodes.ElementAt(1)), raiz.ChildNodes.ElementAt(1).Token.Location.Line, raiz.ChildNodes.ElementAt(1).Token.Location.Column);
			}
			return null;
		}

		private static Expresion GetTernario(ParseTreeNode parseTreeNode)
		{
			Condicion condicion = GetCondicion(parseTreeNode.ChildNodes.ElementAt(0));
			Expresion exp1 = GetExpresion(parseTreeNode.ChildNodes.ElementAt(1));
			Expresion exp2 = GetExpresion(parseTreeNode.ChildNodes.ElementAt(2));
			return new Ternario(condicion,exp1,exp2, parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column);
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
							MyDateTime di;
								if (DateTime.TryParse(raiz.ChildNodes.ElementAt(0).Token.ValueString.Replace("'", string.Empty), out DateTime dt)&&
								Regex.IsMatch(raiz.ChildNodes.ElementAt(0).Token.ValueString.ToString(), "'[0-9]{4}-[0-9]{2}-[0-9]{2}'"))
								{
									di= new MyDateTime(TipoDatoDB.DATE, dt);
								}
								else
								{
								di = new MyDateTime(TipoDatoDB.DATE, DateTime.Parse("0000-00-00"));
								Analizador.ErroresCQL.Add(new Error(TipoError.Advertencia,
											"La fecha es incorrecta, el formato debe ser AAAA-MM-DD",
										   raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column));
							}
							return new Operacion(di, TipoOperacion.Fecha, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
						case "time":
							if (DateTime.TryParse(raiz.ChildNodes.ElementAt(0).Token.ValueString.Replace("'", string.Empty), out DateTime dt1)&&
								Regex.IsMatch(raiz.ChildNodes.ElementAt(0).Token.ValueString.ToString(),"'[0-9]{2}:[0-9]{2}:[0-9]{2}'"))
							{
								di = new MyDateTime(TipoDatoDB.TIME, dt1);
							}
							else
							{
								di = new MyDateTime(TipoDatoDB.TIME, DateTime.Parse("00:00:00"));
								Analizador.ErroresCQL.Add(new Error(TipoError.Advertencia,
											"La hora es incorrecta, el formato debe ser HH:MM:SS",
										   raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column));
							}
							return new Operacion(di, TipoOperacion.Hora, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
						case "EXPRESION":
							return GetExpresion(raiz.ChildNodes.ElementAt(0));
						case "ACCESO":
							return GetAcceso(raiz.ChildNodes.ElementAt(0));
						case "LLAMADAFUNCION":
							return GetLlamadaFuncion(raiz.ChildNodes.ElementAt(0));
						case "INFOCOLLECTIONS": //{valor:{}:val:{}}
							return GetInfoCollection(raiz.ChildNodes.ElementAt(0));
						case "LISTAEXPRESIONES": //[1112,2,3,3,2]
							List<Expresion> lista = GetListaExpresiones(raiz.ChildNodes.ElementAt(0));
							return new Operacion(lista,
							TipoOperacion.ListaDatos,
							raiz.ChildNodes.ElementAt(0).Span.Location.Line,
							raiz.ChildNodes.ElementAt(0).Span.Location.Column);
						case "SETDATOS":
							lista = GetListaExpresiones(raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0));
							return new Operacion(lista,
							TipoOperacion.SetDatos,
							raiz.ChildNodes.ElementAt(0).Span.Location.Line,
							raiz.ChildNodes.ElementAt(0).Span.Location.Column);
						case "MODIFICADORES":
							return GetmodificadorExp(raiz.ChildNodes.ElementAt(0));
						case "FUNCIONAGREGACION":
							break;
						case "OBJETO":
							return GetObjeto(raiz.ChildNodes.ElementAt(0));

					}
					break;
				case 2://menos
					if (raiz.ChildNodes.ElementAt(0).Term.Name.Equals("-"))
					{
						return new Operacion(GetExpresion(raiz.ChildNodes.ElementAt(1)), TipoOperacion.Menos, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
					}
					else if (raiz.ChildNodes.ElementAt(0).Term.Name.ToLower().Equals("new"))
					{
						//new tipodato
						TipoDatoDB tipo = GetTipo(raiz.ChildNodes.ElementAt(1));
						string nombreTipo = GetNombreTipo(tipo, raiz.ChildNodes.ElementAt(1), true);

						return new Operacion(new TipoObjetoDB(tipo,nombreTipo), 
							TipoOperacion.NuevaInstancia, 
							raiz.ChildNodes.ElementAt(0).Token.Location.Line,
							raiz.ChildNodes.ElementAt(0).Token.Location.Column);

					} else if (raiz.ChildNodes.ElementAt(1).Term.Name.Equals("EXPRESION")) {
						//arreglo[]
						return new AccesoArreglo(GetExpresion(raiz.ChildNodes.ElementAt(1)), raiz.ChildNodes.ElementAt(0).Token.ValueString,
							raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
					}
					else
					{
						//id punto acceso
						Acceso acc = new Acceso(raiz.Span.Location.Line, raiz.Span.Location.Column);
						acc.Objetos.Add(new AccesoPar(raiz.ChildNodes.ElementAt(0).Token.ValueString, TipoAcceso.Variable));
						foreach (ParseTreeNode ac_campo in raiz.ChildNodes.ElementAt(1).ChildNodes)
						{
							AccesoPar ac = GetAcCampo(ac_campo);
							if (ac != null) acc.Objetos.Add(ac);
						}
						return acc;
					}
				case 3:
					//arreglo[val].algo
					Acceso aces = new Acceso(raiz.Span.Location.Line, raiz.Span.Location.Column);
					aces.Objetos.Add(new AccesoPar(new AccesoArreglo(GetExpresion(raiz.ChildNodes.ElementAt(1)), raiz.ChildNodes.ElementAt(0).Token.ValueString,
						raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column),
						TipoAcceso.AccesoArreglo));
					Acceso acceso2 = GetAcceso(raiz.ChildNodes.ElementAt(2));
					foreach (AccesoPar acceso in acceso2.Objetos) {
						aces.Objetos.Add(acceso);
					}
					return aces;
			}
			return null;
		}

		private static Expresion GetInfoCollection(ParseTreeNode raiz)
		{
			InfoCollection info = new InfoCollection();
			foreach (ParseTreeNode nodo in raiz.ChildNodes)
			{
				info.Add(new Info(GetExpresion(nodo.ChildNodes.ElementAt(0)), GetExpresion(nodo.ChildNodes.ElementAt(1))));
			}
			return new Operacion(info,
							TipoOperacion.MapDatos,
							raiz.ChildNodes.ElementAt(0).Span.Location.Line,
							raiz.ChildNodes.ElementAt(0).Span.Location.Column);
		}

		private static Expresion GetObjeto(ParseTreeNode raiz)
		{
			return new InstanciaObjeto(raiz.ChildNodes.ElementAt(2).Token.ValueString, GetListaExpresiones(raiz.ChildNodes.ElementAt(0)),
				raiz.ChildNodes.ElementAt(2).Token.Location.Line, raiz.ChildNodes.ElementAt(2).Token.Location.Column);
		}

		public static string GetNombreTipo(TipoDatoDB td, ParseTreeNode parseTreeNode, bool b)
		{
			switch (td)
			{
				case TipoDatoDB.LISTA_PRIMITIVO:
				case TipoDatoDB.LISTA_OBJETO:
					TipoDatoDB t;
					if (parseTreeNode.ChildNodes.Count != 1)
					{
						t = GetTipo(parseTreeNode.ChildNodes.ElementAt(2));
					string nombreTipo = GetNombreTipo(t, parseTreeNode.ChildNodes.ElementAt(2), false);
					if (!b) return "list<" + nombreTipo + ">";
					return nombreTipo;
					}
					else
					{
						return "null";
					}
				case TipoDatoDB.SET_PRIMITIVO:
				case TipoDatoDB.SET_OBJETO:
					if (parseTreeNode.ChildNodes.Count != 1)
					{
						t = GetTipo(parseTreeNode.ChildNodes.ElementAt(2));
					string nombreTipo = GetNombreTipo(t, parseTreeNode.ChildNodes.ElementAt(2), false);
					if (!b) return "set<" + nombreTipo + ">";
					return nombreTipo;
					}
					else
					{
						return "null";
					}
				case TipoDatoDB.MAP_PRIMITIVO:
				case TipoDatoDB.MAP_OBJETO:
					if (parseTreeNode.ChildNodes.Count != 1)
					{
						t = GetTipo(parseTreeNode.ChildNodes.ElementAt(2));
					string nombreTipo = GetNombreTipo(t, parseTreeNode.ChildNodes.ElementAt(2), false);

					TipoDatoDB t1;
					if (parseTreeNode.ChildNodes.Count != 1)
					{
						t1 = GetTipo(parseTreeNode.ChildNodes.ElementAt(3));
					}
					else
					{
						t1= TipoDatoDB.NULO;
					}
					string nombreTipo1 = GetNombreTipo(t1, parseTreeNode.ChildNodes.ElementAt(3), false);
					if (!b) return "map<" + nombreTipo + "," + nombreTipo1 + ">";
					return nombreTipo + "," + nombreTipo1;
					}
					else
					{
						return "null";
					}
				case TipoDatoDB.OBJETO:
					if (parseTreeNode.ChildNodes.Count == 4)
					{
						t = GetTipo(parseTreeNode.ChildNodes.ElementAt(2));
						string nombreTipo = GetNombreTipo(t, parseTreeNode.ChildNodes.ElementAt(2), false);
						return nombreTipo;
					}
					else
					{
						return parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString;
					}
				case TipoDatoDB.BOOLEAN:
					return "boolean";
				case TipoDatoDB.DATE:
					return "date";
				case TipoDatoDB.DOUBLE:
					return "double";
				case TipoDatoDB.INT:
					return "int";
				case TipoDatoDB.STRING:
					return "string";
				case TipoDatoDB.TIME:
					return "time";
				case TipoDatoDB.COUNTER:
					return "counter";
				case TipoDatoDB.NULO:
					return "nulo";
				default:
					return "";
			}
		}

		public static TipoDatoDB GetTipo(ParseTreeNode parseTreeNode)
		{
			switch (parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower())
				{
					case "string":
						return TipoDatoDB.STRING;
					case "int":
						return TipoDatoDB.INT;
					case "double":
						return TipoDatoDB.DOUBLE;
					case "boolean":
						return TipoDatoDB.BOOLEAN;
					case "date":
						return TipoDatoDB.DATE;
					case "time":
						return TipoDatoDB.TIME;
					case "counter":
						return TipoDatoDB.COUNTER;
					//listas
					case "list":
					TipoDatoDB ti;
					if (parseTreeNode.ChildNodes.Count != 1)
					{
						 ti= GetTipo(parseTreeNode.ChildNodes.ElementAt(2));
					}
					else
					{
						ti = TipoDatoDB.NULO;
					}
					if (Datos.IsPrimitivo(ti))
					{
						return TipoDatoDB.LISTA_PRIMITIVO;
					}
						return TipoDatoDB.LISTA_OBJETO;
					//sets
					case "set":
					if (parseTreeNode.ChildNodes.Count != 1)
					{
						ti = GetTipo(parseTreeNode.ChildNodes.ElementAt(2));
					}
					else
					{
						ti = TipoDatoDB.NULO;
					}
					if (Datos.IsPrimitivo(ti))
					{
						return TipoDatoDB.SET_PRIMITIVO;
					}
					return TipoDatoDB.SET_OBJETO;
				//maps
				case "map":
					if (parseTreeNode.ChildNodes.Count != 1)
					{
						ti = GetTipo(parseTreeNode.ChildNodes.ElementAt(3));
					}
					else {
						ti = TipoDatoDB.NULO;
					}
					
					if (Datos.IsPrimitivo(ti))
					{
						return TipoDatoDB.MAP_PRIMITIVO;
					}
					return TipoDatoDB.MAP_OBJETO;
				default:
					return TipoDatoDB.OBJETO;
			}
		}

		private static Expresion GetmodificadorExp(ParseTreeNode raiz)
		{
			if (raiz.ChildNodes.Count == 2)
			{
				bool op = raiz.ChildNodes.ElementAt(1).Token.ValueString == "++";
				Acceso acc = new Acceso(raiz.Span.Location.Line, raiz.Span.Location.Column);
				acc.Objetos.Add(new AccesoPar(raiz.ChildNodes.ElementAt(0).Token.ValueString, TipoAcceso.Variable));
				return new ModificadorExp(acc, op, raiz.ChildNodes.ElementAt(0).Token.Location.Line,
												raiz.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else
			{
				bool op = raiz.ChildNodes.ElementAt(2).Token.ValueString == "++";
				Acceso acces = GetAcceso(raiz.ChildNodes.ElementAt(1));
				Acceso nuevocceso = new Acceso(raiz.Span.Location.Line, raiz.Span.Location.Column);
				nuevocceso.Objetos.Add(new AccesoPar(raiz.ChildNodes.ElementAt(0).Token.ValueString, TipoAcceso.Variable));
				nuevocceso.Objetos.AddRange(acces.Objetos);
				return new ModificadorExp(nuevocceso, op, raiz.ChildNodes.ElementAt(0).Token.Location.Line,
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
