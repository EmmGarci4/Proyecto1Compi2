using com.Analisis;
using com.Analisis.Util;
using Irony.Parsing;
using Proyecto1Compi2.com.AST;
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
		static List<Error> erroresChison = new List<Error>();
		static Dictionary<int, int> intervalos = new Dictionary<int, int>();

		public static List<Error> ErroresChison { get => erroresChison; set => erroresChison = value; }

		public static bool AnalizarChison(String texto)
		{
			GramaticaChison gramatica = new GramaticaChison();
			LanguageData ldata = new LanguageData(gramatica);
			Parser parser = new Parser(ldata);
			//IMPORTAR 
			texto = Importar(texto);
			ParseTree arbol = parser.Parse(texto);
			ParseTreeNode raiz = arbol.Root;

			if (raiz != null && arbol.ParserMessages.Count == 0)
			{
				//generadorDOT.GenerarDOT(raiz, "C:\\Users\\Emely\\Desktop\\chison.dot");
				GeneradorDB.GuardarInformación(raiz);
				Analizador.MostrarReporteDeEstadoChison();
			}
			foreach (Irony.LogMessage mensaje in arbol.ParserMessages)
			{
				//INSERTANDO ERROR EN ErroresChison
				ErroresChison.Add(new Error(
					TipoError.Semantico,
					mensaje.Message,
					mensaje.Location.Line,
					mensaje.Location.Column,
					Datos.GetDate(),
					Datos.GetTime()
					));
			}
			LlenarTablaErrors();
			//Console.WriteLine(errors.ToString());
			return raiz != null && arbol.ParserMessages.Count == 0 && ErroresChison.Count == 0;
		}

		private static string Importar(string texto)
		{
			intervalos.Clear();
			int contadorLineas = 1;
			string[] lineas = texto.Split('\n');

			foreach (string linea in lineas)
			{
				Match match2 = Regex.Match(linea, "\\${.*}\\$");
				if (match2.Success)
				{
					String t1 = HandlerFiles.AbrirArchivo(GetURL(match2.Value));
					if (t1 != null)
					{
						texto = texto.Replace(match2.Value, t1);
						intervalos.Add(contadorLineas, t1.Split('\n').Length + contadorLineas);
					}
					else
					{
						texto = texto.Replace(linea, String.Empty);
						erroresChison.Add(new Error(TipoError.Advertencia, "El archivo '" + match2.Value + "' no existe en la carpeta data",
							contadorLineas, 1,
							Datos.GetDate(), Datos.GetTime()));
					}
				}
				contadorLineas++;
			}
			return texto;
		}

		private static string GetURL(string value)
		{
			value = value.Replace("$", String.Empty);
			value = value.Replace("{", String.Empty);
			value = value.Replace("}", String.Empty);
			value = value.Replace(" ", String.Empty);
			//agregando path directo
			return value;
		}

		private static void LlenarTablaErrors()
		{
			Analizador.Errors.Truncar();
			Queue<object> valores = new Queue<object>();
			foreach (Error error in erroresChison)
			{
				int linea = GetLineaError(error.Linea);
				String mensajeExtra = "";
				if (linea != error.Linea)
				{
					mensajeExtra = "(línea:" + GetLineaRealError(error.Linea) + " en archivo)";
				}
				valores.Clear();
				int contador = Analizador.Errors.BuscarColumna("numero").GetUltimoValorCounter();
				valores.Enqueue(contador + 1);
				valores.Enqueue(error.Tipo.ToString());
				valores.Enqueue(error.Mensaje + mensajeExtra);
				valores.Enqueue(linea);
				valores.Enqueue(error.Columna);
				valores.Enqueue(new MyDateTime(TipoDatoDB.DATE, DateTime.Parse(error.Fecha)));
				valores.Enqueue(new MyDateTime(TipoDatoDB.TIME, DateTime.Parse(error.Hora)));
				//agregando valores
				Analizador.Errors.AgregarValores(valores);
			}
		}

		private static int GetLineaRealError(int line)
		{
			if (intervalos.Count > 0)
			{

				foreach (KeyValuePair<int, int> intervalo in intervalos)
				{
					if (line >= intervalo.Key && line <= intervalo.Value)
					{
						return line - intervalo.Key;
					}
				}
			}
			return line;
		}

		private static int GetLineaError(int line)
		{
			if (intervalos.Count > 0)
			{

				foreach (KeyValuePair<int, int> intervalo in intervalos)
				{
					if (line > intervalo.Key && line < intervalo.Value)
					{
						return intervalo.Key;
					}
				}
			}
			return line;
		}

		public static void GuardarInformación(ParseTreeNode raiz)
		{
			//RECORRIENDO LA ESTRUCTURA PRINCIPAL
			//validando y obteniendo nodos 
			int indexDb = -1;
			int indexUs = -1;
			foreach (ParseTreeNode nodo in raiz.ChildNodes.ElementAt(0).ChildNodes)
			{
				//VALIDANDO
				switch (nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower())
				{
					case "databases":
						if (indexDb < 0)
						{
							indexDb = raiz.ChildNodes.ElementAt(0).ChildNodes.IndexOf(nodo);
						}
						break;
					case "users":
						if (indexUs < 0)
						{
							indexUs = raiz.ChildNodes.ElementAt(0).ChildNodes.IndexOf(nodo);
						}
						break;
					default:
						erroresChison.Add(new Error(TipoError.Advertencia,
							"Estructura principal incorrecta. Solamente se deben incluir los atributos 'DATABASES' y 'USERS'",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
						break;
				}
			}
			//VALIDACION DE EXISTENCIA
			bool todoBien = true;
			if (indexDb < 0)
			{
				erroresChison.Add(new Error(TipoError.Semantico,
					   "Estructura principal incorrecta. Se debe incluir el atributo 'DATABASES'",
					   raiz.Span.Location.Line,
					   raiz.Span.Location.Column,
					   Datos.GetDate(),
					   Datos.GetTime()));
				todoBien = false;
			}
			if (indexUs < 0)
			{
				erroresChison.Add(new Error(TipoError.Semantico,
						   "Estructura principal incorrecta. Se debe incluir el atributo 'USERS'",
						   raiz.Span.Location.Line,
						   raiz.Span.Location.Column,
						   Datos.GetDate(),
						   Datos.GetTime()));
				todoBien = false;
			}
			if (todoBien)
			{
				raiz = raiz.ChildNodes.ElementAt(0);
				//recorrer e insertar bases de datos
				if (raiz.ChildNodes.ElementAt(indexDb).ChildNodes.ElementAt(1).Term.Name == "LISTA")
				{
					RecorrerBasesDeDatos(raiz.ChildNodes.ElementAt(indexDb).ChildNodes.ElementAt(1));
				}
				else
				{
					erroresChison.Add(new Error(TipoError.Semantico,
						"El atributo 'DATABASES' debe contener una lista de bases de datos",
						raiz.Span.Location.Line,
						raiz.Span.Location.Column,
						Datos.GetDate(),
						Datos.GetTime()));
				}
				//recorrer e insertar usuarios
				if (raiz.ChildNodes.ElementAt(indexUs).ChildNodes.ElementAt(1).Term.Name == "LISTA")
				{
					RecorrerUsuarios(raiz.ChildNodes.ElementAt(indexUs).ChildNodes.ElementAt(1));
				}
				else
				{
					erroresChison.Add(new Error(TipoError.Semantico,
						"El atributo 'USERS' debe contener una lista de usuarios",
						raiz.Span.Location.Line,
						raiz.Span.Location.Column,
						Datos.GetDate(),
						Datos.GetTime()));
				}
			}
		}

		private static void RecorrerBasesDeDatos(ParseTreeNode raiz)
		{
			List<BaseDatos> bases = new List<BaseDatos>();
			//VALIDAR QUE SEAN OBJETOS
			foreach (ParseTreeNode nodo in raiz.ChildNodes)
			{
				if (nodo.Term.Name != "OBJETO")
				{
					erroresChison.Add(new Error(TipoError.Semantico,
						"La lista debe estar compuesta solamente por bases de datos",
						raiz.Span.Location.Line, raiz.Span.Location.Column,
						Datos.GetDate(),
						Datos.GetTime()));
				}
				else
				{
					BaseDatos resultado = GetBaseDatos(nodo);
					if (resultado != null) bases.Add(resultado);
				}

			}
			//GUARDAR LAS BASES DE DATOS
			foreach (BaseDatos nuevo in bases)
			{
				if (!Analizador.ExisteDB(nuevo.Nombre))
				{
					Analizador.AddBaseDatos(nuevo);
				}
				else
				{
					erroresChison.Add(new Error(TipoError.Semantico,
						"La base de datos '" + nuevo.Nombre + "' ya existe en el sistema",
						raiz.Span.Location.Line, raiz.Span.Location.Column,
						Datos.GetDate(),
						Datos.GetTime()));
				}
			}
		}

		private static BaseDatos GetBaseDatos(ParseTreeNode raiz)
		{
			BaseDatos based = new BaseDatos();
			List<object> objetos = null;
			List<TablaFalsa> tablas = new List<TablaFalsa>();
			List<ProcedimientoFalso> procedimientos = new List<ProcedimientoFalso>();
			List<UserType> usert = new List<UserType>();

			foreach (ParseTreeNode nodo in raiz.ChildNodes)
			{
				switch (nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower())
				{
					case "name":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "cadena")
						{
							if (based.Nombre == null)
							{
								based.Nombre = nodo.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' debe ser un dato tipo string",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					case "data":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "LISTA")
						{
							if (objetos == null)
							{
								objetos = GetObjetos(nodo.ChildNodes.ElementAt(1));
								foreach (object objetodb in objetos)
								{
									if (objetodb.GetType() == typeof(UserType))
									{
										UserType ut = (UserType)objetodb;
										usert.Add(ut);
									}
									else if (objetodb.GetType() == typeof(ProcedimientoFalso))
									{
										ProcedimientoFalso ut = (ProcedimientoFalso)objetodb;
										procedimientos.Add(ut);
									}
									else if (objetodb.GetType() == typeof(TablaFalsa))
									{
										TablaFalsa ut = (TablaFalsa)objetodb;
										tablas.Add(ut);
									}
								}
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'DATA' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'DATA' debe ser una lista",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					default:
						erroresChison.Add(new Error(TipoError.Advertencia,
							"Estructura de 'base de datos' es incorrecta. Solamente se deben incluir los atributos 'NAME' y 'DATA'",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
						break;
				}
			}

			if (based.Nombre != null && objetos != null)
			{
				foreach (TablaFalsa tabla in tablas)
				{
					ValidarTabla(tabla, based);
				}
				foreach (ProcedimientoFalso proc in procedimientos) {
					ValidarProcedimiento(proc,based);
				}
				return based;
			}
			erroresChison.Add(new Error(TipoError.Advertencia,
				"No se incluyó alguno de los atributos 'NAME' o 'DATA'",
				raiz.Span.Location.Line,
				raiz.Span.Location.Column,
				Datos.GetDate(),
				Datos.GetTime()));
			return null;
		}

		private static void ValidarProcedimiento(ProcedimientoFalso proc, BaseDatos based)
		{
			Procedimiento procedimiento = new Procedimiento(proc.Linea,proc.Columna);
			Boolean valido = true;
			int cont = 0;
			foreach (Parametro param in proc.Parametros)
			{
				if (!procedimiento.existeParametro(param.Nombre))
				{
					if (param.Tipo.Tipo == TipoDatoDB.OBJETO) {

						if (based.ExisteUserType(param.Tipo.Nombre) ){
							procedimiento.Parametros.Add(param);
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El user Type '" + proc.Nombre + "' no existe",
								proc.LineasNum.ElementAt(cont), proc.ColumnasNum.ElementAt(cont),
								Datos.GetDate(), Datos.GetTime()));
							valido = false;
						}

					} else if (Datos.IsLista(param.Tipo.Tipo)) {
						if (ValidarInstanciaLista(param.Tipo, based, proc.LineasNum.ElementAt(cont), proc.ColumnasNum.ElementAt(cont)))
						{
							procedimiento.Parametros.Add(param);
						}
					}
					else {
						procedimiento.Parametros.Add(param);

					}
				}
				else {
					erroresChison.Add(new Error(TipoError.Semantico,
								"El parámetro '" + proc.Nombre + "' ya existe",
								proc.LineasNum.ElementAt(cont), proc.ColumnasNum.ElementAt(cont),
								Datos.GetDate(), Datos.GetTime()));
					valido = false;
				}
				
				cont++;
			}

			foreach (Parametro param in proc.Retornos)
			{
				if (!procedimiento.existeRetorno(param.Nombre))
				{
					if (param.Tipo.Tipo == TipoDatoDB.OBJETO)
					{

						if (based.ExisteUserType(param.Tipo.Nombre))
						{
							procedimiento.Retornos.Add(param);
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El user Type '" + proc.Nombre + "' no existe",
								proc.LineasNum.ElementAt(cont), proc.ColumnasNum.ElementAt(cont),
								Datos.GetDate(), Datos.GetTime()));
							valido = false;
						}

					}
					else if (Datos.IsLista(param.Tipo.Tipo))
					{
						if (ValidarInstanciaLista(param.Tipo, based, proc.LineasNum.ElementAt(cont), proc.ColumnasNum.ElementAt(cont)))
						{
							procedimiento.Retornos.Add(param);
						}
					}
					else
					{
						procedimiento.Retornos.Add(param);

					}
				}
				else
				{
					erroresChison.Add(new Error(TipoError.Semantico,
								"El parámetro '" + proc.Nombre + "' ya existe",
								proc.LineasNum.ElementAt(cont), proc.ColumnasNum.ElementAt(cont),
								Datos.GetDate(), Datos.GetTime()));
					valido = false;
				}

				cont++;
			}

			if (valido) {
				if (!based.ExisteProcedimiento(proc.Nombre))
			{
				based.AgregarProcedimiento(procedimiento);
			}
			else
			{
				erroresChison.Add(new Error(TipoError.Semantico,
					"El procedimiento '" + proc.Nombre + "' ya existe",
					proc.LineasNum.ElementAt(cont),
					proc.ColumnasNum.ElementAt(cont),
					Datos.GetDate(), Datos.GetTime()));
			}
			}
		}

		private static void ValidarTabla(TablaFalsa tablaFalsa, BaseDatos based)
		{
			Tabla tabla = new Tabla(tablaFalsa.Nombre);
			//VALIDAR COLUMNAS 
			int contador = 0;
			foreach (Columna col in tablaFalsa.Columnas)
			{
				if (!tabla.ExisteColumna(col.Nombre))
				{
					if (Datos.IsPrimitivo(col.Tipo.Tipo))
					{
						tabla.AgregarColumna(col);
					}
					else
					{
						if (Datos.IsLista(col.Tipo.Tipo))
						{
							//VALIDAR LISTA
							if (ValidarInstanciaLista(col.Tipo, based, tablaFalsa.LineasNum.ElementAt(contador), tablaFalsa.ColumnasNum.ElementAt(contador)))
							{
								tabla.AgregarColumna(col);
							}
						}
						else
						{
							//VALIDAR OBJETO
							if (based.ExisteUserType(col.Tipo.Nombre))
							{
								tabla.AgregarColumna(col);
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
												"El objeto '" + col.Tipo.ToString() + "' no existe",
											tablaFalsa.LineasNum.ElementAt(contador), tablaFalsa.ColumnasNum.ElementAt(contador),
												Datos.GetDate(),
												Datos.GetTime()));
								return;
							}
						}
					}
				}
				else
				{
					erroresChison.Add(new Error(TipoError.Semantico,
						"La columna '" + col.Nombre + "' no existe",
						tablaFalsa.LineasNum.ElementAt(contador), tablaFalsa.ColumnasNum.ElementAt(contador),
						Datos.GetDate(),
						Datos.GetTime()));
					return;
				}
				contador++;
			}
			//VALIDAR DATOS 
			foreach (FilaDatos fila in tablaFalsa.Datos)
			{
				InsertarEnTabla(tabla, fila.Datos, based, fila.Linea, fila.Columna);
			}

			if (!based.ExisteTabla(tabla.Nombre))
			{
				based.AgregarTabla(tabla);
			}
			else {
				erroresChison.Add(new Error(TipoError.Semantico,
					"La tabla '"+tabla.Nombre+"' ya existe",
					tablaFalsa.LineasNum.ElementAt(contador), tablaFalsa.ColumnasNum.ElementAt(contador),
					Datos.GetDate(),
					Datos.GetTime()));
			}
		}

		private static List<object> GetObjetos(ParseTreeNode raiz)
		{
			List<object> lista = new List<object>();
			//raiz -> LISTA
			foreach (ParseTreeNode nodo in raiz.ChildNodes)
			{
				if (nodo.Term.Name == "OBJETO")
				{
					TipoObjeto t = GetTipoObjetoCql(nodo);
					switch (t)
					{
						case TipoObjeto.Objeto:
							UserType ustype = GetUserType(nodo, lista);
							if (ustype != null) lista.Add(ustype);
							break;
						case TipoObjeto.Procedimiento:
							ProcedimientoFalso proc = GetProcedimiento(nodo, lista);
							if (proc != null) lista.Add(proc);
							break;
						case TipoObjeto.Tabla:
							TablaFalsa tab = GetTabla(nodo, lista);
							if (tab != null) lista.Add(tab);
							break;
					}
				}
				else
				{
					erroresChison.Add(new Error(TipoError.Advertencia,
							"La lista de 'DATA' solo debe contener objetos CQL",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
				}
			}

			return lista;
		}

		private static TablaFalsa GetTabla(ParseTreeNode raiz, List<object> db)
		{
			TablaFalsa tabla = new TablaFalsa();
			string t = null;
			foreach (ParseTreeNode nodo in raiz.ChildNodes)
			{
				switch (nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower())
				{
					case "name":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "cadena")
						{
							if (tabla.Nombre == null)
							{
								tabla.Nombre = nodo.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' debe ser un dato tipo string",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					case "cql-type":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "cadena")
						{
							if (t == null)
							{
								t = nodo.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'CQL-TYPE' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'CQL-TYPE' debe ser un dato tipo string",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					case "columns":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "LISTA")
						{
							if (tabla.Columnas == null)
							{
								tabla.Columnas = GetColumnasTabla(tabla, nodo.ChildNodes.ElementAt(1));
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'COLUMNS' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'COLUMNS' debe ser una lista",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					case "data":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "LISTA")
						{
							if (tabla.Datos == null)
							{
								//recuperar e insertar
								List<FilaDatos> datos = GetDatosTabla(nodo.ChildNodes.ElementAt(1), db);
								if (datos != null)
								{
									tabla.Datos = datos;
								}
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'DATA' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'DATA' debe ser una lista",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					default:
						erroresChison.Add(new Error(TipoError.Advertencia,
							"Estructura de 'tabla' es incorrecta. Solamente se deben incluir los atributos 'NAME', 'CQL-TYPE', 'COLUMNS' y 'DATA'",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
						break;
				}

			}

			if (tabla.Nombre != null && t != null && tabla.Columnas != null) return tabla;
			erroresChison.Add(new Error(TipoError.Advertencia,
				"No se incluyó alguno de los atributos 'NAME', 'CQL-TYPE', 'COLUMNS' o 'DATA'",
				raiz.Span.Location.Line,
				raiz.Span.Location.Column,
				Datos.GetDate(),
				Datos.GetTime()));
			return null;
		}

		private static bool ValidarInstanciaLista(TipoObjetoDB tipoInstancia, BaseDatos db, int linea, int columna)
		{
			switch (tipoInstancia.Tipo)
			{
				case TipoDatoDB.LISTA_OBJETO:
				case TipoDatoDB.SET_OBJETO:
					TipoObjetoDB tipoAdentro = Datos.GetTipoObjetoDBPorCadena(tipoInstancia.Nombre);
					if (Datos.IsLista(tipoAdentro.Tipo))
					{
						if (!ValidarInstanciaLista(tipoAdentro, db, linea, columna))
						{
							return false;
						}
						return true;
					}
					else if (Datos.IsPrimitivo(tipoAdentro.Tipo))
					{
						return true;
					}
					else
					{
						//comprobar que exista el objeto
						if (!db.ExisteUserType(tipoAdentro.Nombre))
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El objeto '" + tipoAdentro.Nombre + "' no existe",
								  linea, columna,
								  Datos.GetDate(), Datos.GetTime()));
							return false;
						}
						return true;
					}
				case TipoDatoDB.LISTA_PRIMITIVO:
				case TipoDatoDB.SET_PRIMITIVO:
				case TipoDatoDB.MAP_PRIMITIVO:
					return true;
				case TipoDatoDB.MAP_OBJETO:
					string[] tipos = tipoInstancia.Nombre.Split(',');
					tipoAdentro = Datos.GetTipoObjetoDBPorCadena(tipos[1]);
					if (Datos.IsLista(tipoAdentro.Tipo))
					{
						if (!ValidarInstanciaLista(tipoAdentro, db, linea, columna))
						{
							return false;
						}
						return true;
					}
					else if (Datos.IsPrimitivo(tipoAdentro.Tipo))
					{
						return true;
					}
					else
					{
						//comprobar que exista el objeto
						if (!db.ExisteUserType(tipoAdentro.Nombre))
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El objeto '" + tipoAdentro.Nombre + "' no existe",
								  linea, columna,
								  Datos.GetDate(), Datos.GetTime()));
							return false;
						}
						return true;
					}
			}
			return false;
		}

		private static void InsertarEnTabla(Tabla tab, List<ParDatos> valores, BaseDatos db, int linea, int columna)
		{
			if (valores.Count == tab.Columnas.Count)
			{
				//VALIDANDO
				int indiceDatos = 0;
				int indiceColumnas = 0;
				Queue<object> valoresAInsertar = new Queue<object>();
				foreach (Columna cl in tab.Columnas)
				{
					ParDatos parDatos = valores.ElementAt(indiceDatos);

					if (IsTipoCompatible(cl.Tipo, parDatos.Valor))
					{
						object respuesta = CasteoImplicito(cl.Tipo, parDatos.Valor, db, linea, columna);
						if (respuesta != null)
						{
							if (respuesta.GetType() == typeof(ThrowError))
							{
								erroresChison.Add(new Error((ThrowError)respuesta, true));
							}
							else
							{
								if (cl.IsPrimary)
								{
									if (cl.Tipo.Tipo == TipoDatoDB.STRING)
									{
										if (respuesta.Equals("$%_null_%$"))
										{
											erroresChison.Add( new Error(TipoError.Semantico,
											"No se pueden insertar nulos en la columna '" + cl.Nombre + "'",
												linea, columna));
											return;
										}
									}
									else if (cl.Tipo.Tipo == TipoDatoDB.DATE || cl.Tipo.Tipo == TipoDatoDB.TIME)
									{
										MyDateTime date = (MyDateTime)respuesta;
										if (date.IsNull)
										{
											erroresChison.Add(new Error(TipoError.Semantico,
											"No se pueden insertar nulos en la columna '" + cl.Nombre + "'",
												linea, columna));
											return;
										}
									}
									else if (cl.Tipo.Tipo == TipoDatoDB.MAP_OBJETO || cl.Tipo.Tipo == TipoDatoDB.MAP_PRIMITIVO)
									{
										CollectionMapCql date = (CollectionMapCql)respuesta;
										if (date.IsNull)
										{
											erroresChison.Add(new Error(TipoError.Semantico,
											"No se pueden insertar nulos en la columna '" + cl.Nombre + "'",
												linea, columna));
											return;
										}
									}
									else if (cl.Tipo.Tipo == TipoDatoDB.LISTA_OBJETO || cl.Tipo.Tipo == TipoDatoDB.LISTA_PRIMITIVO)
									{
										CollectionListCql date = (CollectionListCql)respuesta;
										if (date.IsNull)
										{
											erroresChison.Add(new Error(TipoError.Semantico,
											"No se pueden insertar nulos en la columna '" + cl.Nombre + "'",
												linea, columna));
											return;
										}
									}
									else if (cl.Tipo.Tipo == TipoDatoDB.SET_OBJETO || cl.Tipo.Tipo == TipoDatoDB.SET_PRIMITIVO)
									{
										CollectionListCql date = (CollectionListCql)respuesta;
										if (date.IsNull)
										{
											erroresChison.Add(new Error(TipoError.Semantico,
											"No se pueden insertar nulos en la columna '" + cl.Nombre + "'",
												linea, columna));
											return;
										}
									}
								}
								valoresAInsertar.Enqueue(respuesta);
							}
						}
					}
					else
					{
						if (parDatos.Valor.Equals("null"))
						{
							valoresAInsertar.Enqueue("null");
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El valor No." + (indiceDatos + 1) + " no concuerda con el tipo de dato '" + cl.Nombre + "'(" + cl.Tipo.ToString() + ")",
								  linea, columna,
								  Datos.GetDate(), Datos.GetTime()));
							return;
						}

						
						//}
					}
					indiceColumnas++;
					indiceDatos++;
				}
				//INSERTANDO
				if (tab.Columnas.Count == valoresAInsertar.Count)
				{
					object correcto = tab.ValidarPk(valoresAInsertar, linea, columna);
					if (correcto.GetType() == typeof(ThrowError))
					{
						erroresChison.Add(new Error((ThrowError)correcto, true));
					}
					else
					{
						//LLENANDO TABLA
						tab.AgregarValores(valoresAInsertar);
					}

				}
			}
			else
			{
				erroresChison.Add(new Error(TipoError.Semantico,
						"La cantidad de valores no concuerda con la cantidad de columnas en las que se puede insertar",
						linea, columna,
						Datos.GetDate(), Datos.GetTime()));
			}
		}

		private static List<FilaDatos> GetDatosTabla(ParseTreeNode parseTreeNode, List<object> db)
		{
			List<FilaDatos> datos = new List<FilaDatos>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
			{
				if (nodo.Term.Name == "OBJETO")
				{
					FilaDatos fil = GetFilaDatos(nodo, db);
					if (fil != null)
					{
						datos.Add(fil);
					}
				}
				else
				{
					erroresChison.Add(new Error(TipoError.Semantico,
					  "La data de una tabla solo debe contener columnas",
						parseTreeNode.Span.Location.Line, parseTreeNode.Span.Location.Column,
						Datos.GetDate(), Datos.GetTime()));
				}
			}
			return datos;
		}

		private static FilaDatos GetFilaDatos(ParseTreeNode nodo, List<object> db)
		{
			FilaDatos fila = new FilaDatos();
			foreach (ParseTreeNode nodis in nodo.ChildNodes)
			{
				Object val = GetObjetoDB(nodis.ChildNodes.ElementAt(1), db);
				if (val != null)
				{
					fila.Datos.Add(new ParDatos(nodis.ChildNodes.ElementAt(0).Token.ValueString.ToLower(),
					val));
					fila.Linea = nodis.ChildNodes.ElementAt(0).Span.Location.Line;
					fila.Columna = nodis.ChildNodes.ElementAt(0).Span.Location.Column;
				}
			}
			return fila;
		}

		private static object GetObjetoDB(ParseTreeNode parseTreeNode, List<object> db)
		{
			int linea = parseTreeNode.Span.Location.Line;
			int column = parseTreeNode.Span.Location.Column;
			switch (parseTreeNode.Term.Name)
			{
				case "numero":
					return Datos.GetValor(parseTreeNode.Token.ValueString.ToString());
				case "cadena":
					return parseTreeNode.Token.ValueString.ToString();
				case "true":
					return Datos.GetValor(parseTreeNode.Token.ValueString.ToString());
				case "false":
					return Datos.GetValor(parseTreeNode.Token.ValueString.ToString());
				case "NULL":
					return Datos.GetValor(parseTreeNode.Token.ValueString.ToString());
				case "null":
					return "null";
				case "date":
					MyDateTime di;
					if (DateTime.TryParse(parseTreeNode.Token.ValueString.ToLower().Replace("'", string.Empty), out DateTime dt) &&
					Regex.IsMatch(parseTreeNode.Token.ValueString.ToLower().ToString(), "'[0-9]{4}-[0-9]{2}-[0-9]{2}'"))
					{
						di = new MyDateTime(TipoDatoDB.DATE, dt);
						return di;
					}
					else
					{
						di = new MyDateTime(TipoDatoDB.DATE, DateTime.Parse("0000-00-00"));
						erroresChison.Add(new Error(TipoError.Advertencia,
									"La fecha es incorrecta, el formato debe ser AAAA-MM-DD",
								   linea, column,
								   Datos.GetDate(), Datos.GetTime()));
					}
					break;
				case "time":
					if (DateTime.TryParse(parseTreeNode.Token.ValueString.ToLower().Replace("'", string.Empty), out DateTime dt1) &&
								Regex.IsMatch(parseTreeNode.Token.ValueString.ToLower().ToString(), "'[0-9]{2}:[0-9]{2}:[0-9]{2}'"))
					{
						di = new MyDateTime(TipoDatoDB.TIME, dt1);
						return di;
					}
					else
					{

						try
						{
							di = new MyDateTime(TipoDatoDB.DATE, DateTime.Parse("0000-00-00"));
							erroresChison.Add(new Error(TipoError.Advertencia,
									"La hora es incorrecta, el formato debe ser HH:MM:SS",
								   linea, column,
								   Datos.GetDate(), Datos.GetTime()));
							return di;
						}
						catch (Exception)
						{
							erroresChison.Add(new Error(TipoError.Advertencia,
								"La fecha está fuera de rango",
								 parseTreeNode.ChildNodes.ElementAt(0).Token.Location.Line, parseTreeNode.ChildNodes.ElementAt(0).Token.Location.Column,
								 Datos.GetDate(), Datos.GetTime()));

						}
					}
					break;
				case "LISTA":
					List<object> lista = new List<object>();
					foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
					{
						object respuesta = GetObjetoDB(nodo, db);
						if (respuesta != null)
						{
							lista.Add(respuesta);
						}
					}
					return lista;
				case "OBJETO":
				case "MAP":
					FilaDatos atributos = GetFilaDatos(parseTreeNode, db);
					return atributos;

			}
			return null;
		}

		private static object GetMap(ParseTreeNode parseTreeNode)
		{
			CollectionMapCql map = new CollectionMapCql(null, null);
			if (parseTreeNode.ChildNodes.Count > 0)
			{
				object llave = GetDatoP(parseTreeNode.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).ChildNodes.ElementAt(0));
				object valor = Datos.GetValor(parseTreeNode.ChildNodes.ElementAt(0).ChildNodes.ElementAt(1).Token.ValueString);
				map.TipoLlave = Datos.GetTipoObjetoDB(llave);
				map.TipoValor = Datos.GetTipoObjetoDB(valor);
				foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
				{
					llave = GetDatoP(nodo.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0));
					valor = Datos.GetValor(nodo.ChildNodes.ElementAt(1).Token.ValueString);
					object posibleError = map.AddItem(llave, valor, nodo.Span.Location.Line, nodo.Span.Location.Column);
					if (posibleError != null)
					{
						if (posibleError.GetType() == typeof(ThrowError))
						{
							erroresChison.Add(new Error((ThrowError)posibleError, true));
						}
					}
				}
			}

			return map;
		}

		private static object GetDatoP(ParseTreeNode parseTreeNode)
		{
			int linea = parseTreeNode.Span.Location.Line;
			int column = parseTreeNode.Span.Location.Column;
			switch (parseTreeNode.Term.Name)
			{
				case "numero":
					return Datos.GetValor(parseTreeNode.Token.ValueString.ToString());
				case "cadena":
					return Datos.GetValor(parseTreeNode.Token.ValueString.ToString());
				case "true":
					return Datos.GetValor(parseTreeNode.Token.ValueString.ToString());
				case "false":
					return Datos.GetValor(parseTreeNode.Token.ValueString.ToString());
				case "date":
					MyDateTime di;
					if (DateTime.TryParse(parseTreeNode.Token.ValueString.ToLower().Replace("'", string.Empty), out DateTime dt) &&
					Regex.IsMatch(parseTreeNode.Token.ValueString.ToLower().ToString(), "'[0-9]{4}-[0-9]{2}-[0-9]{2}'"))
					{
						di = new MyDateTime(TipoDatoDB.DATE, dt);
					}
					else
					{
						di = new MyDateTime(TipoDatoDB.DATE, DateTime.Parse("0000-00-00"));
						erroresChison.Add(new Error(TipoError.Advertencia,
									"La fecha es incorrecta, el formato debe ser AAAA-MM-DD",
								   linea, column,
								   Datos.GetDate(), Datos.GetTime()));
					}
					return di;
				case "time":
					if (DateTime.TryParse(parseTreeNode.Token.ValueString.ToLower().Replace("'", string.Empty), out DateTime dt1) &&
								Regex.IsMatch(parseTreeNode.Token.ValueString.ToLower().ToString(), "'[0-9]{2}:[0-9]{2}:[0-9]{2}'"))
					{
						di = new MyDateTime(TipoDatoDB.TIME, dt1);
					}
					else
					{
						di = new MyDateTime(TipoDatoDB.TIME, DateTime.Parse("00:00:00"));
						erroresChison.Add(new Error(TipoError.Advertencia,
									"La hora es incorrecta, el formato debe ser HH:MM:SS",
								   linea, column,
								   Datos.GetDate(), Datos.GetTime()));
					}
					return di;
			}
			return null;
		}

		private static List<UserType> GetUserTypesLista(List<object> tab)
		{
			List<UserType> usertpypes = new List<UserType>();
			foreach (object ut in tab)
			{
				if (ut.GetType() == typeof(UserType))
				{
					usertpypes.Add((UserType)ut);
				}
			}
			return usertpypes;
		}

		private static List<Columna> GetColumnasTabla(TablaFalsa tabla, ParseTreeNode parseTreeNode)
		{
			List<Columna> columnas = new List<Columna>();
			foreach (ParseTreeNode col in parseTreeNode.ChildNodes)
			{
				if (col.Term.Name == "OBJETO")
				{
					Columna cl = new Columna();
					String tipo = null;
					string isPk = null;
					foreach (ParseTreeNode raiz in col.ChildNodes)
					{
						switch (raiz.ChildNodes.ElementAt(0).Token.ValueString.ToLower())
						{
							case "name":
								if (raiz.ChildNodes.ElementAt(1).Term.Name == "cadena")
								{
									if (cl.Nombre == null)
									{
										cl.Nombre = raiz.ChildNodes.ElementAt(1).Token.ValueString.ToLower();

									}
									else
									{
										erroresChison.Add(new Error(TipoError.Semantico,
										"El atributo 'NAME' solo debe aparecer una vez",
										raiz.Span.Location.Line, raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));
									}
								}
								else
								{
									erroresChison.Add(new Error(TipoError.Semantico,
										"El atributo 'NAME' debe ser un dato tipo string",
										raiz.Span.Location.Line, raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));

								}
								break;
							case "type":
								if (raiz.ChildNodes.ElementAt(1).Term.Name == "cadena")
								{
									if (tipo == null)
									{
										tipo = raiz.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
									}
									else
									{
										erroresChison.Add(new Error(TipoError.Semantico,
										"El atributo 'TYPE' solo debe aparecer una vez",
										raiz.Span.Location.Line, raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));
									}
								}
								else
								{
									erroresChison.Add(new Error(TipoError.Semantico,
										"El atributo 'TYPE' debe ser un dato tipo string",
										raiz.Span.Location.Line, raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));

								}
								break;
							case "pk":
								if (raiz.ChildNodes.ElementAt(1).Term.Name == "true" || raiz.ChildNodes.ElementAt(1).Term.Name == "false")
								{
									if (isPk == null)
									{
										isPk = raiz.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
									}
									else
									{
										erroresChison.Add(new Error(TipoError.Semantico,
										"El atributo 'AS' solo debe aparecer una vez",
										raiz.Span.Location.Line, raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));
									}
								}
								break;
							default:
								erroresChison.Add(new Error(TipoError.Advertencia,
									"Estructura de 'Columna' es incorrecta. Solamente se deben incluir los atributos 'NAME', 'PK' y 'TYPE'",
									raiz.Span.Location.Line,
									raiz.Span.Location.Column,
									Datos.GetDate(),
									Datos.GetTime()));
								break;
						}
					}
					if (cl.Nombre != null && tipo != null && isPk != null)
					{
						cl.Tipo = Datos.GetTipoObjetoDBPorCadena(tipo);
						cl.IsPrimary = isPk.ToLower().Equals("true");
						columnas.Add(cl);
						tabla.LineasNum.Add(col.Span.Location.Line);
						tabla.ColumnasNum.Add(col.Span.Location.Column);
					}
					else
					{
						erroresChison.Add(new Error(TipoError.Semantico,
								"No se incluyó alguno de los atributos 'NAME','PK' o 'TYPE'",
								col.Span.Location.Line, col.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
					}
				}
				else
				{
					erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'COLUMNS' solo puede contener columnas",
								col.Span.Location.Line, col.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
				}
			}
			return columnas;
		}

		private static ProcedimientoFalso GetProcedimiento(ParseTreeNode raiz, List<object> db)
		{
			ProcedimientoFalso proc = new ProcedimientoFalso(raiz.Span.Location.Line, raiz.Span.Location.Column);
			string t = null;
			string bren = null;
			foreach (ParseTreeNode nodo in raiz.ChildNodes)
			{
				switch (nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower())
				{
					case "name":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "cadena")
						{
							if (proc.Nombre == null)
							{
								proc.Nombre = nodo.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' debe ser un dato tipo string",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					case "cql-type":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "cadena")
						{
							if (t == null)
							{
								t = nodo.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'CQL-TYPE' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'CQL-TYPE' debe ser un dato tipo string",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					case "parameters":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "LISTA")
						{
							if (bren == null)
							{
								//PARAMETROS
								bren = ":D";
								List<Parametro> resultado = GetListaParametros(proc,nodo.ChildNodes.ElementAt(1));
								if (resultado != null)
								{
									proc.Parametros = resultado;
								}

								//RETORNOS 
								resultado = GetListaRetornos(proc,nodo.ChildNodes.ElementAt(1));
								if (resultado != null)
								{
									proc.Retornos = resultado;
								}
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'PARAMETERS' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'PARAMETERS' debe ser una lista",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
						}
						break;
					case "instr":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "instrucciones")
						{
							if (proc.Instrucciones == null && proc.Sentencias == null)
							{
								//EVALUAR Y ASIGNAR
								String codigo = nodo.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
								codigo = codigo.TrimStart('$');
								codigo = codigo.TrimEnd('$');
								proc.Sentencias = Analizador.GetSentenciasCQL(codigo);
								List<Error> erroresInst = (Analizador.ErroresCQL);
								//cambiar el numero de linea 
								if (erroresInst.Count > 0)
								{
									//agregar a errores chison
									codigo = "//SE ENCONTRARON ERRORES EN EL CODIGO\n";
								}
								proc.Instrucciones = codigo;
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'INSTR' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'INSTR' debe ser una cadena encerrada entre $",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					default:
						erroresChison.Add(new Error(TipoError.Advertencia,
							"Estructura de 'CQL-Type Procedure' es incorrecta. Solamente se deben incluir los atributos 'INSTR','NAME','PARAMETERS' y 'CQL-TYPE'",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
						break;
				}
			}
			if (proc.Nombre!=null &&proc.Parametros!=null &&proc.Instrucciones!=null && proc.Retornos!=null && t != null && bren != null) return proc;
			erroresChison.Add(new Error(TipoError.Advertencia,
				"No se incluyó alguno de los atributos 'NAME','PARAMETERS', 'INSTR' o 'CQL-TYPE'",
				raiz.Span.Location.Line,
				raiz.Span.Location.Column,
				Datos.GetDate(),
				Datos.GetTime()));
			return null;
		}

		private static List<Parametro> GetListaRetornos(ProcedimientoFalso proc, ParseTreeNode rai)
		{
			List<Parametro> dic = new List<Parametro>();
			foreach (ParseTreeNode nodo in rai.ChildNodes)
			{
				if (nodo.Term.Name == "OBJETO")
				{
					String nombre = null;
					String tipo = null;
					string pras = null;
					if (!IsParametro(nodo))
					{
						foreach (ParseTreeNode raiz in nodo.ChildNodes)
						{
							switch (raiz.ChildNodes.ElementAt(0).Token.ValueString.ToLower())
							{
								case "name":
									if (raiz.ChildNodes.ElementAt(1).Term.Name == "cadena")
									{
										if (nombre == null)
										{
											nombre = raiz.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
										}
										else
										{
											erroresChison.Add(new Error(TipoError.Semantico,
											"El atributo 'NAME' solo debe aparecer una vez",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));
										}
									}
									else
									{
										erroresChison.Add(new Error(TipoError.Semantico,
											"El atributo 'NAME' debe ser un dato tipo string",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));

									}
									break;
								case "type":
									if (raiz.ChildNodes.ElementAt(1).Term.Name == "cadena")
									{
										if (tipo == null)
										{
											tipo = raiz.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
										}
										else
										{
											erroresChison.Add(new Error(TipoError.Semantico,
											"El atributo 'TYPE' solo debe aparecer una vez",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));
										}
									}
									else
									{
										erroresChison.Add(new Error(TipoError.Semantico,
											"El atributo 'TYPE' debe ser un dato tipo string",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));

									}
									break;
								case "as":
									if (raiz.ChildNodes.ElementAt(1).Term.Name == "out")
									{
										if (pras == null)
										{
											pras = raiz.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
										}
										else
										{
											erroresChison.Add(new Error(TipoError.Semantico,
											"El atributo 'AS' solo debe aparecer una vez",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));
										}
									}
									break;
								default:
									erroresChison.Add(new Error(TipoError.Advertencia,
										"Estructura de 'Atributo de Objeto' es incorrecta. Solamente se deben incluir los atributos 'NAME' y 'TYPE'",
										raiz.Span.Location.Line,
										raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));
									break;
							}
						}
						if (nombre != null && tipo != null && pras != null)
						{
							dic.Add(new Parametro(nombre, Datos.GetTipoObjetoDBPorCadena(tipo)));
							proc.LineasNum.Add(nodo.Span.Location.Line+1);
							proc.ColumnasNum.Add(nodo.Span.Location.Column);
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Advertencia,
								"No se incluyó alguno de los atributos 'AS','NAME' o 'TYPE'",
								nodo.Span.Location.Line,
								nodo.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
						}
					}
				}
				else
				{
					erroresChison.Add(new Error(TipoError.Advertencia,
							"La lista de 'PARAMETERS' solo debe contener parámetros",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
				}
			}
			return dic;
		}

		private static List<Parametro> GetListaParametros(ProcedimientoFalso proc,ParseTreeNode rai)
		{
			List<Parametro> dic = new List<Parametro>();
			foreach (ParseTreeNode nodo in rai.ChildNodes)
			{
				if (nodo.Term.Name == "OBJETO")
				{
					String nombre = null;
					String tipo = null;
					string pras = null;
					if (IsParametro(nodo))
					{
						foreach (ParseTreeNode raiz in nodo.ChildNodes)
						{
							switch (raiz.ChildNodes.ElementAt(0).Token.ValueString.ToLower())
							{
								case "name":
									if (raiz.ChildNodes.ElementAt(1).Term.Name == "cadena")
									{
										if (nombre == null)
										{
											nombre = raiz.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
										}
										else
										{
											erroresChison.Add(new Error(TipoError.Semantico,
											"El atributo 'NAME' solo debe aparecer una vez",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));
										}
									}
									else
									{
										erroresChison.Add(new Error(TipoError.Semantico,
											"El atributo 'NAME' debe ser un dato tipo string",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));

									}
									break;
								case "type":
									if (raiz.ChildNodes.ElementAt(1).Term.Name == "cadena")
									{
										if (tipo == null)
										{
											tipo = raiz.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
										}
										else
										{
											erroresChison.Add(new Error(TipoError.Semantico,
											"El atributo 'TYPE' solo debe aparecer una vez",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));
										}
									}
									else
									{
										erroresChison.Add(new Error(TipoError.Semantico,
											"El atributo 'TYPE' debe ser un dato tipo string",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));

									}
									break;
								case "as":
									if (raiz.ChildNodes.ElementAt(1).Term.Name == "in")
									{
										if (pras == null)
										{
											pras = raiz.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
										}
										else
										{
											erroresChison.Add(new Error(TipoError.Semantico,
											"El atributo 'AS' solo debe aparecer una vez",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));
										}
									}
									break;
								default:
									erroresChison.Add(new Error(TipoError.Advertencia,
										"Estructura de 'Atributo de Objeto' es incorrecta. Solamente se deben incluir los atributos 'NAME' y 'TYPE'",
										raiz.Span.Location.Line,
										raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));
									break;
							}
						}
						if (nombre != null && tipo != null && pras != null)
						{
							dic.Add(new Parametro(nombre, Datos.GetTipoObjetoDBPorCadena(tipo)));
							proc.LineasNum.Add(nodo.Span.Location.Line+1);
							proc.ColumnasNum.Add(nodo.Span.Location.Column);

						}
						else
						{
							erroresChison.Add(new Error(TipoError.Advertencia,
								"No se incluyó alguno de los atributos 'AS','NAME' o 'TYPE'",
								nodo.Span.Location.Line,
								nodo.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
						}
					}
				}
				else
				{
					erroresChison.Add(new Error(TipoError.Advertencia,
							"La lista de 'PARAMETERS' solo debe contener parámetros",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
				}
			}
			return dic;
		}

		private static bool IsParametro(ParseTreeNode nodo)
		{
			foreach (ParseTreeNode no in nodo.ChildNodes)
			{
				if (no.ChildNodes.ElementAt(0).Token.ValueString.ToLower() == "as")
				{
					if (no.ChildNodes.ElementAt(1).Term.Name == "in" || no.ChildNodes.ElementAt(1).Term.Name == "out")
					{
						return no.ChildNodes.ElementAt(1).Term.Name == "in";
					}
					else
					{
						erroresChison.Add(new Error(TipoError.Semantico,
							"El atributo 'AS' debe ser una cadena OUT o IN",
							no.Span.Location.Line,
							no.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
					}
				}
			}
			return false;
		}

		private static UserType GetUserType(ParseTreeNode raiz, List<object> db)
		{
			UserType user = new UserType();
			string t = null;
			foreach (ParseTreeNode nodo in raiz.ChildNodes)
			{
				switch (nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower())
				{
					case "name":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "cadena")
						{
							if (user.Nombre == null)
							{
								user.Nombre = nodo.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' debe ser un dato tipo string",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					case "cql-type":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "cadena")
						{
							if (t == null)
							{
								t = nodo.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'CQL-TYPE' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'CQL-TYPE' debe ser un dato tipo string",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					case "attrs":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "LISTA")
						{
							if (user.Atributos == null)
							{
								Dictionary<string, TipoObjetoDB> resultado = GetListaAtributos(nodo.ChildNodes.ElementAt(1));
								if (resultado != null)
									user.Atributos = resultado;
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'ATTRS' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'ATTRS' debe ser una lista",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					default:
						erroresChison.Add(new Error(TipoError.Advertencia,
							"Estructura de 'CQL-Type object' es incorrecta. Solamente se deben incluir los atributos 'NAME','ATTRS' y 'CQL-TYPE'",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
						break;
				}
			}

			if (user.IsValido() && t != null && user.Atributos != null)
			{
				return user;
			}
			erroresChison.Add(new Error(TipoError.Advertencia,
				"No se incluyó alguno de los atributos 'NAME','ATTRS' o 'CQL-TYPE'",
				raiz.Span.Location.Line,
				raiz.Span.Location.Column,
				Datos.GetDate(),
				Datos.GetTime()));
			return null;
		}

		private static Dictionary<string, TipoObjetoDB> GetListaAtributos(ParseTreeNode parseTreeNode)
		{
			Dictionary<string, TipoObjetoDB> dic = new Dictionary<string, TipoObjetoDB>();

			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
			{
				if (nodo.Term.Name == "OBJETO")
				{
					string nombre = null;
					string tipo = null;
					//recorriendo los atributos del 'ATRIB'
					foreach (ParseTreeNode raiz in nodo.ChildNodes)
					{
						switch (raiz.ChildNodes.ElementAt(0).Token.ValueString.ToLower())
						{
							case "name":
								if (raiz.ChildNodes.ElementAt(1).Term.Name == "cadena")
								{
									if (nombre == null)
									{
										nombre = raiz.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
									}
									else
									{
										erroresChison.Add(new Error(TipoError.Semantico,
										"El atributo 'NAME' solo debe aparecer una vez",
										raiz.Span.Location.Line, raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));
									}
								}
								else
								{
									erroresChison.Add(new Error(TipoError.Semantico,
										"El atributo 'NAME' debe ser un dato tipo string",
										raiz.Span.Location.Line, raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));

								}
								break;
							case "type":
								if (raiz.ChildNodes.ElementAt(1).Term.Name == "cadena")
								{
									if (tipo == null)
									{
										tipo = raiz.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
									}
									else
									{
										erroresChison.Add(new Error(TipoError.Semantico,
										"El atributo 'TYPE' solo debe aparecer una vez",
										raiz.Span.Location.Line, raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));
									}
								}
								else
								{
									erroresChison.Add(new Error(TipoError.Semantico,
										"El atributo 'TYPE' debe ser un dato tipo string",
										raiz.Span.Location.Line, raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));

								}
								break;
							default:
								erroresChison.Add(new Error(TipoError.Advertencia,
									"Estructura de 'Atributo de Objeto' es incorrecta. Solamente se deben incluir los atributos 'NAME' y 'TYPE'",
									raiz.Span.Location.Line,
									raiz.Span.Location.Column,
									Datos.GetDate(),
									Datos.GetTime()));
								break;
						}
					}
					if (nombre != null && tipo != null)
					{
						try
						{
							dic.Add(nombre, Datos.GetTipoObjetoDBPorCadena(tipo));
						}
						catch (ArgumentException)
						{
							erroresChison.Add(new Error(TipoError.Advertencia,
							"El atributo '" + nombre + "' ya existe",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
						}
					}
					else
					{
						erroresChison.Add(new Error(TipoError.Advertencia,
							"No se incluyó alguno de los atributos 'NAME' o 'TYPE'",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
					}
				}
				else
				{
					erroresChison.Add(new Error(TipoError.Advertencia,
							"La lista de 'ATTRS' solo debe contener atributos",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
				}
			}

			return dic;
		}

		private static TipoObjeto GetTipoObjetoCql(ParseTreeNode nodo)
		{
			foreach (ParseTreeNode no in nodo.ChildNodes)
			{
				if (no.ChildNodes.ElementAt(0).Token.ValueString.ToLower() == "cql-type")
				{
					if (no.ChildNodes.ElementAt(1).Term.Name == "cadena")
					{
						switch (no.ChildNodes.ElementAt(1).Token.ValueString.ToLower())
						{
							case "object":
								return TipoObjeto.Objeto;
							case "table":
								return TipoObjeto.Tabla;
							case "procedure":
								return TipoObjeto.Procedimiento;
							default:
								return TipoObjeto.Error;
						}
					}
					else
					{
						erroresChison.Add(new Error(TipoError.Semantico,
							"El atributo 'CQL-TYPE' debe ser una cadena",
							no.Span.Location.Line,
							no.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
					}
				}
			}
			return TipoObjeto.Error;
		}

		private static void RecorrerUsuarios(ParseTreeNode raiz)
		{
			List<Usuario> usuarios = new List<Usuario>();
			//VALIDAR QUE SEAN OBJETOS
			foreach (ParseTreeNode nodo in raiz.ChildNodes)
			{
				if (nodo.Term.Name != "OBJETO")
				{
					erroresChison.Add(new Error(TipoError.Semantico,
						"La lista debe estar compuesta solamente por usuarios",
						raiz.Span.Location.Line, raiz.Span.Location.Column,
						Datos.GetDate(),
						Datos.GetTime()));
				}
				else
				{
					object resultado = GetUsuario(nodo);
					if (resultado != null) usuarios.Add((Usuario)resultado);
				}

			}
			//GUARDAR LOS USUARIOS
			foreach (Usuario nuevo in usuarios)
			{
				if (!Analizador.ExisteUsuario(nuevo.Nombre))
				{
					Analizador.AddUsuario(nuevo);
				}
				else
				{
					erroresChison.Add(new Error(TipoError.Semantico,
								"El usuario '" + nuevo.Nombre + "' ya existe en el sistema",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

				}
			}
		}

		private static Usuario GetUsuario(ParseTreeNode raiz)
		{
			Usuario usuario = new Usuario();
			foreach (ParseTreeNode nodo in raiz.ChildNodes)
			{
				switch (nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower())
				{
					case "name":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "cadena")
						{
							if (usuario.Nombre == null)
							{
								usuario.Nombre = nodo.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' debe ser un dato tipo string",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					case "password":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "cadena")
						{
							if (usuario.Password == null)
							{
								usuario.Password = nodo.ChildNodes.ElementAt(1).Token.ValueString.ToLower();
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'PASSWORD' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'PASSWORD' debe ser un dato tipo string",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					case "permissions":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "LISTA")
						{
							if (usuario.Permisos == null)
							{
								List<string> resultado = GetListaPermisosUsuario(nodo.ChildNodes.ElementAt(1));
								if (resultado != null)
								{
									List<string> permisos = new List<string>();
									foreach (string permiso in resultado)
									{
										if (Analizador.ExisteDB(permiso))
										{
											permisos.Add(permiso);
										}
										else
										{
											erroresChison.Add(new Error(TipoError.Semantico, "No se puede asignar el permiso sobre la base de datos '" + permiso + "' si no existe",
														nodo.Span.Location.Line, nodo.Span.Location.Column,
														Datos.GetDate(), Datos.GetTime()));
										}
									}
									usuario.Permisos = resultado;
								}

							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'PERMISSIONS' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'PERMISSIONS' debe ser una lista",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					default:
						erroresChison.Add(new Error(TipoError.Advertencia,
							"Estructura de 'usuario' es incorrecta. Solamente se deben incluir los atributos 'NAME','PERMISSIONS' y 'PASSWORD'",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
						break;
				}
			}
			if (usuario.IsValido()) return usuario;
			return null;
		}

		private static List<string> GetListaPermisosUsuario(ParseTreeNode parseTreeNode)
		{
			List<string> permisos = new List<string>();
			if (parseTreeNode.ChildNodes.Count > 0)
			{
				ParseTreeNode raiz;
				foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
				{
					if (nodo.Term.Name == "OBJETO")
					{
						raiz = nodo.ChildNodes.ElementAt(0);
						if (raiz.ChildNodes.ElementAt(0).Token.ValueString.ToLower() == "name")
						{

							if (raiz.ChildNodes.ElementAt(1).Term.Name == "cadena")
							{
								if (!permisos.Contains(raiz.ChildNodes.ElementAt(1).Token.ValueString.ToLower()))
								{
									permisos.Add(raiz.ChildNodes.ElementAt(1).Token.ValueString.ToLower());
								}
								else
								{
									erroresChison.Add(new Error(TipoError.Semantico,
										"El permiso ya existe para el usuario",
										raiz.Span.Location.Line,
										raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));
								}
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' debe ser un dato tipo string",
								raiz.Span.Location.Line,
								raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							erroresChison.Add(new Error(TipoError.Advertencia,
							"Estructura de 'permiso' es incorrecta. Solamente se deben incluir el atributo 'NAME'",
							raiz.Span.Location.Line,
							raiz.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
						}
					}
					else
					{
						erroresChison.Add(new Error(TipoError.Semantico,
						"La lista debe estar compuesta solamente por permisos",
						nodo.Span.Location.Line, nodo.Span.Location.Column,
						Datos.GetDate(),
						Datos.GetTime()));

					}
				}

			}
			//LISTA VACIA
			return permisos;
		}

		public static object CasteoImplicito(TipoObjetoDB tipo, object res, BaseDatos db, int linea, int columna)
		{
			switch (tipo.Tipo)
			{
				case TipoDatoDB.INT:
					{
						if (double.TryParse(res.ToString(), out double d2))
						{
							int entero = (int)d2;
							if (entero <= -2147483648 || entero >= 2147483647)
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"El número está fuera de rango",
									linea, columna);
							}
							return entero;
						}
						break;
					}
				case TipoDatoDB.DOUBLE:
					{
						if (double.TryParse(res.ToString(), out double d2))
						{
							if (d2 <= -9223372036854775800 || d2 >= 9223372036854775800)
							{
								return new ThrowError(TipoThrow.ArithmeticException,
									"El número está fuera de rango",
									linea, columna);
							}

							return d2;
						}

						break;
					}
				case TipoDatoDB.DATE:
					if (res.GetType() == typeof(MyDateTime))
					{
						MyDateTime dt = (MyDateTime)res;
						if (dt.Dato.Year > 2999)
						{
							return new ThrowError(TipoThrow.Exception,
								"El año está fuera de rango",
								linea, columna);
						}
					}
					if (res.Equals("null"))
					{
						return new MyDateTime();
					}
					break;
				case TipoDatoDB.TIME:
					if (res.Equals("null"))
					{
						return new MyDateTime();
					}
					break;
				case TipoDatoDB.STRING:
					if (res.Equals("null"))
					{
						return "$%_null_%$";
					}
					else if (res.ToString().Length >= 2147483647)
					{
						return new ThrowError(TipoThrow.Exception,
								"La cadena está fuera de rango",
								linea, columna);
					}
					break;
				case TipoDatoDB.MAP_OBJETO:
				case TipoDatoDB.MAP_PRIMITIVO:
					if (res.Equals("null"))
					{
						return new CollectionMapCql();
					}
					else if (res.GetType() == typeof(FilaDatos))
					{
						FilaDatos objeto = (FilaDatos)res;
						//VALIDANDO TIPO EN FILA
						bool bandddera = false;
						foreach (ParDatos fd in objeto.Datos) {
							if (fd.Valor.GetType()==typeof(FilaDatos)) {
								bandddera = true;
								break;
							}
						}
						if (!bandddera)
						{
							string[] tipos = tipo.Nombre.Split(',');
							if (tipos.Length > 1)
							{
								TipoObjetoDB tipoClave = Datos.GetTipoObjetoDBPorCadena(tipos[0]);
								TipoObjetoDB tipoValor = Datos.GetTipoObjetoDBPorCadena(tipos[1]);
								CollectionMapCql collection = new CollectionMapCql(tipoClave,tipoValor);

								foreach (ParDatos valor in objeto.Datos)
								{
									object valorNuevo = Datos.GetValor(valor.Nombre);
									if (IsTipoCompatible(tipoClave, valorNuevo) && IsTipoCompatible(tipoValor, valor.Valor))
									{
										
										object objeto1 = CasteoImplicito(tipoClave, valorNuevo, db, linea, columna);
										if (objeto1 != null)
										{
											if (objeto1.GetType() == typeof(ThrowError))
											{
												erroresChison.Add(new Error((ThrowError)objeto1, true));
											}
											else
											{
												object objeto2 = CasteoImplicito(tipoValor, valor.Valor, db, linea, columna);
												if (objeto2 != null)
												{
													if (objeto2.GetType() == typeof(ThrowError))
													{
														erroresChison.Add(new Error((ThrowError)objeto2, true));
													}
													else
													{

														collection.AddItem(objeto1, objeto2, linea, columna);
													}
												}
											}
										}
									}
									else
									{
										erroresChison.Add(new Error(TipoError.Semantico,
											"El valor no concuerda con el tipo de dato " + tipo.ToString(),
											  linea, columna,
											  Datos.GetDate(), Datos.GetTime()));
									}
								}
								return collection;
							}
							else {
								erroresChison.Add(new Error(TipoError.Semantico,
										"El tipo de dato no concuerda con el tipo de un Collection " + tipo.ToString(),
										  linea, columna,
										  Datos.GetDate(), Datos.GetTime()));
							}
						}
						else {
							erroresChison.Add(new Error(TipoError.Semantico,
										"Los valores no concuerdan con un Collection tipo Map",
										  linea, columna,
										  Datos.GetDate(), Datos.GetTime()));
						}
						

					}
					break;
				case TipoDatoDB.LISTA_PRIMITIVO:
				case TipoDatoDB.LISTA_OBJETO:
					if (res.Equals("null"))
					{
						return new CollectionListCql(true);
					} else if (res.GetType()==typeof(List<object>)) {
						List<object> objeto = (List<object>)res;
						CollectionListCql collection = new CollectionListCql(tipo, true);
						TipoObjetoDB tipoAdentro = Datos.GetTipoObjetoDBPorCadena(tipo.Nombre);
						foreach (object valor in objeto) {
							if (IsTipoCompatible(tipoAdentro, valor))
							{
								object objeto1 = CasteoImplicito(tipoAdentro, valor, db, linea, columna);
								if (objeto1 != null)
								{
									if (objeto1.GetType() == typeof(ThrowError))
									{
										erroresChison.Add(new Error((ThrowError)objeto1, true));
									}
									else
									{
										collection.AddItem(objeto1, linea, columna);
									}
								}
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
									"El valor no concuerda con el tipo de dato "+tipo.ToString(),
									  linea, columna,
									  Datos.GetDate(), Datos.GetTime()));
							}
						}
						return collection;
					}
					break;
				case TipoDatoDB.SET_PRIMITIVO:
				case TipoDatoDB.SET_OBJETO:
					if (res.Equals("null"))
					{
						return new CollectionListCql(true);
					}
					else if (res.GetType() == typeof(List<object>))
					{
						List<object> objeto = (List<object>)res;
						CollectionListCql collection = new CollectionListCql(tipo, false);
						TipoObjetoDB tipoAdentro = Datos.GetTipoObjetoDBPorCadena(tipo.Nombre);
						foreach (object valor in objeto)
						{
							if (IsTipoCompatible(tipoAdentro, valor))
							{
								object objeto1 = CasteoImplicito(tipoAdentro, valor, db, linea, columna);
								if (objeto1 != null)
								{
									if (objeto1.GetType() == typeof(ThrowError))
									{
										erroresChison.Add(new Error((ThrowError)objeto1, true));
									}
									else
									{
										collection.AddItem(objeto1, linea, columna);
									}
								}
							}
							else
							{
								erroresChison.Add(new Error(TipoError.Semantico,
									"El valor no concuerda con el tipo de dato " + tipo.ToString(),
									  linea, columna,
									  Datos.GetDate(), Datos.GetTime()));
							}
						}
						return collection;
					}
					break;
				case TipoDatoDB.OBJETO:
					if (res.Equals("null"))
					{
						object obj = GetInstanciaObjeto(tipo, db, linea, columna);
						 if (obj != null)
						{
							if (obj.GetType() == typeof(ThrowError))
							{
								return obj;
							}
							if (obj.GetType() == typeof(Objeto))
							{
								((Objeto)obj).IsNull = true;
								return obj;
							}
						}
					}
					else
					{
						FilaDatos fila = (FilaDatos)res;
						UserType usertype = GetUt(fila.Datos, db);
						if (usertype != null)
						{
							Objeto obj = new Objeto(usertype);
							foreach (ParDatos par in fila.Datos)
							{
								TipoObjetoDB tipodato = usertype.Atributos[par.Nombre];
								if (tipodato!=null) {
									if (IsTipoCompatible(tipodato,par.Valor))
									{
										object objeto = CasteoImplicito(tipodato, par.Valor, db, linea, columna);
										if (objeto != null)
										{
											if (objeto.GetType() == typeof(ThrowError))
											{
												erroresChison.Add(new Error((ThrowError)objeto, true));
											}
											else
											{
												obj.Atributos.Add(par.Nombre, objeto);
											}
										}
									}
									else
									{
										erroresChison.Add(new Error(TipoError.Semantico,
											"El valor no concuerda con el tipo de dato '" + par.Nombre + "'(" + tipodato.ToString() + ")",
											  linea, columna,
											  Datos.GetDate(), Datos.GetTime()));
									}
								}

							}
							if (obj.Atributos.Count == usertype.Atributos.Count)
							{
								return obj;
							}
							else {
								erroresChison.Add(new Error(TipoError.Semantico,
											"La cantidad de atrbutos no concuerda con el User Type '"+usertype.Nombre+"'",
											  linea, columna,
											  Datos.GetDate(), Datos.GetTime()));
							}
						}
						return null;
					}
					break;
			}

			return res;
		}

		private static UserType GetUt(List<ParDatos> attrs, BaseDatos tab)
		{
			foreach (UserType ut in tab.UserTypes)
			{
				if (ut.Atributos.Count == attrs.Count)
				{
					bool contienetodo = true;
					int contadorAt = 0;
					foreach (KeyValuePair<string, TipoObjetoDB> atributo in ut.Atributos)
					{
						if (!atributo.Key.Equals(attrs.ElementAt(contadorAt).Nombre))
						{
							contienetodo = false;
							break;
						}
						contadorAt++;
					}
					if (contienetodo)
					{
						return ut;
					}
				}
			}
			return null;
		}

		public static object GetInstanciaObjeto(TipoObjetoDB tipoInstancia, BaseDatos db, int linea, int columna)
		{

			if (db.ExisteUserType(tipoInstancia.ToString()))
			{
				UserType ut = db.BuscarUserType(tipoInstancia.ToString());
				Objeto nuevaInstancia = new Objeto(ut);
				foreach (KeyValuePair<string, TipoObjetoDB> atributo in ut.Atributos)
				{
					object valPre = GetValorPredeterminado(atributo.Value, db, linea, columna);
					if (valPre != null)
					{
						if (valPre.GetType() == typeof(ThrowError))
						{
							return valPre;
						}
						nuevaInstancia.Atributos.Add(atributo.Key, valPre);

					}
				}
				return nuevaInstancia;
			}
			else
			{
				return new ThrowError(Util.TipoThrow.UseBDException,
					"No existe el tipo '" + tipoInstancia.ToString() + "'",
					linea, columna);
			}
		}

		public static object GetValorPredeterminado(TipoObjetoDB tipo, BaseDatos db, int Linea, int Columna)
		{
			switch (tipo.Tipo)
			{
				case TipoDatoDB.BOOLEAN:
					return false;
				case TipoDatoDB.DOUBLE:
					return 0.0;
				case TipoDatoDB.COUNTER:
				case TipoDatoDB.INT:
					return 0;
				case TipoDatoDB.STRING:
					return "$%_null_%$";
				case TipoDatoDB.LISTA_OBJETO:
				case TipoDatoDB.LISTA_PRIMITIVO:
				case TipoDatoDB.MAP_OBJETO:
				case TipoDatoDB.MAP_PRIMITIVO:
				case TipoDatoDB.SET_OBJETO:
				case TipoDatoDB.SET_PRIMITIVO:
					return "null";
				case TipoDatoDB.DATE:
				case TipoDatoDB.TIME:
					return new MyDateTime();
				case TipoDatoDB.OBJETO:

					if (db.ExisteUserType(tipo.Nombre))
					{

						//buscar objeto;
						Objeto objeto = new Objeto(db.BuscarUserType(tipo.Nombre), true);
						return objeto;
					}
					else
					{
						return new ThrowError(Util.TipoThrow.TypeDontExists,
					"El user Type '" + tipo.Nombre + "' no existe",
					Linea, Columna);
					}
				default:
					return "null";
			}
		}

		public static bool IsTipoCompatible(TipoObjetoDB tipoDato, object v)
		{
			switch (tipoDato.Tipo)
			{
				case TipoDatoDB.BOOLEAN:
					return v.GetType() == typeof(bool);
				case TipoDatoDB.DOUBLE:
					return v.GetType() == typeof(double);
				case TipoDatoDB.COUNTER:
				case TipoDatoDB.INT:
					return v.GetType() == typeof(int);
				case TipoDatoDB.DATE:
					if (v.GetType() == typeof(MyDateTime))
					{
						return ((MyDateTime)v).Tipo.Equals(TipoDatoDB.DATE);
					} else if (v.Equals("null")) {
						return true;
					}
					return false;
				case TipoDatoDB.NULO:
					if (v.GetType() == typeof(string))
					{
						return v.ToString().ToLower().Equals("null");
					}
					return false;
				case TipoDatoDB.STRING:
					return (v.GetType() == typeof(string));
				case TipoDatoDB.TIME:
					if (v.GetType() == typeof(MyDateTime))
					{
						return ((MyDateTime)v).Tipo.Equals(TipoDatoDB.TIME);
					}
					else if (v.Equals("null"))
					{
						return true;
					}
					return false;
				case TipoDatoDB.LISTA_OBJETO:
				case TipoDatoDB.SET_OBJETO:
				case TipoDatoDB.LISTA_PRIMITIVO:
				case TipoDatoDB.SET_PRIMITIVO:
					if (v.GetType()==typeof(List<object>)) {
						return true;
					}
					else if (v.Equals("null"))
					{
						return true;
					}
					break;
				case TipoDatoDB.MAP_PRIMITIVO:
				case TipoDatoDB.MAP_OBJETO:
					if (v.GetType() == typeof(FilaDatos))
					{
						return true;
					}
					else if (v.Equals("null"))
					{
						return true;
					}
					break;
				case TipoDatoDB.OBJETO:
					if (v.GetType() == typeof(FilaDatos))
					{
						return true;
					} else if (v.ToString().Equals("null")) {
						return true;
					}
					return false;
			}
			return false;
		}
	}
}
