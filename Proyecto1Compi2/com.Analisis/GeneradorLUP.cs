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
				respuesta.Append("[+DESC]");
				respuesta.Append(error.Mensaje);
				respuesta.Append("[-DESC]");
				respuesta.Append("[-ERROR]");
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
			string usuario = nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Replace(" ",string.Empty);
			//RECORRER BASE DE DATOS
			respuesta.AppendLine("[+DATABASES]");
			Usuario usu = Analizador.BuscarUsuario(usuario);
			if (usu!=null) {
				foreach (string str in usu.Permisos) {
					BaseDatos db = Analizador.BuscarDB(str);
					if (db != null) {
						respuesta.AppendLine("[+DATABASE]");
						respuesta.AppendLine("[+NAME]");
						respuesta.AppendLine(db.Nombre);
						respuesta.AppendLine("[-NAME]");
						//TABLAS
						respuesta.AppendLine("[+TABLES]");
						foreach (Tabla tb in db.Tablas) {
							respuesta.AppendLine("[+TABLE]");
							respuesta.AppendLine("[+NAME]");
							respuesta.AppendLine(tb.Nombre);
							respuesta.AppendLine("[-NAME]");
							respuesta.AppendLine("[+COLUMNS]");
							foreach (Columna col in tb.Columnas) {
								respuesta.AppendLine( col.Nombre+":"+col.Tipo.ToString());
							}
							respuesta.AppendLine("[-COLUMNS]");
							respuesta.AppendLine("[-TABLE]");
						}
						respuesta.AppendLine("[-TABLES]");
						//USERTYPES
						respuesta.AppendLine("[+TYPES]");
						foreach (UserType ut in db.UserTypes) {
							respuesta.AppendLine("[+TYPE]");
							respuesta.AppendLine("[+NAME]");
							respuesta.AppendLine(ut.Nombre);
							respuesta.AppendLine("[-NAME]");
							respuesta.AppendLine("[+ATRIBUTES]");
							foreach (KeyValuePair<string,TipoObjetoDB> par in ut.Atributos) {
								respuesta.AppendLine(par.Key+":"+par.Value.ToString());
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
								respuesta.AppendLine("" + par.Nombre + ":" + par.Tipo.ToString());
							}
							respuesta.AppendLine("[-PARAMETERS]");
							respuesta.AppendLine("[+RETURNS]");
							foreach (Parametro par in ut.Retornos)
							{
								respuesta.AppendLine("" + par.Nombre + ":" + par.Tipo.ToString());
							}
							respuesta.AppendLine("[-RETURNS]");
							respuesta.AppendLine("[-PROCEDURE]");
						}
						respuesta.AppendLine("[-PROCEDURES]");
						respuesta.AppendLine("[-DATABASE]");
					}
				}
			}
			respuesta.AppendLine("[-DATABASES]");
		}

		private static void Consulta(ParseTreeNode nodo)
		{
			string usuario = nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Replace(" ", string.Empty);
			string codigocql = nodo.ChildNodes.ElementAt(1).Token.ValueString.ToLower();

			if (Analizador.ExisteUsuario(usuario))
			{
				Sesion sesion = new Sesion(usuario, null);
				//analizar 
				bool res = Analizador.AnalizarCql(codigocql, sesion);
				//paquete de resultados
				foreach (ResultadoConsulta resultado in Analizador.ResultadosConsultas)
				{
					respuesta.Append("[+DATA]");
					respuesta.Append(resultado.ToString());
					respuesta.Append("[-DATA]");
				}
				//mensajes
				foreach (String mensaje in sesion.Mensajes)
				{
					respuesta.Append("[+MESSAGE]");
					respuesta.Append(mensaje);
					respuesta.Append("[-MESSAGE]");
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
					respuesta.Append("[+DESC]");
					respuesta.Append(error.Mensaje);
					respuesta.Append("[-DESC]");
					respuesta.Append("[-ERROR]");
				}
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
				respuesta.Append("[+DESC]");
				respuesta.Append("el usuario '"+usuario+"' no existe");
				respuesta.Append("[-DESC]");
				respuesta.Append("[-ERROR]");
			}
		}

		private static void Logout(ParseTreeNode nodo)
		{
			string usuario = nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Replace(" ", string.Empty);
			bool r = true;
			respuesta.Append("[+LOGOUT]");
			if (r)
			{
				respuesta.Append("[SUCCESS]");
			}
			else
			{
				respuesta.Append("[FAIL]");
			}
			respuesta.Append("[-LOGOUT]");
		}

		private static void Login(ParseTreeNode nodo)
		{
			//0 = usuario
			//1 = password
			string usuario = nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Replace(" ",string.Empty);
			string passwd = nodo.ChildNodes.ElementAt(1).Token.ValueString.ToLower().Replace(" ", string.Empty);
			bool r=Analizador.IniciarSesion(usuario,passwd);
			respuesta.Append("[+LOGIN]");
			if (r)
			{
				respuesta.Append("SUCCESS");
			}
			else {
				respuesta.Append("[FAIL]");
			}
			respuesta.Append("[-LOGIN]");
		}
	}
}
