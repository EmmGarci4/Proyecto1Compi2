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

		//public static List<Sentencia> GetAST(ParseTreeNode raiz)
		//{
		//	return GetSentencias(raiz.ChildNodes.ElementAt(0));
		//}

		public static Expresion GetAST(ParseTreeNode raiz)
		{
			return GetExpresion(raiz.ChildNodes.ElementAt(0));
		}

		private static List<Sentencia> GetSentencias(ParseTreeNode parseTreeNode)
		{
			List<Sentencia> sentencias = new List<Sentencia>();
			foreach (ParseTreeNode sentencia in parseTreeNode.ChildNodes) {
				switch (sentencia.Term.Name) {
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
				}
			}
			return sentencias;
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
			else {
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
			return new FuncionAgregacion(sentencia.ChildNodes.ElementAt(0).Token.ValueString,(Seleccionar)GetSeleccionar(sentencia.ChildNodes.ElementAt(2)),
				sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
		}

		private static Sentencia GetBatch(ParseTreeNode sentencia)
		{
			Batch bt = new Batch(sentencia.Span.Location.Line,sentencia.Span.Location.Column);
			foreach (ParseTreeNode nodo in sentencia.ChildNodes) {
				switch (nodo.Term.Name) {
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
						Analizador.ErroresCQL.Add(new Error(TipoError.Semantico,"No se puede agregar una sentencia tipo SELECT en un bloque Batch",
							nodo.Span.Location.Line,nodo.Span.Location.Column));
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
			else {
				listaAccesos = null;
			}
			Seleccionar seleccionar = new Seleccionar(listaAccesos, sentencia.ChildNodes.ElementAt(1).Token.ValueString,
				sentencia.ChildNodes.ElementAt(1).Token.Location.Line, sentencia.ChildNodes.ElementAt(1).Token.Location.Column);

			GetPropiedadesSeleccionar(seleccionar,sentencia.ChildNodes.ElementAt(2));
			return seleccionar;
		}

		private static void GetPropiedadesSeleccionar(Seleccionar seleccionar, ParseTreeNode parseTreeNode)
		{
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes) {
				switch (nodo.Term.Name) {
					case "PROPIEDADDONDE":
						if (seleccionar.PropiedadWhere == null)
						{
							Where wh = GetWhere(nodo);
							if (wh != null) seleccionar.PropiedadWhere = wh;
						}
						else {
							Analizador.ErroresCQL.Add(new Error(TipoError.Semantico,"No se puede agregar dos clausulas Where a una sentencia" +
								" de selección", nodo.Span.Location.Line,nodo.Span.Location.Column));
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
			OrderBy order = new OrderBy(nodo.Span.Location.Line,nodo.Span.Location.Column);
			foreach (ParseTreeNode nodito in nodo.ChildNodes) {
				bool isAsc = nodito.ChildNodes.ElementAt(1).Token.ValueString.Equals("asc");
				order.Propiedades.Add(new PropOrderBy(GetAcceso(nodito.ChildNodes.ElementAt(0)),isAsc));
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
			else {
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
			else {
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
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes) {
				lista.Add(new AsignacionColumna(GetAcceso(nodo.ChildNodes.ElementAt(0)),GetExpresion(nodo.ChildNodes.ElementAt(1))));
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
			else {
				//insercion especial
				return new Insertar(sentencia.ChildNodes.ElementAt(0).Token.ValueString,GetListaStrings(sentencia.ChildNodes.ElementAt(1)),
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
				foreach (ParseTreeNode nodo in sentencia.ChildNodes.ElementAt(2).ChildNodes) {
					TipoDatoDB tipo = GeneradorDB.GetTipo(nodo.ChildNodes.ElementAt(1));
					string nombreTipo = GeneradorDB.GetNombreTipo(tipo, nodo.ChildNodes.ElementAt(1),true);
					try {
						atributos.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString, new TipoObjetoDB(tipo, nombreTipo));
					}
					catch (ArgumentException ex) {
						Analizador.ErroresCQL.Add(new Error(TipoError.Semantico, "No se pueden agregar dos atributos con el mismo nombre",
							nodo.ChildNodes.ElementAt(0).Token.Location.Line,
							nodo.ChildNodes.ElementAt(0).Token.Location.Column));
					}
				}
				return new AlterarUserType(TipoAccion.Agregar, sentencia.ChildNodes.ElementAt(0).Token.ValueString,
					atributos, sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else {
				//borrar
				List<Acceso> accesos = GetListaAcceso(sentencia.ChildNodes.ElementAt(1));
				return new AlterarUserType(TipoAccion.Quitar,sentencia.ChildNodes.ElementAt(0).Token.ValueString,accesos,
					sentencia.ChildNodes.ElementAt(0).Token.Location.Line, sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
		}

		private static List<Acceso> GetListaAcceso(ParseTreeNode parseTreeNode)
		{
			List<Acceso> lista = new List<Acceso>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes) {
				lista.Add(GetAcceso(nodo));
			}
			return lista;
		}

		private static Acceso GetAcceso(ParseTreeNode nodo)
		{
			Acceso accesss = new Acceso(nodo.Span.Location.Line,nodo.Span.Location.Column);
			foreach (ParseTreeNode ac_campo in nodo.ChildNodes) {
				object ac = GetAcCampo(ac_campo);
				if(ac!=null)accesss.Objetos.Enqueue(ac);	
			}
			return accesss;
		}

		private static object GetAcCampo(ParseTreeNode ac_campo)
		{
			if (ac_campo.ChildNodes.Count == 1)
			{
				//nombre
				return ac_campo.ChildNodes.ElementAt(0).Token.ValueString;
			}
			else if (ac_campo.ChildNodes.Count == 2)
			{
				if (ac_campo.ChildNodes.ElementAt(1).Term.Name == "LISTAEXPRESIONES")
				{
					//llamada a funcion
					return new LlamadaFuncion(ac_campo.ChildNodes.ElementAt(0).Token.ValueString,
						GetListaExpresiones(ac_campo.ChildNodes.ElementAt(1)), ac_campo.ChildNodes.ElementAt(0).Token.Location.Line,
						ac_campo.ChildNodes.ElementAt(0).Token.Location.Column);
				}
				else
				{
					//acceso a arreglo
					return new AccesoArreglo(GetExpresion(ac_campo.ChildNodes.ElementAt(1)),
						ac_campo.ChildNodes.ElementAt(0).Token.ValueString,
						ac_campo.ChildNodes.ElementAt(0).Token.Location.Line, ac_campo.ChildNodes.ElementAt(0).Token.Location.Column);
				}

			}
			return null;
		}

		private static List<Expresion> GetListaExpresiones(ParseTreeNode parseTreeNode)
		{
			List<Expresion> lista = new List<Expresion>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes) {
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
			else {
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

		private static Sentencia GetCrearTabla(ParseTreeNode sentencia)
		{
			if (sentencia.ChildNodes.Count == 2) {
				//sin if-exist
				List<object> cls = GetColumnasTabla(sentencia.ChildNodes.ElementAt(1));
				return new CrearTabla(sentencia.ChildNodes.ElementAt(0).Token.ValueString, cls, false, 
					sentencia.ChildNodes.ElementAt(0).Token.Location.Line,
				   sentencia.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			else{
				//con if-exist
				List<object> cls = GetColumnasTabla(sentencia.ChildNodes.ElementAt(2));
				return new CrearTabla(sentencia.ChildNodes.ElementAt(1).Token.ValueString, cls, true, sentencia.ChildNodes.ElementAt(1).Token.Location.Line,
				   sentencia.ChildNodes.ElementAt(1).Token.Location.Column);
			}
				
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
				string nombreTipo = GeneradorDB.GetNombreTipo(t, nodo.ChildNodes.ElementAt(1),true);
				return new Columna(nodo.ChildNodes.ElementAt(0).Token.ValueString, new TipoObjetoDB(t, nombreTipo), false);
			} else if (nodo.ChildNodes.Count==3) {
				//llave primaria compuesta
				return GetListaStrings(nodo.ChildNodes.ElementAt(2));
			}
			else if (nodo.ChildNodes.Count == 4) {
				//columna con llave primaria
				TipoDatoDB t = GeneradorDB.GetTipo(nodo.ChildNodes.ElementAt(1));
				string nombreTipo = GeneradorDB.GetNombreTipo(t, nodo.ChildNodes.ElementAt(1),true);
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
			else {
				return new CrearBaseDatos(sentencia.ChildNodes.ElementAt(1).Token.ValueString, true,
					sentencia.ChildNodes.ElementAt(1).Token.Location.Line, sentencia.ChildNodes.ElementAt(1).Token.Location.Column);
			}
		}
		#endregion
	
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

		private static Condicion GetCondicion(ParseTreeNode raiz)
		{
			switch (raiz.ChildNodes.Count) {
				case 1://true o false
					return new Condicion(new Operacion(raiz.ChildNodes.ElementAt(0).Token.ValueString.ToLower(),TipoOperacion.Booleano,
						raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column), TipoOperacion.Booleano, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
				case 2://not
					return new Condicion(GetCondicion(raiz.ChildNodes.ElementAt(1)),TipoOperacion.Not, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
				case 3://operaciones
					return new Condicion(GetExpresion(raiz.ChildNodes.ElementAt(0)), GetExpresion(raiz.ChildNodes.ElementAt(2)), GetTipoOperacion(raiz.ChildNodes.ElementAt(1)), raiz.ChildNodes.ElementAt(1).Token.Location.Line, raiz.ChildNodes.ElementAt(1).Token.Location.Column);
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
				case "%": return TipoOperacion.Modulo;
			}
			return TipoOperacion.Nulo;
		}
		
		private static Expresion GetValor(ParseTreeNode raiz)
		{
			switch(raiz.ChildNodes.Count) {
				case 1://valores, o expresion o llamada
					switch (raiz.ChildNodes.ElementAt(0).Term.Name) {
					case "NULL":
							return new Operacion("null", TipoOperacion.Nulo, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
					case "numero":
							return new Operacion(Datos.GetValor(raiz.ChildNodes.ElementAt(0).Token.ValueString), TipoOperacion.Numero,raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
					case "cadena":
							return new Operacion(raiz.ChildNodes.ElementAt(0).Token.ValueString, TipoOperacion.Cadena, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
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
					if (raiz.ChildNodes.ElementAt(0).Term.Name.Equals("menos"))
					{
						return new Operacion(GetExpresion(raiz.ChildNodes.ElementAt(0)), TipoOperacion.Menos, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);
					}
					else if (raiz.ChildNodes.ElementAt(0).Term.Name.Equals("new"))
					{
						//new tipodato
						return new Operacion(raiz.ChildNodes.ElementAt(1).Token.ValueString, TipoOperacion.Objeto, raiz.ChildNodes.ElementAt(0).Token.Location.Line, raiz.ChildNodes.ElementAt(0).Token.Location.Column);

					}
					else {
						//id punto acceso
						Acceso acc = new Acceso(raiz.Span.Location.Line,raiz.Span.Location.Column);
						acc.Objetos.Enqueue(raiz.ChildNodes.ElementAt(0).Token.ValueString);
						foreach (ParseTreeNode ac_campo in raiz.ChildNodes.ElementAt(1).ChildNodes)
						{
							object ac = GetAcCampo(ac_campo);
							if (ac != null) acc.Objetos.Enqueue(ac);
						}
						return acc;
					}	
			}
			return null;
		}

		private static Expresion GetmodificadorExp(ParseTreeNode raiz)
		{
			bool op = raiz.ChildNodes.ElementAt(1).Token.ValueString == "+";
							return new ModificadorExp(raiz.ChildNodes.ElementAt(0).Token.ValueString,op,raiz.ChildNodes.ElementAt(0).Token.Location.Line,
								raiz.ChildNodes.ElementAt(0).Token.Location.Column);
		}

		private static Expresion GetLlamadaFuncion(ParseTreeNode parseTreeNode)
		{
			return new LlamadaFuncionExp(parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString,GetListaExpresiones(parseTreeNode.ChildNodes.ElementAt(1)),
				parseTreeNode.Span.Location.Line,parseTreeNode.Span.Location.Column);
		}
		#endregion
	}
}
