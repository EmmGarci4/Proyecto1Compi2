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
						Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
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
				Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
					   "Estructura principal incorrecta. Se debe incluir el atributo 'DATABASES'",
					   raiz.Span.Location.Line,
					   raiz.Span.Location.Column,
					   Datos.GetDate(),
					   Datos.GetTime()));
				todoBien = false;
			}
			if (indexUs < 0)
			{
				Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
					Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
					Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
					Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
					Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
										if (!based.ExisteUserType(ut.Nombre))
										{
											based.AgregarUserType(ut);
										}
										else
										{
											Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
												"El user Type '" + ut.Nombre + "' ya existe",
												nodo.ChildNodes.ElementAt(1).Span.Location.Line,
												nodo.ChildNodes.ElementAt(1).Span.Location.Column,
												Datos.GetDate(), Datos.GetTime()));
										}
									}
									else if (objetodb.GetType() == typeof(Procedimiento))
									{
										Procedimiento ut = (Procedimiento)objetodb;
										if (!based.ExisteProcedimiento(ut.Nombre))
										{
											based.AgregarProcedimiento(ut);
										}
										else
										{
											Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
												"El procedimiento '" + ut.Nombre + "' ya existe",
												nodo.ChildNodes.ElementAt(1).Span.Location.Line,
												nodo.ChildNodes.ElementAt(1).Span.Location.Column,
												Datos.GetDate(), Datos.GetTime()));
										}
									}
									else if (objetodb.GetType() == typeof(Tabla))
									{
										Tabla ut = (Tabla)objetodb;
										if (!based.ExisteTabla(ut.Nombre))
										{
											based.AgregarTabla(ut);
										}
										else
										{
											Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
												"La tabla '" + ut.Nombre + "' ya existe",
												nodo.ChildNodes.ElementAt(1).Span.Location.Line,
												nodo.ChildNodes.ElementAt(1).Span.Location.Column,
												Datos.GetDate(), Datos.GetTime()));
										}
									}
								}
							}
							else
							{
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'DATA' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'DATA' debe ser una lista",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					default:
						Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
							"Estructura de 'base de datos' es incorrecta. Solamente se deben incluir los atributos 'NAME' y 'DATA'",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
						break;
				}
			}
			if (based.IsValido()) return based;
			Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
				"No se incluyó alguno de los atributos 'NAME' o 'DATA'",
				raiz.Span.Location.Line,
				raiz.Span.Location.Column,
				Datos.GetDate(),
				Datos.GetTime()));
			return null;
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
							UserType ustype = GetUserType(nodo,lista);
							if (ustype != null) lista.Add(ustype);
							break;
						case TipoObjeto.Procedimiento:
							Procedimiento proc = GetProcedimiento(nodo,lista);
							if (proc != null) lista.Add(proc);
							break;
						case TipoObjeto.Tabla:
							Tabla tab = GetTabla(nodo, lista);
							if (tab != null) lista.Add(tab);
							break;
					}
				}
				else
				{
					Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
							"La lista de 'DATA' solo debe contener objetos CQL",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
				}
			}

			return lista;
		}

		private static Tabla GetTabla(ParseTreeNode raiz, List<object> db)
		{
			Tabla tabla = new Tabla();
			string t = null;
			List<object> filas = null;
			List<Columna> columnas = null;
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
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'CQL-TYPE' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'CQL-TYPE' debe ser un dato tipo string",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					case "columns":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "LISTA")
						{
							if (columnas == null)
							{
								columnas = GetColumnasTabla(nodo.ChildNodes.ElementAt(1));
								foreach (Columna col in columnas)
								{
									if (!tabla.ExisteColumna(col.Nombre))
									{
										if (col.Tipo.Tipo == TipoDatoDB.OBJETO)
										{
											if (ExisteUt(col.Tipo.Nombre, db))
											{
												tabla.AgregarColumna(col);
											}
											else
											{
											Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
											"El objeto '" + col.Tipo.ToString() + "' no existe",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));
											}
										}
										else
										{
											if (Datos.IsLista(col.Tipo.Tipo)) {
												if (!ValidarInstanciaLista(col.Tipo, db, raiz.Span.Location.Line, raiz.Span.Location.Column)) {
													continue;
												}
											}
											//validar si es lista, set o map
											tabla.AgregarColumna(col);
										}

									}
									else
									{
										Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
										"La columna '" + col.Nombre + "' no existe",
										raiz.Span.Location.Line, raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));
									}
								}
							}
							else
							{
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'COLUMNS' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'COLUMNS' debe ser una lista",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					case "data":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "LISTA")
						{
							if (filas == null)
							{
								//recuperar e insertar
								List<FilaDatos> datos = GetDatosTabla(nodo.ChildNodes.ElementAt(1), db);
								foreach (FilaDatos fila in datos)
								{
									InsertarEnTabla(tabla, fila.Datos,nodo.Span.Location.Line,nodo.Span.Location.Column);
								}
							}
							else
							{
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'DATA' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'DATA' debe ser una lista",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					default:
						Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
							"Estructura de 'tabla' es incorrecta. Solamente se deben incluir los atributos 'NAME', 'CQL-TYPE', 'COLUMNS' y 'DATA'",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
						break;
				}

			}
			if (tabla.Nombre != null && t != null) return tabla;
			Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
				"No se incluyó alguno de los atributos 'NAME', 'CQL-TYPE', 'COLUMNS' o 'DATA'",
				raiz.Span.Location.Line,
				raiz.Span.Location.Column,
				Datos.GetDate(),
				Datos.GetTime()));
			return null;
		}

		private static bool ValidarInstanciaLista(TipoObjetoDB tipoInstancia, List<object> objetos,int linea,int columna)
		{
			switch (tipoInstancia.Tipo)
			{
				case TipoDatoDB.LISTA_OBJETO:
				case TipoDatoDB.SET_OBJETO:
					TipoObjetoDB tipoAdentro = Datos.GetTipoObjetoDBPorCadena(tipoInstancia.Nombre);
					if (Datos.IsLista(tipoAdentro.Tipo))
					{
						if (!ValidarInstanciaLista(tipoAdentro, objetos,linea,columna))
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
						if (!ExisteUt(tipoAdentro.Nombre,objetos))
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El objeto '"+tipoAdentro.Nombre+"' no existe",
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
						if (!ValidarInstanciaLista(tipoAdentro, objetos,linea,columna))
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
						if (!ExisteUt(tipoAdentro.Nombre, objetos))
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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

		private static void InsertarEnTabla(Tabla tab, List<ParDatos> valores,int linea,int columna)
		{
			if (valores.Count == tab.Columnas.Count)
			{
				//VALIDANDO
				int indiceDatos = 0;
				int indiceColumnas = 0;
				Queue<object> valoresAInsertar = new Queue<object>();
				foreach (Columna cl in tab.Columnas)
				{
					ParDatos respuesta = valores.ElementAt(indiceDatos);

						if (Datos.IsTipoCompatible(cl.Tipo, respuesta.Valor))
						{
							valoresAInsertar.Enqueue(respuesta.Valor);
						}
						else
						{
						TipoObjetoDB tipoRes = Datos.GetTipoObjetoDB(respuesta.Valor);
						if (Datos.IsLista(tipoRes.Tipo))
						{
							CollectionListCql colection = (CollectionListCql)respuesta.Valor;
							if (tipoRes.Nombre == "null")
							{
								colection.TipoDato = cl.Tipo;
								valoresAInsertar.Enqueue(colection);
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El valor No." + (indiceDatos + 1) + " no concuerda con el tipo de dato '" + cl.Nombre + "'(" + cl.Tipo.ToString() + ")",
								  linea, columna,
								  Datos.GetDate(), Datos.GetTime()));
						}
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
						Analizador.ErroresChison.Add(new Error((ThrowError)correcto,true));
					}
					else {
						//LLENANDO TABLA
						tab.AgregarValores(valoresAInsertar);
					}
					
				}
			}
			else
			{
				Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
						"La cantidad de valores no concuerda con la cantidad de columnas en las que se puede insertar",
						linea, columna,
						Datos.GetDate(), Datos.GetTime()));
			}
		}

		private static bool ExisteUt(string nombre, List<object> db)
		{
			List<UserType> usertpypes = GetUserTypesLista(db);
			foreach (UserType ut in usertpypes)
			{
				if (ut.Nombre.Equals(nombre))
				{
					return true;
				}
			}
			return false;
		}

		private static List<FilaDatos> GetDatosTabla(ParseTreeNode parseTreeNode, List<object> db)
		{
			List<FilaDatos> datos = new List<FilaDatos>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
			{
				if (nodo.Term.Name == "OBJETO")
				{
					FilaDatos fil = GetFilaDatos(nodo, db);
					if (fil!=null){
						datos.Add(fil);
					}
				}
				else
				{
					Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
				}
				else {
					return null;
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
					return Datos.GetValor(parseTreeNode.Token.ValueString.ToString());
				case "pr_true":
					return Datos.GetValor(parseTreeNode.Token.ValueString.ToString());
				case "pr_false":
					return Datos.GetValor(parseTreeNode.Token.ValueString.ToString());
				case "NULL":
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
						Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
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
						Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
									"La hora es incorrecta, el formato debe ser HH:MM:SS",
								   linea, column,
								   Datos.GetDate(), Datos.GetTime()));
					}
					return di;
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
					if (lista.Count > 0)
					{
						object primer_elemento = lista.ElementAt(0);
						TipoObjetoDB tipodato = Datos.GetTipoObjetoDB(primer_elemento);
						TipoObjetoDB tipoCol = null;
						if (Datos.IsPrimitivo(tipodato.Tipo))
						{
							tipoCol = new TipoObjetoDB(TipoDatoDB.LISTA_PRIMITIVO, tipodato.ToString());
						}
						else
						{
							tipoCol = new TipoObjetoDB(TipoDatoDB.LISTA_OBJETO, tipodato.ToString());
						}
						CollectionListCql collection = new CollectionListCql(tipoCol, true);
						foreach (object exp in lista)
						{
							if (Datos.IsTipoCompatible(Datos.GetTipoObjetoDBPorCadena(collection.TipoDato.Nombre), exp))
							{

								object posibleError = collection.AddItem(exp, linea, column);
								if (posibleError != null)
								{
									if (posibleError.GetType() == typeof(ThrowError))
									{
										Analizador.ErroresChison.Add(new Error((ThrowError)posibleError, true));
									}
								}
							}
							else
							{
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
									"No se puede almacenar un valor " + Datos.GetTipoObjetoDB(exp) + " en un Collection tipo " + collection.TipoDato.ToString(),
									linea, column, Datos.GetDate(), Datos.GetTime()));
							}

						}
						return collection;
					}
					else
					{
						return new CollectionListCql(new TipoObjetoDB(TipoDatoDB.LISTA_OBJETO,"null"),true);
					}
				case "OBJETO":
					FilaDatos atributos = GetFilaDatos(parseTreeNode, db);
					List<object> expresiones = new List<object>();
					List<String> attrs = new List<string>();
					foreach (ParDatos par in atributos.Datos)
					{
						attrs.Add(par.Nombre);
						expresiones.Add(par.Valor);
					}
					UserType usert = GetUt(attrs, db);
					if (usert != null)
					{
						//CREANDO INSTANCIAS***************************
						Objeto nuevaInstancia = new Objeto(usert);
						int indice = 0;
						if (usert.Atributos.Count == expresiones.Count)
						{
							foreach (KeyValuePair<string, TipoObjetoDB> atributo in usert.Atributos)
							{
								if (Datos.IsTipoCompatibleParaAsignar(atributo.Value, expresiones.ElementAt(indice)))
								{
									nuevaInstancia.Atributos.Add(atributo.Key, expresiones.ElementAt(indice));
								}
								else
								{
									Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"Los atributos no corresponden al tipo '" + usert.Nombre + "'",
								linea, column, Datos.GetDate(), Datos.GetTime()));
								}
								indice++;
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"Los atributos no corresponden en numero al tipo '" + usert.Nombre + "'",
								linea, column, Datos.GetDate(), Datos.GetTime()));
						}
						return nuevaInstancia;
						//*************************************************
					}
					else
					{
						//error
						Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
									"No existe un objeto con esos atributos en la base de datos",
								   linea, column, Datos.GetDate(), Datos.GetTime()));
					}
					break;
			}
			return null;
		}

		private static UserType GetUt(List<string> attrs, List<object> tab)
		{
			List<UserType> usertpypes = GetUserTypesLista(tab);
			foreach (UserType ut in usertpypes)
			{
				if (ut.Contiene(attrs))
				{
					return ut;
				}
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

		private static List<Columna> GetColumnasTabla(ParseTreeNode parseTreeNode)
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
										Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
										"El atributo 'NAME' solo debe aparecer una vez",
										raiz.Span.Location.Line, raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));
									}
								}
								else
								{
									Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
										Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
										"El atributo 'TYPE' solo debe aparecer una vez",
										raiz.Span.Location.Line, raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));
									}
								}
								else
								{
									Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
										Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
										"El atributo 'AS' solo debe aparecer una vez",
										raiz.Span.Location.Line, raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));
									}
								}
								break;
							default:
								Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
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
					}
					else
					{
						Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"No se incluyó alguno de los atributos 'NAME','PK' o 'TYPE'",
								col.Span.Location.Line, col.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
					}
				}
				else
				{
					Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'COLUMNS' solo puede contener columnas",
								col.Span.Location.Line, col.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
				}
			}
			return columnas;
		}

		private static Procedimiento GetProcedimiento(ParseTreeNode raiz,List<object> db)
		{
			Procedimiento proc = new Procedimiento(raiz.Span.Location.Line, raiz.Span.Location.Column);
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
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'CQL-TYPE' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
								List<Parametro> resultado = GetListaParametros(nodo.ChildNodes.ElementAt(1));
								if (resultado != null)
								{
									foreach (Parametro par in resultado)
									{
										if (!proc.Parametros.Contains(par))
										{
											if (par.Tipo.Tipo == TipoDatoDB.OBJETO)
											{
												if (ExisteUt(par.Tipo.Nombre, db))
												{
													proc.Parametros.Add(par);
												}
												else
												{
													Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
													"El objeto '" + par.Tipo.ToString() + "' no existe",
													raiz.Span.Location.Line, raiz.Span.Location.Column,
													Datos.GetDate(),
													Datos.GetTime()));
												}
											}
											else
											{
												if (Datos.IsLista(par.Tipo.Tipo))
												{
													if (ValidarInstanciaLista(par.Tipo, db, raiz.Span.Location.Line, raiz.Span.Location.Column))
													{
														proc.Parametros.Add(par);
													}
												}
												else {
													proc.Parametros.Add(par);
												}
												
											}
											
										}
										else
										{
											Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
												"El parámetro '" + par.Nombre + "' ya existe",
												raiz.Span.Location.Line, raiz.Span.Location.Column,
												Datos.GetDate(),
												Datos.GetTime()));
										}
									}
								}

								//RETORNOS 
								resultado = GetListaRetornos(nodo.ChildNodes.ElementAt(1));
								if (resultado != null)
								{
									foreach (Parametro par in resultado)
									{
										if (!proc.Retornos.Contains(par))
										{
											if (par.Tipo.Tipo == TipoDatoDB.OBJETO)
											{
												if (ExisteUt(par.Tipo.Nombre, db))
												{
													proc.Retornos.Add(par);
												}
												else
												{
													Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
													"El objeto '" + par.Tipo.ToString() + "' no existe",
													raiz.Span.Location.Line, raiz.Span.Location.Column,
													Datos.GetDate(),
													Datos.GetTime()));
												}
											}
											else
											{
												if (Datos.IsLista(par.Tipo.Tipo))
												{
													if (ValidarInstanciaLista(par.Tipo, db,raiz.Span.Location.Line, raiz.Span.Location.Column))
													{
														proc.Retornos.Add(par);
													}
												}
												else
												{
													proc.Retornos.Add(par);
												}
											}
										}
										else
										{
											Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
												"El retorno '" + par.Nombre + "' ya existe",
												raiz.Span.Location.Line, raiz.Span.Location.Column,
												Datos.GetDate(),
												Datos.GetTime()));
										}
									}
								}
							}
							else
							{
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'PARAMETERS' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'INSTR' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'INSTR' debe ser una cadena encerrada entre $",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					default:
						Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
							"Estructura de 'CQL-Type Procedure' es incorrecta. Solamente se deben incluir los atributos 'INSTR','NAME','PARAMETERS' y 'CQL-TYPE'",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
						break;
				}
			}
			if (proc.isValido() && t != null && bren != null) return proc;
			Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
				"No se incluyó alguno de los atributos 'NAME','PARAMETERS', 'INSTR' o 'CQL-TYPE'",
				raiz.Span.Location.Line,
				raiz.Span.Location.Column,
				Datos.GetDate(),
				Datos.GetTime()));
			return null;
		}

		private static List<Parametro> GetListaRetornos(ParseTreeNode rai)
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
											Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
											"El atributo 'NAME' solo debe aparecer una vez",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));
										}
									}
									else
									{
										Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
											Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
											"El atributo 'TYPE' solo debe aparecer una vez",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));
										}
									}
									else
									{
										Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
											Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
											"El atributo 'AS' solo debe aparecer una vez",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));
										}
									}
									break;
								default:
									Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
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
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
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
					Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
							"La lista de 'PARAMETERS' solo debe contener parámetros",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
				}
			}
			return dic;
		}

		private static List<Parametro> GetListaParametros(ParseTreeNode rai)
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
											Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
											"El atributo 'NAME' solo debe aparecer una vez",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));
										}
									}
									else
									{
										Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
											Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
											"El atributo 'TYPE' solo debe aparecer una vez",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));
										}
									}
									else
									{
										Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
											Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
											"El atributo 'AS' solo debe aparecer una vez",
											raiz.Span.Location.Line, raiz.Span.Location.Column,
											Datos.GetDate(),
											Datos.GetTime()));
										}
									}
									break;
								default:
									Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
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
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
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
					Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
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
						Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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

		private static UserType GetUserType(ParseTreeNode raiz,List<object> db)
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
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'CQL-TYPE' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
								Dictionary<string, TipoObjetoDB> resultado = GetListaAtributos(nodo.ChildNodes.ElementAt(1),db);
								if (resultado != null)
									user.Atributos = resultado;
							}
							else
							{
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'ATTRS' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'ATTRS' debe ser una lista",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					default:
						Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
							"Estructura de 'CQL-Type object' es incorrecta. Solamente se deben incluir los atributos 'NAME','ATTRS' y 'CQL-TYPE'",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
						break;
				}
			}

			if (user.IsValido() && t != null)
			{
				return user;
			}
			Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
				"No se incluyó alguno de los atributos 'NAME','ATTRS' o 'CQL-TYPE'",
				raiz.Span.Location.Line,
				raiz.Span.Location.Column,
				Datos.GetDate(),
				Datos.GetTime()));
			return null;
		}

		private static Dictionary<string, TipoObjetoDB> GetListaAtributos(ParseTreeNode parseTreeNode,List<object> db)
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
										Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
										"El atributo 'NAME' solo debe aparecer una vez",
										raiz.Span.Location.Line, raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));
									}
								}
								else
								{
									Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
										TipoObjetoDB tipodato = Datos.GetTipoObjetoDBPorCadena(raiz.ChildNodes.ElementAt(1).Token.ValueString.ToLower());
										if (tipodato.Tipo == TipoDatoDB.OBJETO)
										{
											if (ExisteUt(tipodato.Nombre, db))
											{
												tipo = tipodato.ToString() ;
											}
											else
											{
												Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
												"El objeto '" + tipodato.ToString() + "' no existe",
												raiz.Span.Location.Line, raiz.Span.Location.Column,
												Datos.GetDate(),
												Datos.GetTime()));
											}
										}
										else
										{
											if (Datos.IsLista(tipodato.Tipo))
											{
												if (ValidarInstanciaLista(tipodato, db, raiz.Span.Location.Line, raiz.Span.Location.Column))
												{
													tipo = tipodato.ToString();
												}
											}
											else {
												tipo = tipodato.ToString();
											}
											
										}
									}
									else
									{
										Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
										"El atributo 'TYPE' solo debe aparecer una vez",
										raiz.Span.Location.Line, raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));
									}
								}
								else
								{
									Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
										"El atributo 'TYPE' debe ser un dato tipo string",
										raiz.Span.Location.Line, raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));

								}
								break;
							default:
								Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
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
							Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
							"El atributo '" + nombre + "' ya existe",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
						}
					}
					else
					{
						Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
							"No se incluyó alguno de los atributos 'NAME' o 'TYPE'",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
					}
				}
				else
				{
					Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
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
						Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
					Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
					Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'PASSWORD' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
								if (resultado != null) {
									List<string> permisos = new List<string>();
									foreach (string permiso in resultado) {
										if (Analizador.ExisteDB(permiso))
										{
											permisos.Add(permiso);
										}
										else {
											Analizador.ErroresChison.Add(new Error(TipoError.Semantico, "No se puede asignar el permiso sobre la base de datos '" + permiso + "' si no existe",
														nodo.Span.Location.Line,nodo.Span.Location.Column, 
														Datos.GetDate(), Datos.GetTime()));
										}
									}
									usuario.Permisos = resultado;
								}
									
							}
							else
							{
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'PERMISSIONS' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'PERMISSIONS' debe ser una lista",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

						}
						break;
					default:
						Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
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
									Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
										"El permiso ya existe para el usuario",
										raiz.Span.Location.Line,
										raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));
								}
							}
							else
							{
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' debe ser un dato tipo string",
								raiz.Span.Location.Line,
								raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else
						{
							Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
							"Estructura de 'permiso' es incorrecta. Solamente se deben incluir el atributo 'NAME'",
							raiz.Span.Location.Line,
							raiz.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
						}
					}
					else
					{
						Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
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
	}
}
