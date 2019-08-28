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
			foreach (ParseTreeNode nodo in raiz.ChildNodes.ElementAt(0).ChildNodes) {
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
				if (indexUs<0) {
				Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
						   "Estructura principal incorrecta. Se debe incluir el atributo 'USERS'",
						   raiz.Span.Location.Line, 
						   raiz.Span.Location.Column, 
						   Datos.GetDate(), 
						   Datos.GetTime()));
				todoBien = false;
				}
			if (todoBien) {
				raiz = raiz.ChildNodes.ElementAt(0);
				//recorrer e insertar bases de datos
				if (raiz.ChildNodes.ElementAt(indexDb).ChildNodes.ElementAt(1).Term.Name == "LISTA")
				{
					RecorrerBasesDeDatos(raiz.ChildNodes.ElementAt(indexDb).ChildNodes.ElementAt(1));
				}
				else {
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
								based.Nombre = nodo.ChildNodes.ElementAt(1).Token.ValueString;
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
										else {
											Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
												"El user Type '" + ut.Nombre + "' ya existe",
												nodo.ChildNodes.ElementAt(1).Span.Location.Line,
												nodo.ChildNodes.ElementAt(1).Span.Location.Column));
										}
									}else if (objetodb.GetType() == typeof(Procedimiento))
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
												nodo.ChildNodes.ElementAt(1).Span.Location.Column));
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
												nodo.ChildNodes.ElementAt(1).Span.Location.Column));
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
			foreach (ParseTreeNode nodo in raiz.ChildNodes) {
				if (nodo.Term.Name == "OBJETO")
				{
					TipoObjeto t = GetTipoObjetoCql(nodo);
					switch (t) {
						case TipoObjeto.Objeto:
							UserType ustype = GetUserType(nodo);
							if(ustype!=null)lista.Add(ustype);
							break;
						case TipoObjeto.Procedimiento:
							Procedimiento proc = GetProcedimiento(nodo);
							if (proc != null) lista.Add(proc);
							break;
						case TipoObjeto.Tabla:
							Tabla tab = GetTabla(nodo);
							if (tab != null) lista.Add(tab);
							break;
					}
				}
				else {
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

		private static Tabla GetTabla(ParseTreeNode raiz)
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
									tabla.Nombre = nodo.ChildNodes.ElementAt(1).Token.ValueString;
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
								t = nodo.ChildNodes.ElementAt(1).Token.ValueString;
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
											tabla.AgregarColumna(col);
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
			if (tabla.Nombre!=null&&t!=null) return tabla;
			Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
				"No se incluyó alguno de los atributos 'NAME', 'CQL-TYPE', 'COLUMNS' o 'DATA'",
				raiz.Span.Location.Line,
				raiz.Span.Location.Column,
				Datos.GetDate(),
				Datos.GetTime()));
			return null;
		}

		private static List<Columna> GetColumnasTabla(ParseTreeNode parseTreeNode)
		{
			List<Columna> columnas = new List<Columna>();
			foreach (ParseTreeNode col in parseTreeNode.ChildNodes) {
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
										cl.Nombre = raiz.ChildNodes.ElementAt(1).Token.ValueString;
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
										tipo = raiz.ChildNodes.ElementAt(1).Token.ValueString;
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
										isPk = raiz.ChildNodes.ElementAt(1).Token.ValueString;
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
					if (cl.Nombre!=null && tipo != null && isPk != null)
					{
						cl.Tipo = Datos.GetTipoObjetoDBPorCadena(tipo);
						cl.IsPrimary = isPk.ToLower().Equals("true");
						columnas.Add(cl);
					}
					else {
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

		private static Procedimiento GetProcedimiento(ParseTreeNode raiz)
		{
			Procedimiento proc = new Procedimiento(raiz.Span.Location.Line,raiz.Span.Location.Column);
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
								proc.Nombre = nodo.ChildNodes.ElementAt(1).Token.ValueString;
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
								t = nodo.ChildNodes.ElementAt(1).Token.ValueString;
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
							if (bren==null)
							{
								//PARAMETROS
								bren = ":D";
								List<Parametro> resultado = GetListaParametros(nodo.ChildNodes.ElementAt(1));
								if (resultado != null) {
									foreach (Parametro par in resultado) {
										if (!proc.Parametros.Contains(par))
										{
											proc.Parametros.Add(par);
										}
										else {
											Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
												"El parámetro '"+par.Nombre+"' ya existe",
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
											proc.Retornos.Add(par);
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
							if (proc.Instrucciones == null && proc.Sentencias==null)
							{
								//EVALUAR Y ASIGNAR
								String codigo = nodo.ChildNodes.ElementAt(1).Token.ValueString;
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
											nombre = raiz.ChildNodes.ElementAt(1).Token.ValueString;
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
											tipo = raiz.ChildNodes.ElementAt(1).Token.ValueString;
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
											pras = raiz.ChildNodes.ElementAt(1).Token.ValueString;
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
			foreach (ParseTreeNode nodo in rai.ChildNodes) {
				if (nodo.Term.Name == "OBJETO")
				{
					String nombre = null;
					String tipo = null;
					string pras= null;
					if (IsParametro(nodo)) {
						foreach (ParseTreeNode raiz in nodo.ChildNodes)
						{
							switch (raiz.ChildNodes.ElementAt(0).Token.ValueString.ToLower())
							{
								case "name":
									if (raiz.ChildNodes.ElementAt(1).Term.Name == "cadena")
									{
										if (nombre == null)
										{
											nombre = raiz.ChildNodes.ElementAt(1).Token.ValueString;
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
											tipo = raiz.ChildNodes.ElementAt(1).Token.ValueString;
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
											pras = raiz.ChildNodes.ElementAt(1).Token.ValueString;
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
						if (nombre != null && tipo != null && pras!=null)
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
					if (no.ChildNodes.ElementAt(1).Term.Name == "in"|| no.ChildNodes.ElementAt(1).Term.Name == "out")
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

		private static UserType GetUserType(ParseTreeNode raiz)
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
								user.Nombre = nodo.ChildNodes.ElementAt(1).Token.ValueString;
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
								t=nodo.ChildNodes.ElementAt(1).Token.ValueString;
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
								Dictionary<string,TipoObjetoDB> resultado = GetListaAtributos(nodo.ChildNodes.ElementAt(1));
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

		private static Dictionary<string, TipoObjetoDB> GetListaAtributos(ParseTreeNode parseTreeNode)
		{
			Dictionary<string, TipoObjetoDB> dic = new Dictionary<string, TipoObjetoDB>();
			
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
			{	
				if (nodo.Term.Name == "OBJETO")
				{
					string nombre=null;
					string tipo=null;
					//recorriendo los atributos del 'ATRIB'
					foreach (ParseTreeNode raiz in nodo.ChildNodes) {
						switch (raiz.ChildNodes.ElementAt(0).Token.ValueString.ToLower())
						{
							case "name":
								if (raiz.ChildNodes.ElementAt(1).Term.Name == "cadena")
								{
									if (nombre == null)
									{
										nombre = raiz.ChildNodes.ElementAt(1).Token.ValueString;
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
										tipo = raiz.ChildNodes.ElementAt(1).Token.ValueString;
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
						try {
							dic.Add(nombre, Datos.GetTipoObjetoDBPorCadena(tipo));
						}
						catch (ArgumentException) {
							Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
							"El atributo '"+nombre+"' ya existe",
							nodo.Span.Location.Line,
							nodo.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
						}
					}
					else {
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
			foreach (ParseTreeNode no in nodo.ChildNodes) {
				if (no.ChildNodes.ElementAt(0).Token.ValueString.ToLower()=="cql-type") {
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
					else {
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
			foreach (Usuario nuevo in usuarios) {
				if (!Analizador.ExisteUsuario(nuevo.Nombre))
				{
					Analizador.AddUsuario(nuevo);
				}
				else {
					Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El usuario '"+nuevo.Nombre+"' ya existe en el sistema",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));

				}
			}
		}

		private static Usuario GetUsuario(ParseTreeNode raiz)
		{
			Usuario usuario = new Usuario();
			foreach (ParseTreeNode nodo in raiz.ChildNodes) {
				switch (nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower()) {
					case "name":
						if (nodo.ChildNodes.ElementAt(1).Term.Name == "cadena")
						{
							if (usuario.Nombre == null)
							{
								usuario.Nombre = nodo.ChildNodes.ElementAt(1).Token.ValueString;
							}
							else {
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' solo debe aparecer una vez",
								raiz.Span.Location.Line, raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else {
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
								usuario.Password = nodo.ChildNodes.ElementAt(1).Token.ValueString;
							}
							else {
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
								if(resultado!=null)
								usuario.Permisos =resultado;
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
				foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes) {
					if (nodo.Term.Name == "OBJETO")
					{
						raiz = nodo.ChildNodes.ElementAt(0);
						if (raiz.ChildNodes.ElementAt(0).Token.ValueString.ToLower() == "name")
						{

							if (raiz.ChildNodes.ElementAt(1).Term.Name == "cadena")
							{
								if (!permisos.Contains(raiz.ChildNodes.ElementAt(1).Token.ValueString))
								{
									permisos.Add(raiz.ChildNodes.ElementAt(1).Token.ValueString);
								}
								else {
									Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
										"El permiso ya existe para el usuario",
										raiz.Span.Location.Line,
										raiz.Span.Location.Column,
										Datos.GetDate(),
										Datos.GetTime()));
								}
							}
							else {
								Analizador.ErroresChison.Add(new Error(TipoError.Semantico,
								"El atributo 'NAME' debe ser un dato tipo string",
								raiz.Span.Location.Line, 
								raiz.Span.Location.Column,
								Datos.GetDate(),
								Datos.GetTime()));
							}
						}
						else {
							Analizador.ErroresChison.Add(new Error(TipoError.Advertencia,
							"Estructura de 'permiso' es incorrecta. Solamente se deben incluir el atributo 'NAME'",
							raiz.Span.Location.Line,
							raiz.Span.Location.Column,
							Datos.GetDate(),
							Datos.GetTime()));
						}
					}
					else {
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


		#region chisonQuemado
		//private static void GuardarBasesDeDatos(ParseTreeNode raiz)
		//{
		//	foreach (ParseTreeNode nodo in raiz.ChildNodes)
		//	{

		//		BaseDatos db = GetBaseDatos(nodo);
		//		Analizador.AddBaseDatos(db);
		//	}
		//}

		//private static void AddDbObj(object obj, BaseDatos db,int linea,int columna)
		//{
		//		if (obj is Tabla)
		//		{
		//			if (!db.ExisteTabla(((Tabla)obj).Nombre))
		//			{
		//				db.AgregarTabla((Tabla)obj);
		//			}
		//			else
		//			{
		//				//INSERTANDO ERROR EN TABLA ERRORS
						
		//				Analizador.ErroresChison.Add(new Error(

		//					TipoError.Semantico,
		//					"La tabla '"+((Tabla)obj).Nombre+"' ya existe en la base de datos '"+db.Nombre+"'",
		//					linea+1,
		//					columna+1,
		//					Datos.GetDate(), //fecha
		//					Datos.GetTime()//hora
		//				));

		//			}
		//		}
		//		else if (obj is UserType)
		//		{
		//			if (!db.ExisteUserType(((UserType)obj).Nombre))
		//			{
		//				db.AgregarUserType((UserType)obj);
		//			}
		//			else
		//			{
		//				//INSERTANDO ERROR EN TABLA ERRORS
		//				Analizador.ErroresChison.Add(new Error(

		//					TipoError.Semantico,
		//					"El user type '"+((UserType)obj).Nombre+"' ya existe en la base de datos '"+db.Nombre+"'",
		//					linea+1,
		//					columna+1,
		//					Datos.GetDate(), //fecha
		//					Datos.GetTime()//hora
		//				));
		//			}
		//		}
		//		else if (obj is Procedimiento)
		//		{
		//			if (!db.ExisteProcedimiento(((Procedimiento)obj).Nombre))
		//			{
		//				db.AgregarProcedimiento((Procedimiento)obj);
		//			}
		//			else
		//			{
		//				//INSERTANDO ERROR EN TABLA ERRORS
		//				Analizador.ErroresChison.Add(new Error(

		//					TipoError.Semantico,
		//					"El procedimiento '"+((Procedimiento)obj).Nombre+"' ya existe en la base de datos '"+db.Nombre+"'",
		//					linea+1,
		//					columna+1,
		//					Datos.GetDate(), //fecha
		//					Datos.GetTime()//hora
		//				));
		//			}

		//		}
		//}

		//private static List<object> GetObjetosDb(ParseTreeNode raiz)
		//{
		//	List<object> objetosdb = new List<object>();
		//	foreach (ParseTreeNode nodo in raiz.ChildNodes)
		//	{
		//		switch (nodo.Term.Name)
		//		{
		//			case "TABLA":
		//				Tabla tb = GetTabla(objetosdb,nodo);
		//				//tb.MostrarCabecera();
		//				//tb.MostrarDatos();
		//				objetosdb.Add(tb);
		//				break;
		//			case "OBJETO":
		//				UserType obj = GetObjeto(nodo);
		//				//obj.Mostrar();
		//				objetosdb.Add(obj);
		//				break;
		//			case "PROCEDIMIENTO":
		//				Procedimiento proc = GetProcedimiento(nodo);
		//				//proc.Mostrar();
		//				objetosdb.Add(proc);
		//				break;
		//		}

		//	}
		//	return objetosdb;
		//}

		//private static Procedimiento GetProcedimiento(ParseTreeNode nodo)
		//{
		//	List<Parametro> par = GetParametros(nodo.ChildNodes.ElementAt(1));
		//	List<Parametro> ret = GeRetornos(nodo.ChildNodes.ElementAt(1));
		//	String codigo = nodo.ChildNodes.ElementAt(2).Token.ValueString;
		//	codigo = codigo.TrimStart('$');
		//	codigo = codigo.TrimEnd('$');
		//	Analizador.AnalizarCql(codigo);
		//	List<Error> erroresInst = (Analizador.ErroresChison);
		//	//cambiar el numero de linea 
		//	if (erroresInst.Count>0) {
		//		//Analizador.ErroresChison.AddRange(erroresInst);
		//		codigo = "//SE ENCONTRARON ERRORES EN EL CODIGO\n";
		//	}
		//	return new Procedimiento(nodo.ChildNodes.ElementAt(0).Token.ValueString, par, ret, null,codigo,nodo.Span.Location.Line,nodo.Span.Location.Column);
		//}

		//private static List<Parametro> GeRetornos(ParseTreeNode parseTreeNode)
		//{
		//	List<Parametro> ret = new List<Parametro>();
		//	foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
		//	{
		//		if (nodo.ChildNodes.ElementAt(2).Token.ValueString.ToLower().Equals("out"))
		//		{
		//			try
		//			{
		//				TipoDatoDB t = GetTipo(nodo.ChildNodes.ElementAt(1));
		//				string nombreTipo = GetNombreTipo(t, nodo.ChildNodes.ElementAt(1),true);

		//					ret.Add(new Parametro(nodo.ChildNodes.ElementAt(0).Token.ValueString, new TipoObjetoDB(t, nombreTipo)));
		//			}
		//			catch (ArgumentException ex)
		//			{
		//				//INSERTANDO ERROR EN TABLA ERRORS
		//				Analizador.ErroresChison.Add(new Error(

		//					TipoError.Semantico,
		//					"Error grave leyendo datos en retornos del procedimiento",
		//					nodo.Span.Location.Line+1,
		//					nodo.Span.Location.Column+1,
		//					Datos.GetDate(), //fecha
		//					Datos.GetTime()//hora
		//				));
		//			}
		//		}
		//	}
		//	return ret;
		//}

		//private static List<Parametro> GetParametros(ParseTreeNode parseTreeNode)
		//{
		//	List<Parametro> param = new List<Parametro>();
		//	foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
		//	{
		//		if (nodo.ChildNodes.ElementAt(2).Token.ValueString.ToLower().Equals("in"))
		//		{
		//			try
		//			{
		//				TipoDatoDB t = GetTipo(nodo.ChildNodes.ElementAt(1));
		//				string nombreTipo = GetNombreTipo(t, nodo.ChildNodes.ElementAt(1),true);
		//				param.Add(new Parametro(nodo.ChildNodes.ElementAt(0).Token.ValueString, new TipoObjetoDB(t, nombreTipo)));
		//			}
		//			catch (ArgumentException ex)
		//			{
		//				//INSERTANDO ERROR EN TABLA ERRORS
		//				Analizador.ErroresChison.Add(new Error(

		//					TipoError.Semantico,
		//					"Error grave leyendo datos en parametros del procedimiento",
		//					nodo.Span.Location.Line+1,
		//					nodo.Span.Location.Column+1,
		//					Datos.GetDate(), //fecha
		//					Datos.GetTime()//hora
		//				));
		//			}
		//		}
		//	}
		//	return param;
		//}

		//private static UserType GetObjeto(ParseTreeNode nodo)
		//{
		//	return new UserType(nodo.ChildNodes.ElementAt(0).Token.ValueString,
		//		GetAtributos(nodo.ChildNodes.ElementAt(1)));
		//}

		//private static Dictionary<string, TipoObjetoDB> GetAtributos(ParseTreeNode parseTreeNode)
		//{
		//	Dictionary<string, TipoObjetoDB> atributos = new Dictionary<string, TipoObjetoDB>();
		//	foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
		//	{
		//		try
		//		{
		//			TipoDatoDB td = GetTipo(nodo.ChildNodes.ElementAt(1));
		//			string nombreTipo = GetNombreTipo(td,nodo.ChildNodes.ElementAt(1),true);
					
		//			atributos.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString, new TipoObjetoDB(td, nombreTipo));
		//		}
		//		catch (ArgumentException ex)
		//		{
		//			//INSERTANDO ERROR EN TABLA ERRORS
		//			Analizador.ErroresChison.Add(new Error(

		//					TipoError.Semantico,
		//					"Ya existe el atributo '"+nodo.ChildNodes.ElementAt(0).Token.ValueString+"' en el User Type",
		//					nodo.Span.Location.Line+1,
		//					nodo.Span.Location.Column+1,
		//					Datos.GetDate(), //fecha
		//					Datos.GetTime()//hora
		//				));

		//		}
		//	}
		//	return atributos;
		//}

		//public static string GetNombreTipo(TipoDatoDB td, ParseTreeNode parseTreeNode,bool b)
		//{
		//	switch (td) {
		//		case TipoDatoDB.LISTA_PRIMITIVO:
		//		case TipoDatoDB.LISTA_OBJETO:
		//			TipoDatoDB t = GetTipo(parseTreeNode.ChildNodes.ElementAt(2));
		//			string nombreTipo = GetNombreTipo(t, parseTreeNode.ChildNodes.ElementAt(2), false);
		//			if (!b) return "list<" + nombreTipo + ">";
		//			return nombreTipo;
		//		case TipoDatoDB.SET_PRIMITIVO:
		//		case TipoDatoDB.SET_OBJETO:
		//			t = GetTipo(parseTreeNode.ChildNodes.ElementAt(2));
		//			nombreTipo = GetNombreTipo(t, parseTreeNode.ChildNodes.ElementAt(2), false);
		//			if (!b) return "set<" + nombreTipo + ">";
		//			return nombreTipo;
		//		case TipoDatoDB.MAP_PRIMITIVO:
		//		case TipoDatoDB.MAP_OBJETO:
		//			t = GetTipo(parseTreeNode.ChildNodes.ElementAt(2));
		//			nombreTipo = GetNombreTipo(t, parseTreeNode.ChildNodes.ElementAt(2), false);
					
		//			TipoDatoDB t1 = GetTipo(parseTreeNode.ChildNodes.ElementAt(3));
		//			string nombreTipo1 = GetNombreTipo(t1, parseTreeNode.ChildNodes.ElementAt(3), false);
		//			if (!b) return "map<" + nombreTipo + "," + nombreTipo1 + ">";
		//			return nombreTipo + "," + nombreTipo1;
		//		case TipoDatoDB.OBJETO:
		//			if (parseTreeNode.ChildNodes.Count == 4)
		//			{
		//				t = GetTipo(parseTreeNode.ChildNodes.ElementAt(2));
		//				nombreTipo = GetNombreTipo(t, parseTreeNode.ChildNodes.ElementAt(2), false);
		//				return nombreTipo;

		//			}
		//			else {
		//				return parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString;
		//			}
		//		case TipoDatoDB.BOOLEAN:
		//			return "boolean";
		//		case TipoDatoDB.DATE:
		//			return "date";
		//		case TipoDatoDB.DOUBLE:
		//			return "double";
		//		case TipoDatoDB.INT:
		//			return "int";
		//		case TipoDatoDB.STRING:
		//			return "string";
		//		case TipoDatoDB.TIME:
		//			return "time";
		//		case TipoDatoDB.COUNTER:
		//			return "counter";
		//		case TipoDatoDB.NULO:
		//			return "nulo";
		//		default:
		//			return "";
		//	}
		//}

		//private static Tabla GetTabla(List<object> objetosdb,ParseTreeNode nodo)
		//{
		//	Tabla tb = new Tabla(nodo.ChildNodes.ElementAt(0).Token.ValueString, GetColumnas(objetosdb, nodo.ChildNodes.ElementAt(1)));
		//	  //agregando datos a la tabla
		//	AddDataTabla(tb, nodo.ChildNodes.ElementAt(2));
		//	return tb;
		//}

		//private static void AddDataTabla(Tabla tb, ParseTreeNode raiz)
		//{
		//	foreach (ParseTreeNode nodo in raiz.ChildNodes)
		//	{
		//		Dictionary<string, object> fila = GetFila(nodo);
		//		if (fila != null)
		//		{
		//			//List<Error> er =tb.Insertar(fila,nodo.Span.Location.Line,nodo.Span.Location.Column);
		//			//Analizador.ErroresChison.AddRange(er);
		//		}
		//	}
		//}

		//private static Dictionary<string, object> GetFila(ParseTreeNode fila)
		//{
		//	Dictionary<string, object> datos = new Dictionary<string, object>();
		//	try
		//	{
		//		foreach (ParseTreeNode nod in fila.ChildNodes)
		//		{
		//			if (nod.ChildNodes.ElementAt(1).Term.Name.Equals("LISTA_DATOS"))
		//			{
		//				//ES UNA LISTA
		//				//CollectionLista valores = GetListaDatos(nod.ChildNodes.ElementAt(1));
		//				//datos.Add(nod.ChildNodes.ElementAt(0).Token.ValueString, valores);
		//			}
		//			else if (nod.ChildNodes.ElementAt(1).Term.Name.Equals("LISTA_DATATABLE")) {
		//				//ES UN OBJETO
		//				Dictionary<string, object> atributos = GetFila(nod.ChildNodes.ElementAt(1));
		//				//datos.Add(nod.ChildNodes.ElementAt(0).Token.ValueString,new Objeto(atributos));
		//			}else {
		//				datos.Add(nod.ChildNodes.ElementAt(0).Token.ValueString, Datos.GetValor(nod.ChildNodes.ElementAt(1).Token.ValueString));

		//			}

		//		}
		//	}
		//	catch (ArgumentException ex)
		//	{
		//		//INSERTANDO ERROR EN TABLA ERRORS
		//		Analizador.ErroresChison.Add(new Error(
		//					TipoError.Semantico,
		//					"Error grave al leer los datos de la tabla ",
		//					fila.Span.Location.Line+1,
		//					fila.Span.Location.Column+1,
		//					Datos.GetDate(), //fecha
		//					Datos.GetTime()//hora
		//				));
		//		return null;
		//	}
		//	return datos;
		//}

		////private static CollectionLista GetListaDatos(ParseTreeNode parseTreeNode)
		////{
		////	CollectionLista valores = new CollectionLista();
		////	foreach (ParseTreeNode nod in parseTreeNode.ChildNodes)
		////	{
		////		if (nod.ChildNodes.ElementAt(0).Term.Name != "OBJETO" && nod.ChildNodes.ElementAt(0).Term.Name != "LISTA_DATOS"&& nod.ChildNodes.ElementAt(0).Term.Name != "LISTA_DATATABLE")
		////		{
		////			//DATO primitivo
		////			valores.SetItem(Datos.GetValor(nod.ChildNodes.ElementAt(0).Token.ValueString));
		////		}
		////		else if (nod.ChildNodes.ElementAt(0).Term.Name == "LISTA_DATOS") {
		////			//lista
		////			valores.SetItem(GetListaDatos(nod.ChildNodes.ElementAt(0)));
		////		}else
		////		{
		////			//objeto
		////			Dictionary<string, object> atributos = GetFila(nod.ChildNodes.ElementAt(0));
		////			valores.SetItem(new Objeto(atributos));
		////		}
		////	}

		////	return valores;
		////}

		//private static List<Columna> GetColumnas(List<object> objetosdb,ParseTreeNode raiz)
		//{
		//	List<Columna> columnas = new List<Columna>();
		//	foreach (ParseTreeNode nodo in raiz.ChildNodes)
		//	{
		//		Columna cl = GetColumna(objetosdb,nodo);
		//		if (cl!=null) {
		//			columnas.Add(cl);
		//		}
		//	}
		//	return columnas;
		//}

		//private static Columna GetColumna(List<object> objetos,ParseTreeNode nodo)
		//{
		//	TipoDatoDB tipo = GetTipo(nodo.ChildNodes.ElementAt(1));
		//	string nombreTipo = GetNombreTipo(tipo, nodo.ChildNodes.ElementAt(1),true);
		//	if (tipo==TipoDatoDB.OBJETO) {
		//		if (!ExisteUserTypeEnDb(objetos, nombreTipo))
		//		{
		//			//INSERTANDO ERROR EN TABLA ERRORS
		//			Analizador.ErroresChison.Add(new Error(

		//					TipoError.Semantico,
		//					"No existe el User Type '"+nombreTipo+"' para crear el objeto",
		//					nodo.Span.Location.Line+1,
		//					nodo.Span.Location.Column+1,
		//					Datos.GetDate(), //fecha
		//					Datos.GetTime()//hora
		//				));

		//			return null;
		//		}
		//	}
		//		return new Columna(nodo.ChildNodes.ElementAt(0).Token.ValueString,
		//			new TipoObjetoDB(tipo, nombreTipo),
		//			IsPk(nodo.ChildNodes.ElementAt(2)));
		//	}

		//private static bool ExisteUserTypeEnDb(List<object> objetos, string v)
		//{
		//	foreach (object ob in objetos)
		//	{
		//		if (ob.GetType() == typeof(UserType))
		//		{
		//			if (((UserType)ob).Nombre.Equals(v))
		//			{
		//				return true;
		//			}
		//		}
		//	}
		//	return false;
		//}

		//private static bool IsPk(ParseTreeNode parseTreeNode)
		//{
		//	if (bool.TryParse(parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString, out bool res))
		//	{
		//		return res;
		//	}
		//	return false;
		//}

		//public static TipoDatoDB GetTipo(ParseTreeNode parseTreeNode)
		//{
		//	switch (parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower())
		//	{
		//		case "string":
		//			return TipoDatoDB.STRING;
		//		case "int":
		//			return TipoDatoDB.INT;
		//		case "double":
		//			return TipoDatoDB.DOUBLE;
		//		case "boolean":
		//			return TipoDatoDB.BOOLEAN;
		//		case "date":
		//			return TipoDatoDB.DATE;
		//		case "time":
		//			return TipoDatoDB.TIME;
		//		case "counter":
		//			return TipoDatoDB.COUNTER;
		//		//listas
		//		case "list<string>":
		//		case "list<int>":
		//		case "list<double>":
		//		case "list<boolean>":
		//		case "list<date>":
		//		case "list<time>":
		//			return TipoDatoDB.LISTA_PRIMITIVO;
		//		//sets
		//		case "set<string>":
		//		case "set<int>":
		//		case "set<double>":
		//		case "set<boolean>":
		//		case "set<date>":
		//		case "set<time>":
		//			return TipoDatoDB.SET_PRIMITIVO;
		//		//maps
		//		case "map<string>":
		//		case "map<int>":
		//		case "map<double>":
		//		case "map<boolean>":
		//		case "map<date>":
		//		case "map<time>":
		//			return TipoDatoDB.MAP_PRIMITIVO;
		//		default:
		//			if (parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Equals("list")||
		//				parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower().StartsWith("list"))
		//			{
		//				return TipoDatoDB.LISTA_OBJETO;
		//			}
		//			else if (parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Equals("set")||
		//				parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower().StartsWith("set"))
		//			{
		//				return TipoDatoDB.SET_OBJETO;
		//			}
		//			else if (parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Equals("map")||
		//				parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower().StartsWith("map"))
		//			{
		//				return TipoDatoDB.MAP_OBJETO;
		//			}

		//			return TipoDatoDB.OBJETO;
		//	}
		//}

		//private static List<string> GetListaOtroNombre(ParseTreeNode lista_nombres)
		//{
		//	List<string> nombres = new List<string>();
		//	foreach (ParseTreeNode nodo in lista_nombres.ChildNodes)
		//	{
		//		nombres.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString);
		//	}
		//	return nombres;
		//}
	}
	#endregion
}
