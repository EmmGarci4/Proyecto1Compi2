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
			return new BaseDatos(nodo.ChildNodes.ElementAt(0).Token.ValueString, objetosdb);
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
				Analizador.ErroresCHISON.AddRange(erroresInst);
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
						string nombreTipo = GetNombreTipo(t, nodo.ChildNodes.ElementAt(1));

							ret.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString,new TipoObjetoDB(t,nombreTipo));
						
						
					}
					catch (ArgumentException ex)
					{
						Console.WriteLine("ERROR LEYENDO DATOS DE PARAMETROS DE PROCEDIMIENTO");
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
						string nombreTipo = GetNombreTipo(t, nodo.ChildNodes.ElementAt(1));
						param.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString, new TipoObjetoDB(t, nombreTipo));
						

					}
					catch (ArgumentException ex)
					{
						Console.WriteLine("ERROR LEYENDO DATOS DE PARAMETROS DE PROCEDIMIENTO");
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
					string nombreTipo = GetNombreTipo(td,nodo.ChildNodes.ElementAt(1));
					
					atributos.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString, new TipoObjetoDB(td, nombreTipo));
				}
				catch (ArgumentException ex)
				{
					Console.WriteLine("ERROR YA EXISTE UN ATRIBUTO CON ESE NOMBRE");
				}
			}
			return atributos;
		}

		private static string GetNombreTipo(TipoDatoDB td, ParseTreeNode parseTreeNode)
		{
			if (td == TipoDatoDB.OBJETO)
			{
				return parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString;

			}
			else if (td == TipoDatoDB.LISTA_OBJETO)
			{
				string val = Regex.Replace(parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString, "list", String.Empty, RegexOptions.IgnoreCase);
				val = Regex.Replace(val, "<|>", String.Empty, RegexOptions.IgnoreCase);

				return val;
			}
			else if (td == TipoDatoDB.SET_OBJETO)
			{
				string val = Regex.Replace(parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString, "set",String.Empty,RegexOptions.IgnoreCase);
				val = Regex.Replace(val, "<|>", String.Empty, RegexOptions.IgnoreCase);

				return val;
			}
			else if (td == TipoDatoDB.MAP_OBJETO)
			{
				string val = Regex.Replace(parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString, "map", String.Empty, RegexOptions.IgnoreCase);
				val = Regex.Replace(val, "<|>", String.Empty, RegexOptions.IgnoreCase);

				return val;
			}
			else
			{
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
					tb.Insertar(fila);
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
						CollectionLista valores = GetListaDatos(nod.ChildNodes.ElementAt(1));
						datos.Add(nod.ChildNodes.ElementAt(0).Token.ValueString, valores);
					}
					else if (nod.ChildNodes.ElementAt(1).Term.Name.Equals("LISTA_DATATABLE")) {
						//ES UN OBJETO
						Dictionary<string, object> atributos = GetFila(nod.ChildNodes.ElementAt(1));
						datos.Add(nod.ChildNodes.ElementAt(0).Token.ValueString,new Objeto(atributos));
					}else {
						datos.Add(nod.ChildNodes.ElementAt(0).Token.ValueString, GetValor(nod.ChildNodes.ElementAt(1).Token.ValueString));

					}

				}
			}
			catch (ArgumentException ex)
			{
				Console.WriteLine("ERROR LEYENDO DATOS DE FILAS DE TABLA DE BASE DE DATOS");
				return null;
			}
			return datos;
		}

		private static CollectionLista GetListaDatos(ParseTreeNode parseTreeNode)
		{
			CollectionLista valores = new CollectionLista();
			foreach (ParseTreeNode nod in parseTreeNode.ChildNodes)
			{
				if (nod.ChildNodes.ElementAt(0).Term.Name != "OBJETO" && nod.ChildNodes.ElementAt(0).Term.Name != "LISTA_DATOS"&& nod.ChildNodes.ElementAt(0).Term.Name != "LISTA_DATATABLE")
				{
					//DATO primitivo
					valores.AddItem(GetValor(nod.ChildNodes.ElementAt(0).Token.ValueString));
				}
				else if (nod.ChildNodes.ElementAt(0).Term.Name == "LISTA_DATOS") {
					//lista
					valores.AddItem(GetListaDatos(nod.ChildNodes.ElementAt(0)));
				}else
				{
					//objeto
					Dictionary<string, object> atributos = GetFila(nod.ChildNodes.ElementAt(0));
					valores.AddItem(new Objeto(atributos));
				}
			}

			return valores;
		}

		private static object GetValor(string valueString)
		{
			if (valueString.Contains(".")) {
				//decimal
				if (double.TryParse(valueString, out double val1))
				{
					return val1;
				}
				else
				{
					return valueString;
				}
			}
			//entero
			if (int.TryParse(valueString, out int val2))
			{
				return val2;
			}
			
			//booleano
			else if (bool.TryParse(valueString, out bool val3))
			{
				return val3;
			}
			else
			{
				return valueString;
			}

		}

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
			string nombreTipo = GetNombreTipo(tipo, nodo.ChildNodes.ElementAt(1));
			if (!ExisteUserTypeEnDb(objetos, nodo.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0).Token.ValueString))
			{
				Console.WriteLine("ERROR NO EXISTE EL USERTYHPE PARA CREAR EL OBJETO");
				return null;
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

		private static TipoDatoDB GetTipo(ParseTreeNode parseTreeNode)
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
					return TipoDatoDB.LISTA_STRING;
				case "list<int>":
					return TipoDatoDB.LISTA_INT;
				case "list<double>":
					return TipoDatoDB.LISTA_DOUBLE;
				case "list<boolean>":
					return TipoDatoDB.LISTA_BOOLEAN;
				case "list<date>":
					return TipoDatoDB.LISTA_DATE;
				case "list<time>":
					return TipoDatoDB.LISTA_TIME;
				//sets
				case "set<string>":
					return TipoDatoDB.SET_STRING;
				case "set<int>":
					return TipoDatoDB.SET_INT;
				case "set<double>":
					return TipoDatoDB.SET_DOUBLE;
				case "set<boolean>":
					return TipoDatoDB.SET_BOOLEAN;
				case "set<date>":
					return TipoDatoDB.SET_DATE;
				case "set<time>":
					return TipoDatoDB.SET_TIME;
				//maps
				case "map<string>":
					return TipoDatoDB.MAP_STRING;
				case "map<int>":
					return TipoDatoDB.MAP_INT;
				case "map<double>":
					return TipoDatoDB.MAP_DOUBLE;
				case "map<boolean>":
					return TipoDatoDB.MAP_BOOLEAN;
				case "map<date>":
					return TipoDatoDB.MAP_DATE;
				case "map<time>":
					return TipoDatoDB.MAP_TIME;
				default:
					if (parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Contains("list<"))
					{
						return TipoDatoDB.LISTA_OBJETO;
					}
					else if (parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Contains("set<"))
					{
						return TipoDatoDB.SET_OBJETO;
					}
					else if (parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Contains("map<"))
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
					Console.WriteLine("ERROR NO EXISTE LA BASE DE DATOS");
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
