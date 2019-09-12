using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis;
using Irony.Parsing;
using Proyecto1Compi2.com.AST;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.Analisis
{
	class GeneradorLup
	{
		private static StringBuilder respuesta = new StringBuilder();

		public static StringBuilder Respuesta { get => respuesta; set => respuesta = value; }

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
						respuesta.AppendLine("\t"+db.Nombre);
						respuesta.AppendLine("[-NAME]");
						//TABLAS
						respuesta.AppendLine("[+TABLES]");
						foreach (Tabla tb in db.Tablas) {
							respuesta.AppendLine("\t[+TABLE]");
							respuesta.AppendLine("\t\t[+NAME]");
							respuesta.AppendLine("\t\t\t"+tb.Nombre);
							respuesta.AppendLine("\t\t[-NAME]");
							respuesta.AppendLine("\t\t[+COLUMNS]");
							foreach (Columna col in tb.Columnas) {
								respuesta.AppendLine("\t\t\t" + col.Nombre+":"+col.Tipo.ToString());
							}
							respuesta.AppendLine("\t\t[-COLUMNS]");
							respuesta.AppendLine("\t[-TABLE]");
						}
						respuesta.AppendLine("[-TABLES]");
						//USERTYPES
						respuesta.AppendLine("[+TYPES]");
						foreach (UserType ut in db.UserTypes) {
							respuesta.AppendLine("\t[+TYPE]");
							respuesta.AppendLine("\t\t[+NAME]");
							respuesta.AppendLine("\t\t\t"+ut.Nombre);
							respuesta.AppendLine("\t\t[-NAME]");
							respuesta.AppendLine("\t\t[+ATRIBUTES]");
							foreach (KeyValuePair<string,TipoObjetoDB> par in ut.Atributos) {
								respuesta.AppendLine("\t\t\t"+par.Key+":"+par.Value.ToString());
							}
							respuesta.AppendLine("\t\t[-ATRIBUTES]");
							respuesta.AppendLine("\t[-TYPE]");
						}
						respuesta.AppendLine("[-TYPES]");
						//PROCEDURES
						respuesta.AppendLine("[+PROCEDURES]");
						foreach (Procedimiento ut in db.Procedimientos)
						{
							respuesta.AppendLine("\t[+PROCEDURE]");
							respuesta.AppendLine("\t\t[+NAME]");
							respuesta.AppendLine("\t\t\t" + ut.Nombre);
							respuesta.AppendLine("\t\t[-NAME]");
							respuesta.AppendLine("\t\t[+PARAMETERS]");
							foreach (Parametro par in ut.Parametros)
							{
								respuesta.AppendLine("\t\t\t" + par.Nombre + ":" + par.Tipo.ToString());
							}
							respuesta.AppendLine("\t\t[-PARAMETERS]");
							respuesta.AppendLine("\t\t[+RETURNS]");
							foreach (Parametro par in ut.Retornos)
							{
								respuesta.AppendLine("\t\t\t" + par.Nombre + ":" + par.Tipo.ToString());
							}
							respuesta.AppendLine("\t\t[-RETURNS]");
							respuesta.AppendLine("\t[-PROCEDURE]");
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

			if (Analizador.ExisteUsuario(usuario)) {
				Sesion sesion = new Sesion(usuario, "");
				//analizar 
				bool res =Analizador.AnalizarCql(codigocql, sesion);
				if (res)
				{
					//paquete de resultados
					respuesta.Append("[+DATA]");
					//INSERTAR RESULTADOS
					respuesta.Append("[-DATA]");
					respuesta.Append("[+MESSAGE]");
					respuesta.Append("Todo salió bien :D");
					respuesta.Append("[-MESSAGE]");

				}
				else {
					//paquete de error 
				}
				
			}
		}

		private static void Logout(ParseTreeNode nodo)
		{
			string usuario = nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Replace(" ", string.Empty);
			bool r = true;
			respuesta.Append("[+LOGOUT]\n");
			if (r)
			{
				respuesta.Append("[SUCCESS]\n");
			}
			else
			{
				respuesta.Append("[FAIL]\n");
			}
			respuesta.Append("[-LOGOUT]\n");
		}

		private static void Login(ParseTreeNode nodo)
		{
			//0 = usuario
			//1 = password
			string usuario = nodo.ChildNodes.ElementAt(0).Token.ValueString.ToLower().Replace(" ",string.Empty);
			string passwd = nodo.ChildNodes.ElementAt(1).Token.ValueString.ToLower().Replace(" ", string.Empty);
			bool r=Analizador.IniciarSesion(usuario,passwd);
			respuesta.Append("[+LOGIN]\n");
			if (r)
			{
				respuesta.Append("[SUCCESS]\n");
			}
			else {
				respuesta.Append("[FAIL]\n");
			}
			respuesta.Append("[-LOGIN]\n");
		}
	}
}
