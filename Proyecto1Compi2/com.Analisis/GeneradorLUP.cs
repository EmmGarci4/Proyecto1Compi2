

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis;
using com.Analisis.Util;
using Irony.Parsing;
using Proyecto1Compi2.com.AST;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.Analisis
{
	class GeneradorLup
	{
		private static StringBuilder respuesta = new StringBuilder();
		private static List<Error> erroresLup = new List<Error>();

		public static bool Analizar(String texto)
		{
			GramaticaLup gramatica = new GramaticaLup();
			LanguageData ldata = new LanguageData(gramatica);
			Parser parser = new Parser(ldata);
			ParseTree arbol = parser.Parse(texto);
			erroresLup.Clear();
			ParseTreeNode raiz = arbol.Root;
			if (raiz != null)
			{
				//generadorDOT.GenerarDOT(Analizador.Raiz, "C:\\Users\\Emely\\Desktop\\LUP.dot");
				AnalizarEntrada(raiz);
			}
			foreach (Irony.LogMessage mensaje in arbol.ParserMessages)
			{
				erroresLup.Add(new Error(TipoError.Lexico, mensaje.Message, mensaje.Location.Line, mensaje.Location.Column));
			}

			foreach (Error error in ErroresLup)
			{
				respuesta.Append("[+ERROR]");
				respuesta.Append("[+LINE]");
				respuesta.Append(error.Linea);
				respuesta.Append("[-LINE]");
				respuesta.Append("[+COLUMN]");
				respuesta.Append(error.Columna);
				respuesta.Append("[-COLUMN]");
				respuesta.Append("[+TYPE]");
				respuesta.Append(error.Tipo);
				respuesta.Append("[-TYPE]");
				respuesta.Append("[+DESC]½");
				respuesta.Append(error.Mensaje);
				respuesta.Append("½[-DESC]");
				respuesta.Append("[-ERROR]\n");
			}

			return raiz != null;
		}


		public static StringBuilder Resultado { get => respuesta; set => respuesta = value; }
		public static List<Error> ErroresLup { get => erroresLup; set => erroresLup = value; }

		internal static void AnalizarEntrada(ParseTreeNode raiz)
		{
			respuesta.Clear();
			if (raiz.ChildNodes.Count>0) {
				ParseTreeNode nodo = raiz.ChildNodes.ElementAt(0);
				switch (nodo.ChildNodes.ElementAt(0).Term.Name) {
					case "LOGIN":
						Login(nodo.ChildNodes.ElementAt(0));
						break;
					case "LOGOUT":
						Logout(nodo.ChildNodes.ElementAt(0));
						break;
					case "CONSULTA":
						Consulta(nodo.ChildNodes.ElementAt(0));
						break;
					case "STRUCT":
						Struct(nodo.ChildNodes.ElementAt(0));
						break;
					default:
						erroresLup.Add(new Error(TipoError.Semantico,"El paquete no fue reconocido",raiz.Span.Location.Line,
							raiz.Span.Location.Column));
						break;

				}
			}
		}

		private static void Struct(ParseTreeNode nodo)
		{
			string usuario = nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Replace(" ", string.Empty);
			//RECORRER BASE DE DATOS
			respuesta.AppendLine("[+DATABASES]");
			Usuario usu = Analizador.BuscarUsuario(usuario);
			if (usu != null)
			{
				foreach (string str in usu.Permisos)
				{
					BaseDatos db = Analizador.BuscarDB(str);
					if (db != null)
					{
						respuesta.AppendLine("[+DATABASE]");
						respuesta.AppendLine("[+NAME]");
						respuesta.AppendLine(db.Nombre);
						respuesta.AppendLine("[-NAME]");
						//TABLAS
						respuesta.AppendLine("[+TABLES]");
						foreach (Tabla tb in db.Tablas)
						{
							respuesta.AppendLine("[+TABLE]");
							respuesta.AppendLine("[+NAME]");
							respuesta.AppendLine(tb.Nombre);
							respuesta.AppendLine("[-NAME]");
							respuesta.AppendLine("[+COLUMNS]");
							foreach (Columna col in tb.Columnas)
							{
								respuesta.AppendLine("[+COLUMN]");
								respuesta.AppendLine("[+NAME]");
								respuesta.AppendLine(col.Nombre);
								respuesta.AppendLine("[-NAME]");
								respuesta.Append("[+TYPE]");
								respuesta.Append(col.Tipo.ToString());
								if (col.IsPrimary)
								{
									respuesta.Append("_llave_primaria");
								}
								respuesta.AppendLine("[-TYPE]");
								respuesta.AppendLine("[-COLUMN]");
							}
							respuesta.AppendLine("[-COLUMNS]");
							respuesta.AppendLine("[-TABLE]");
						}
						respuesta.AppendLine("[-TABLES]");
						//USERTYPES
						respuesta.AppendLine("[+TYPES]");
						foreach (UserType ut in db.UserTypes)
						{
							respuesta.AppendLine("[+TYPE]");
							respuesta.AppendLine("[+NAME]");
							respuesta.AppendLine(ut.Nombre);
							respuesta.AppendLine("[-NAME]");
							respuesta.AppendLine("[+ATRIBUTES]");
							foreach (KeyValuePair<string, TipoObjetoDB> par in ut.Atributos)
							{
								respuesta.AppendLine("[+ATRIBUTE]");
								respuesta.AppendLine("[+NAME]");
								respuesta.AppendLine(par.Key);
								respuesta.AppendLine("[-NAME]");
								respuesta.AppendLine("[+TYPE]");
								respuesta.AppendLine(par.Value.ToString());
								respuesta.AppendLine("[-TYPE]");
								respuesta.AppendLine("[-ATRIBUTE]");

							}
							respuesta.AppendLine("[-ATRIBUTES]");
							respuesta.AppendLine("[-TYPE]");
						}
						respuesta.AppendLine("[-TYPES]");
						//PROCEDURES
						respuesta.AppendLine("[+PROCEDURES]");
						foreach (Procedimiento ut in db.Procedimientos)
						{
							respuesta.AppendLine("[+PROCEDURE]");
							respuesta.AppendLine("[+NAME]");
							respuesta.AppendLine("" + ut.Nombre);
							respuesta.AppendLine("[-NAME]");
							respuesta.AppendLine("[+PARAMETERS]");
							foreach (Parametro par in ut.Parametros)
							{
								respuesta.AppendLine("[+PARAMETER]");
								respuesta.AppendLine("[+NAME]");
								respuesta.AppendLine(par.Nombre);
								respuesta.AppendLine("[-NAME]");
								respuesta.AppendLine("[+TYPE]");
								respuesta.AppendLine(par.Tipo.ToString());
								respuesta.AppendLine("[-TYPE]");
								respuesta.AppendLine("[-PARAMETER]");
							}
							respuesta.AppendLine("[-PARAMETERS]");
							respuesta.AppendLine("[+RETURNS]");
							foreach (Parametro par in ut.Retornos)
							{
								respuesta.AppendLine("[+RETURN]");
								respuesta.AppendLine("[+NAME]");
								respuesta.AppendLine(par.Nombre);
								respuesta.AppendLine("[-NAME]");
								respuesta.AppendLine("[+TYPE]");
								respuesta.AppendLine(par.Tipo.ToString());
								respuesta.AppendLine("[-TYPE]");
								respuesta.AppendLine("[-RETURN]");
							}
							respuesta.AppendLine("[-RETURNS]");
							respuesta.AppendLine("[-PROCEDURE]");
						}
						respuesta.AppendLine("[-PROCEDURES]");
						respuesta.AppendLine("[-DATABASE]");
					}
				}
			}
			else {
				//respuesta.Clear();
				//respuesta.Append("[+ERROR]");
				//respuesta.Append("[+LINE]");
				//respuesta.Append(1);
				//respuesta.Append("[-LINE]");
				//respuesta.Append("[+COLUMN]");
				//respuesta.Append(1);
				//respuesta.Append("[-COLUMN]");
				//respuesta.Append("[+TYPE]");
				//respuesta.Append("Semantico");
				//respuesta.Append("[-TYPE]");
				//respuesta.Append("[+DESC]½");
				//respuesta.Append("No existe el usuario");
				//respuesta.Append("½[-DESC]");
				//respuesta.Append("[-ERROR]\n");
			}
			respuesta.AppendLine("[-DATABASES]");
		}

		private static void Consulta(ParseTreeNode nodo)
		{
			string usuario = nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Replace(" ", string.Empty);
			string codigocql = nodo.ChildNodes.ElementAt(1).Token.ValueString;
			codigocql = codigocql.Replace("DATA]", string.Empty);
			codigocql = codigocql.Replace("[-DATA", string.Empty);
			

			if (Analizador.ExisteUsuario(usuario))
			{
				Sesion sesion = new Sesion(usuario, null);
				//analizar 
				try
				{
					Analizador.AnalizarCql(codigocql, sesion);
				}
				catch (StackOverflowException ex) {
					Analizador.ErroresCQL.Add(new Error(TipoError.Advertencia,"Se ha producido un desbordamiento de pila",0,0));
				}
				//paquete de resultados
				foreach (string resultado in Analizador.ResultadosConsultas)
				{
					respuesta.Append("[+DATA]½");
					respuesta.Append(resultado);
					respuesta.Append("½[-DATA]\n");
				}
				//mensajes
				foreach (String mensaje in sesion.Mensajes)
				{
					respuesta.Append("[+MESSAGE]½");
					respuesta.Append(mensaje);
					respuesta.Append("½[-MESSAGE]\n");
				}
				//errores lup
				foreach (Error error in Analizador.ErroresCQL)
				{
					respuesta.Append("[+ERROR]");
					respuesta.Append("[+LINE]");
					respuesta.Append(error.Linea);
					respuesta.Append("[-LINE]");
					respuesta.Append("[+COLUMN]");
					respuesta.Append(error.Columna);
					respuesta.Append("[-COLUMN]");
					respuesta.Append("[+TYPE]");
					respuesta.Append(error.Tipo);
					respuesta.Append("[-TYPE]");
					respuesta.Append("[+DESC]½");
					respuesta.Append(error.Mensaje);
					respuesta.Append("½[-DESC]");
					respuesta.Append("[-ERROR]\n");
				}
				Analizador.LiberarDB(sesion);
			}
			else {
				//el usuario es incorrecto
				respuesta.Append("[+ERROR]");
				respuesta.Append("[+LINE]");
				respuesta.Append(nodo.ChildNodes.ElementAt(1).Token.Location.Line);
				respuesta.Append("[-LINE]");
				respuesta.Append("[+COLUMN]");
				respuesta.Append(nodo.ChildNodes.ElementAt(1).Token.Location.Column);
				respuesta.Append("[-COLUMN]");
				respuesta.Append("[+TYPE]");
				respuesta.Append(TipoError.Semantico);
				respuesta.Append("[-TYPE]");
				respuesta.Append("[+DESC]½");
				respuesta.Append("el usuario '"+usuario+"' no existe");
				respuesta.Append("½[-DESC]");
				respuesta.Append("[-ERROR]\n");
			}
		}

		private static void Logout(ParseTreeNode nodo)
		{
			string usuario = nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Replace(" ", string.Empty);
			bool r = true;
			respuesta.Append("[+LOGOUT]");
			if (r)
			{
				respuesta.Append("SUCCESS");
			}
			else
			{
				respuesta.Append("FAIL");
			}
			respuesta.Append("[-LOGOUT]");
		}

		private static void Login(ParseTreeNode nodo)
		{
			//0 = usuario
			//1 = password
			string usuario = nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Replace(" ",string.Empty);
			string passwd = nodo.ChildNodes.ElementAt(1).Token.ValueString;
			passwd = passwd.Replace("PASS]", string.Empty);
			passwd = passwd.Replace("[-PASS", string.Empty);
			passwd = passwd.Replace(" ",string.Empty);
			bool r=Analizador.IniciarSesion(usuario,passwd);
			respuesta.Append("[+LOGIN]");
			if (r)
			{
				respuesta.Append("SUCCESS");
			}
			else {
				respuesta.Append("FAIL");
			}
			respuesta.Append("[-LOGIN]");
		}
	}
}
