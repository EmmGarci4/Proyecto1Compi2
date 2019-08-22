using com.Analisis;
using com.Analisis.Util;
using Irony.Parsing;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Proyecto1Compi2.com.Analisis
{
	class GeneradorDB
	{
		public static void GuardarInformación(ParseTreeNode raiz)
		{
			//raiz
			//databases=0-- users=1
			GuardarBasesDeDatos(raiz.ChildNodes.ElementAt(0));
			GuardarUsuarios(raiz.ChildNodes.ElementAt(1));
		}

		private static void GuardarBasesDeDatos(ParseTreeNode raiz)
		{
			foreach (ParseTreeNode nodo in raiz.ChildNodes)
			{

				BaseDatos db = GetBaseDatos(nodo);
				Analizador.AddBaseDatos(db);
			}
		}

		private static BaseDatos GetBaseDatos(ParseTreeNode nodo)
		{
			List<object> objetosdb = GetObjetosDb(nodo.ChildNodes.ElementAt(1));
			BaseDatos db=new  BaseDatos(nodo.ChildNodes.ElementAt(0).Token.ValueString);
			foreach (object obj in objetosdb) {
				AddDbObj(obj, db, nodo.ChildNodes.ElementAt(0).Token.Location.Line, nodo.ChildNodes.ElementAt(0).Token.Location.Column);
			}
			return db;
		}

		private static void AddDbObj(object obj, BaseDatos db,int linea,int columna)
		{
				if (obj is Tabla)
				{
					if (!db.ExisteTabla(((Tabla)obj).Nombre))
					{
						db.AgregarTabla((Tabla)obj);
					}
					else
					{
						//INSERTANDO ERROR EN TABLA ERRORS
						
						Analizador.ErroresChison.Add(new Error(

							TipoError.Semantico,
							"La tabla '"+((Tabla)obj).Nombre+"' ya existe en la base de datos '"+db.Nombre+"'",
							linea+1,
							columna+1,
							Datos.GetDate(), //fecha
							Datos.GetTime()//hora
						));

					}
				}
				else if (obj is UserType)
				{
					if (!db.ExisteUserType(((UserType)obj).Nombre))
					{
						db.AgregarUserType((UserType)obj);
					}
					else
					{
						//INSERTANDO ERROR EN TABLA ERRORS
						Analizador.ErroresChison.Add(new Error(

							TipoError.Semantico,
							"El user type '"+((UserType)obj).Nombre+"' ya existe en la base de datos '"+db.Nombre+"'",
							linea+1,
							columna+1,
							Datos.GetDate(), //fecha
							Datos.GetTime()//hora
						));
					}
				}
				else if (obj is Procedimiento)
				{
					if (!db.ExisteProcedimiento(((Procedimiento)obj).Nombre))
					{
						db.AgregarProcedimiento((Procedimiento)obj);
					}
					else
					{
						//INSERTANDO ERROR EN TABLA ERRORS
						Analizador.ErroresChison.Add(new Error(

							TipoError.Semantico,
							"El procedimiento '"+((Procedimiento)obj).Nombre+"' ya existe en la base de datos '"+db.Nombre+"'",
							linea+1,
							columna+1,
							Datos.GetDate(), //fecha
							Datos.GetTime()//hora
						));
					}

				}
		}

		private static List<object> GetObjetosDb(ParseTreeNode raiz)
		{
			List<object> objetosdb = new List<object>();
			foreach (ParseTreeNode nodo in raiz.ChildNodes)
			{
				switch (nodo.Term.Name)
				{
					case "TABLA":
						Tabla tb = GetTabla(objetosdb,nodo);
						//tb.MostrarCabecera();
						//tb.MostrarDatos();
						objetosdb.Add(tb);
						break;
					case "OBJETO":
						UserType obj = GetObjeto(nodo);
						//obj.Mostrar();
						objetosdb.Add(obj);
						break;
					case "PROCEDIMIENTO":
						Procedimiento proc = GetProcedimiento(nodo);
						//proc.Mostrar();
						objetosdb.Add(proc);
						break;
				}

			}
			return objetosdb;
		}

		private static Procedimiento GetProcedimiento(ParseTreeNode nodo)
		{
			Dictionary<string, TipoObjetoDB> par = GetParametros(nodo.ChildNodes.ElementAt(1));
			Dictionary<string, TipoObjetoDB> ret = GeRetornos(nodo.ChildNodes.ElementAt(1));
			String codigo = nodo.ChildNodes.ElementAt(2).Token.ValueString;
			codigo = codigo.TrimStart('$');
			codigo = codigo.TrimEnd('$');
			Analizador.AnalizarCql(codigo);
			List<Error> erroresInst = (Analizador.ErroresCQL);
			//cambiar el numero de linea 
			if (erroresInst.Count>0) {
				//Analizador.ErroresCHISON.AddRange(erroresInst);
				codigo = "//SE ENCONTRARON ERRORES EN EL CODIGO\n";
			}
			return new Procedimiento(nodo.ChildNodes.ElementAt(0).Token.ValueString, par, ret, codigo);
		}

		private static Dictionary<string, TipoObjetoDB> GeRetornos(ParseTreeNode parseTreeNode)
		{
			Dictionary<string, TipoObjetoDB> ret = new Dictionary<string, TipoObjetoDB>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
			{
				if (nodo.ChildNodes.ElementAt(2).Token.ValueString.ToLower().Equals("out"))
				{
					try
					{
						TipoDatoDB t = GetTipo(nodo.ChildNodes.ElementAt(1));
						string nombreTipo = GetNombreTipo(t, nodo.ChildNodes.ElementAt(1),true);

							ret.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString,new TipoObjetoDB(t,nombreTipo));
						
						
					}
					catch (ArgumentException ex)
					{
						//INSERTANDO ERROR EN TABLA ERRORS
						Analizador.ErroresChison.Add(new Error(

							TipoError.Semantico,
							"Error grave leyendo datos en retornos del procedimiento",
							nodo.Span.Location.Line+1,
							nodo.Span.Location.Column+1,
							Datos.GetDate(), //fecha
							Datos.GetTime()//hora
						));
					}
				}
			}
			return ret;
		}

		private static Dictionary<string, TipoObjetoDB> GetParametros(ParseTreeNode parseTreeNode)
		{
			Dictionary<string, TipoObjetoDB> param = new Dictionary<string, TipoObjetoDB>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
			{
				if (nodo.ChildNodes.ElementAt(2).Token.ValueString.ToLower().Equals("in"))
				{
					try
					{
						TipoDatoDB t = GetTipo(nodo.ChildNodes.ElementAt(1));
						string nombreTipo = GetNombreTipo(t, nodo.ChildNodes.ElementAt(1),true);
						param.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString, new TipoObjetoDB(t, nombreTipo));
						

					}
					catch (ArgumentException ex)
					{
						//INSERTANDO ERROR EN TABLA ERRORS
						Analizador.ErroresChison.Add(new Error(

							TipoError.Semantico,
							"Error grave leyendo datos en parametros del procedimiento",
							nodo.Span.Location.Line+1,
							nodo.Span.Location.Column+1,
							Datos.GetDate(), //fecha
							Datos.GetTime()//hora
						));
					}
				}
			}
			return param;
		}

		private static UserType GetObjeto(ParseTreeNode nodo)
		{
			return new UserType(nodo.ChildNodes.ElementAt(0).Token.ValueString,
				GetAtributos(nodo.ChildNodes.ElementAt(1)));
		}

		private static Dictionary<string, TipoObjetoDB> GetAtributos(ParseTreeNode parseTreeNode)
		{
			Dictionary<string, TipoObjetoDB> atributos = new Dictionary<string, TipoObjetoDB>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
			{
				try
				{
					TipoDatoDB td = GetTipo(nodo.ChildNodes.ElementAt(1));
					string nombreTipo = GetNombreTipo(td,nodo.ChildNodes.ElementAt(1),true);
					
					atributos.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString, new TipoObjetoDB(td, nombreTipo));
				}
				catch (ArgumentException ex)
				{
					//INSERTANDO ERROR EN TABLA ERRORS
					Analizador.ErroresChison.Add(new Error(

							TipoError.Semantico,
							"Ya existe el atributo '"+nodo.ChildNodes.ElementAt(0).Token.ValueString+"' en el User Type",
							nodo.Span.Location.Line+1,
							nodo.Span.Location.Column+1,
							Datos.GetDate(), //fecha
							Datos.GetTime()//hora
						));

				}
			}
			return atributos;
		}

		public static string GetNombreTipo(TipoDatoDB td, ParseTreeNode parseTreeNode,bool b)
		{
			switch (td) {
				case TipoDatoDB.LISTA_PRIMITIVO:
				case TipoDatoDB.LISTA_OBJETO:
					TipoDatoDB t = GetTipo(parseTreeNode.ChildNodes.ElementAt(2));
					string nombreTipo = GetNombreTipo(t, parseTreeNode.ChildNodes.ElementAt(2), false);
					if (!b) return "list<" + nombreTipo + ">";
					return nombreTipo;
				case TipoDatoDB.SET_PRIMITIVO:
				case TipoDatoDB.SET_OBJETO:
					t = GetTipo(parseTreeNode.ChildNodes.ElementAt(2));
					nombreTipo = GetNombreTipo(t, parseTreeNode.ChildNodes.ElementAt(2), false);
					if (!b) return "set<" + nombreTipo + ">";
					return nombreTipo;
				case TipoDatoDB.MAP_PRIMITIVO:
				case TipoDatoDB.MAP_OBJETO:
					t = GetTipo(parseTreeNode.ChildNodes.ElementAt(2));
					nombreTipo = GetNombreTipo(t, parseTreeNode.ChildNodes.ElementAt(2), false);
					
					TipoDatoDB t1 = GetTipo(parseTreeNode.ChildNodes.ElementAt(3));
					string nombreTipo1 = GetNombreTipo(t1, parseTreeNode.ChildNodes.ElementAt(3), false);
					if (!b) return "map<" + nombreTipo + "," + nombreTipo1 + ">";
					return nombreTipo + "," + nombreTipo1;
				case TipoDatoDB.OBJETO:
					if (parseTreeNode.ChildNodes.Count == 4)
					{
						t = GetTipo(parseTreeNode.ChildNodes.ElementAt(2));
						nombreTipo = GetNombreTipo(t, parseTreeNode.ChildNodes.ElementAt(2), false);
						return nombreTipo;

					}
					else {
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

		private static Tabla GetTabla(List<object> objetosdb,ParseTreeNode nodo)
		{
			Tabla tb = new Tabla(nodo.ChildNodes.ElementAt(0).Token.ValueString, GetColumnas(objetosdb, nodo.ChildNodes.ElementAt(1)));
			  //agregando datos a la tabla
			AddDataTabla(tb, nodo.ChildNodes.ElementAt(2));
			return tb;
		}

		private static void AddDataTabla(Tabla tb, ParseTreeNode raiz)
		{
			foreach (ParseTreeNode nodo in raiz.ChildNodes)
			{
				Dictionary<string, object> fila = GetFila(nodo);
				if (fila != null)
				{
					//List<Error> er =tb.Insertar(fila,nodo.Span.Location.Line,nodo.Span.Location.Column);
					//Analizador.ErroresChison.AddRange(er);
				}
			}
		}

		private static Dictionary<string, object> GetFila(ParseTreeNode fila)
		{
			Dictionary<string, object> datos = new Dictionary<string, object>();
			try
			{
				foreach (ParseTreeNode nod in fila.ChildNodes)
				{
					if (nod.ChildNodes.ElementAt(1).Term.Name.Equals("LISTA_DATOS"))
					{
						//ES UNA LISTA
						//CollectionLista valores = GetListaDatos(nod.ChildNodes.ElementAt(1));
						//datos.Add(nod.ChildNodes.ElementAt(0).Token.ValueString, valores);
					}
					else if (nod.ChildNodes.ElementAt(1).Term.Name.Equals("LISTA_DATATABLE")) {
						//ES UN OBJETO
						Dictionary<string, object> atributos = GetFila(nod.ChildNodes.ElementAt(1));
						//datos.Add(nod.ChildNodes.ElementAt(0).Token.ValueString,new Objeto(atributos));
					}else {
						datos.Add(nod.ChildNodes.ElementAt(0).Token.ValueString, Datos.GetValor(nod.ChildNodes.ElementAt(1).Token.ValueString));

					}

				}
			}
			catch (ArgumentException ex)
			{
				//INSERTANDO ERROR EN TABLA ERRORS
				Analizador.ErroresChison.Add(new Error(
							TipoError.Semantico,
							"Error grave al leer los datos de la tabla ",
							fila.Span.Location.Line+1,
							fila.Span.Location.Column+1,
							Datos.GetDate(), //fecha
							Datos.GetTime()//hora
						));
				return null;
			}
			return datos;
		}

		//private static CollectionLista GetListaDatos(ParseTreeNode parseTreeNode)
		//{
		//	CollectionLista valores = new CollectionLista();
		//	foreach (ParseTreeNode nod in parseTreeNode.ChildNodes)
		//	{
		//		if (nod.ChildNodes.ElementAt(0).Term.Name != "OBJETO" && nod.ChildNodes.ElementAt(0).Term.Name != "LISTA_DATOS"&& nod.ChildNodes.ElementAt(0).Term.Name != "LISTA_DATATABLE")
		//		{
		//			//DATO primitivo
		//			valores.SetItem(Datos.GetValor(nod.ChildNodes.ElementAt(0).Token.ValueString));
		//		}
		//		else if (nod.ChildNodes.ElementAt(0).Term.Name == "LISTA_DATOS") {
		//			//lista
		//			valores.SetItem(GetListaDatos(nod.ChildNodes.ElementAt(0)));
		//		}else
		//		{
		//			//objeto
		//			Dictionary<string, object> atributos = GetFila(nod.ChildNodes.ElementAt(0));
		//			valores.SetItem(new Objeto(atributos));
		//		}
		//	}

		//	return valores;
		//}

		private static List<Columna> GetColumnas(List<object> objetosdb,ParseTreeNode raiz)
		{
			List<Columna> columnas = new List<Columna>();
			foreach (ParseTreeNode nodo in raiz.ChildNodes)
			{
				Columna cl = GetColumna(objetosdb,nodo);
				if (cl!=null) {
					columnas.Add(cl);
				}
			}
			return columnas;
		}

		private static Columna GetColumna(List<object> objetos,ParseTreeNode nodo)
		{
			TipoDatoDB tipo = GetTipo(nodo.ChildNodes.ElementAt(1));
			string nombreTipo = GetNombreTipo(tipo, nodo.ChildNodes.ElementAt(1),true);
			if (tipo==TipoDatoDB.OBJETO) {
				if (!ExisteUserTypeEnDb(objetos, nombreTipo))
				{
					//INSERTANDO ERROR EN TABLA ERRORS
					Analizador.ErroresChison.Add(new Error(

							TipoError.Semantico,
							"No existe el User Type '"+nombreTipo+"' para crear el objeto",
							nodo.Span.Location.Line+1,
							nodo.Span.Location.Column+1,
							Datos.GetDate(), //fecha
							Datos.GetTime()//hora
						));

					return null;
				}
			}
				return new Columna(nodo.ChildNodes.ElementAt(0).Token.ValueString,
					new TipoObjetoDB(tipo, nombreTipo),
					IsPk(nodo.ChildNodes.ElementAt(2)));
			}

		private static bool ExisteUserTypeEnDb(List<object> objetos, string v)
		{
			foreach (object ob in objetos)
			{
				if (ob.GetType() == typeof(UserType))
				{
					if (((UserType)ob).Nombre.Equals(v))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool IsPk(ParseTreeNode parseTreeNode)
		{
			if (bool.TryParse(parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString, out bool res))
			{
				return res;
			}
			return false;
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
				case "list<string>":
				case "list<int>":
				case "list<double>":
				case "list<boolean>":
				case "list<date>":
				case "list<time>":
					return TipoDatoDB.LISTA_PRIMITIVO;
				//sets
				case "set<string>":
				case "set<int>":
				case "set<double>":
				case "set<boolean>":
				case "set<date>":
				case "set<time>":
					return TipoDatoDB.SET_PRIMITIVO;
				//maps
				case "map<string>":
				case "map<int>":
				case "map<double>":
				case "map<boolean>":
				case "map<date>":
				case "map<time>":
					return TipoDatoDB.MAP_PRIMITIVO;
				default:
					if (parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Equals("list")||
						parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower().StartsWith("list"))
					{
						return TipoDatoDB.LISTA_OBJETO;
					}
					else if (parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Equals("set")||
						parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower().StartsWith("set"))
					{
						return TipoDatoDB.SET_OBJETO;
					}
					else if (parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Equals("map")||
						parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower().StartsWith("map"))
					{
						return TipoDatoDB.MAP_OBJETO;
					}

					return TipoDatoDB.OBJETO;
			}
		}

		private static void GuardarUsuarios(ParseTreeNode lista_usuarios)
		{
			foreach (ParseTreeNode usuario in lista_usuarios.ChildNodes)
			{
				Usuario usu = GetUsuario(usuario);
				Analizador.AddUsuario(usu);
			}
		}

		private static Usuario GetUsuario(ParseTreeNode raiz)
		{
			List<string> listadb = GetListaOtroNombre(raiz.ChildNodes.ElementAt(2));
			List<string> permisos = new List<string>();
			foreach (string per in listadb)
			{
				if (Analizador.ExisteDB(per)) {
					permisos.Add(per);
				} else
				{
					//INSERTANDO ERROR EN TABLA ERRORS
					Analizador.ErroresChison.Add(new Error(
							TipoError.Semantico,
							"No se puede asignar permisos al usuario pues La base de datos '"+per+"' no existe",
							raiz.Span.Location.Line+1,
							raiz.Span.Location.Column+1,
							Datos.GetDate(), //fecha
							Datos.GetTime()//hora
						));
				}
			}
			return new Usuario(raiz.ChildNodes.ElementAt(0).Token.ValueString,
				raiz.ChildNodes.ElementAt(1).Token.ValueString, permisos);
		}

		private static List<string> GetListaOtroNombre(ParseTreeNode lista_nombres)
		{
			List<string> nombres = new List<string>();
			foreach (ParseTreeNode nodo in lista_nombres.ChildNodes)
			{
				nombres.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString);
			}
			return nombres;
		}
	}
}
