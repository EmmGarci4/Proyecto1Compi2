using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Ast;
using com.Analisis.Util;
using Proyecto1Compi2.com.AST;
using Proyecto1Compi2.com.Analisis;
using Proyecto1Compi2.com.db;
using System.Text.RegularExpressions;
using Proyecto1Compi2.com.Util;

namespace com.Analisis
{
	static class Analizador
	{
		private const string path = "C:\\Users\\Emely\\Documents\\Visual Studio 2017\\Projects\\Proyecto1Compi2\\Proyecto1Compi2\\bin\\Debug\\data\\";
		private static List<Error> erroresCQL= new List<Error>();
		private static List<Usuario> usuariosdb = new List<Usuario>();
		private static List<BaseDatos> dbs = new List<BaseDatos>();
		private static NodoAST ast = null;
		private static ParseTreeNode raiz;
		static Tabla errors = new Tabla("errors", new List<Columna> {
				new Columna("Numero",new TipoObjetoDB(TipoDatoDB.COUNTER,""),true),
				new Columna("Tipo",new TipoObjetoDB(TipoDatoDB.STRING,""),false),
				new Columna("Descripcion",new TipoObjetoDB(TipoDatoDB.STRING,""),false),
				new Columna("Fila",new TipoObjetoDB(TipoDatoDB.INT,""),false),
				new Columna("Columna",new TipoObjetoDB(TipoDatoDB.INT,""),false),
				new Columna("Fecha",new TipoObjetoDB(TipoDatoDB.DATE,""),false),
				new Columna("Hora",new TipoObjetoDB(TipoDatoDB.TIME,""),false),
			}); 

		internal static void AddBaseDatos(BaseDatos db)
		{
			if (!ExisteDB(db.Nombre))
			{
				BasesDeDatos.Add(db);
			}
			else
			{
				Console.WriteLine("ERROR YA EXISTE LA BASE DE DATOS");
			}
		}
		

		public static bool AnalizarCql(String texto){
			GramaticaCql gramatica = new GramaticaCql();
			LanguageData ldata = new LanguageData(gramatica);
			Parser parser = new Parser(ldata);
			ParseTree arbol = parser.Parse(texto);
			Analizador.ErroresCQL.Clear();
			Analizador.raiz = arbol.Root;
			if (raiz!=null) {
				//Analizador.ast = GeneradorAstSql.GetAST(arbol.Root);
				//Expresion ex = (Expresion)Analizador.ast;
				//if (ex.GetValor(new TablaSimbolos(0, "global"))!=null) {
				//	Console.WriteLine("Valor:" + ex.GetValor(new TablaSimbolos(0, "global"))+" Tipo:"+ ex.GetTipo(new TablaSimbolos(0, "global")));
				//}
			}
			foreach (Irony.LogMessage mensaje in arbol.ParserMessages)
			{
				ErroresCQL.Add(new Error(TipoError.Lexico, mensaje.Message, mensaje.Location.Line,mensaje.Location.Column));
			}

			return Analizador.raiz != null;
		}

		public static bool AnalizarChison(String texto)
		{
			GramaticaChison gramatica = new GramaticaChison();
			LanguageData ldata = new LanguageData(gramatica);
			Parser parser = new Parser(ldata);
			//IMPORTAR 
			texto = Importar(texto);
			ParseTree arbol = parser.Parse(texto);
			Analizador.Errors.Truncar();
			Analizador.raiz = arbol.Root;
			
			if (raiz != null)
			{
				GeneradorDB.GuardarInformación(raiz);
				foreach (BaseDatos db in Analizador.BasesDeDatos)
				{
					db.MostrarBaseDatos();
				}
				foreach (Usuario us in Analizador.Usuariosdb)
				{
					us.Mostrar();
				}
			}
			foreach (Irony.LogMessage mensaje in arbol.ParserMessages)
			{

				//INSERTANDO ERROR EN TABLA ERRORS
				Errors.Insertar(new List<object>
				{
					"Léxico",
					mensaje.Message,
					mensaje.Location.Line,
					mensaje.Location.Column,
					HandlerFiles.getDate(), //fecha
					HandlerFiles.getTime()//hora
				});
			}
			Errors.MostrarCabecera();
			Errors.MostrarDatos();
			return Analizador.raiz != null;
		}

		internal static void GenerarArchivos(string v)
		{
			StringBuilder cadena = new StringBuilder();
			cadena.Append("$<\n\"DATABASES\"=[");
			IEnumerator<BaseDatos> enumerator = BasesDeDatos.GetEnumerator();
			bool hasNext = enumerator.MoveNext();
			while (hasNext)
			{
				BaseDatos i = enumerator.Current;
				cadena.Append(i.ToString());
				hasNext = enumerator.MoveNext();
				if (hasNext)
				{
					cadena.Append(",");
				}
			}
			enumerator.Dispose();
			cadena.Append("],\n");
			cadena.Append("\"USERS\"=[");
			IEnumerator<Usuario> enumerator2 = Usuariosdb.GetEnumerator();
			bool hasNext2 = enumerator2.MoveNext();
			while (hasNext2)
			{
				Usuario i = enumerator2.Current;
				cadena.Append(i.ToString());
				hasNext2 = enumerator2.MoveNext();
				if (hasNext2)
				{
					cadena.Append(",");
				}
			}
			enumerator2.Dispose();
			cadena.Append("]\n");
			cadena.Append(">$");
			HandlerFiles.guardarArchivo(cadena.ToString(), "MiPrincipal.chison");
		}

		internal static void Clear()
		{
			ErroresCQL.Clear();
			Errors.Truncar();
			Usuariosdb.Clear();
			BasesDeDatos.Clear();
			ast = null;
			raiz = null;
			Console.WriteLine("*************************************************************************");
		}

		private static string Importar(string texto)
		{
			foreach (Match match in Regex.Matches(texto, "\\${.*}\\$", RegexOptions.IgnoreCase))
			{
				String t1 = HandlerFiles.AbrirArchivo(GetURL(match.Value));
				if (t1 != null)
				{
					texto = texto.Replace(match.Value, t1);
				}
				else
				{
					texto = texto.Replace(match.Value, String.Empty);
					Console.Error.WriteLine("ERROR EL ARCHIVO NO EXISTE");
				}
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
			value = PATH + value;
			return value;
		}

		public static bool AnalizarLup(String texto)
		{
			GramaticaLup gramatica = new GramaticaLup();
			LanguageData ldata = new LanguageData(gramatica);
			Parser parser = new Parser(ldata);
			ParseTree arbol = parser.Parse(texto);
			Analizador.ErroresCQL.Clear();
			Analizador.raiz = arbol.Root;
			if (raiz != null)
			{

				//if (ex.GetValor(new TablaSimbolos(0, "global"))!=null) {
				//	Console.WriteLine("Valor:" + ex.GetValor(new TablaSimbolos(0, "global"))+" Tipo:"+ ex.GetTipo(new TablaSimbolos(0, "global")));
				//}
			}
			foreach (Irony.LogMessage mensaje in arbol.ParserMessages)
			{
				ErroresCQL.Add(new Error(TipoError.Lexico, mensaje.Message, mensaje.Location.Line, mensaje.Location.Column));
			}

			return Analizador.raiz != null;
		}

		public static bool ExisteUsuario(string nombre)
		{
			foreach (Usuario db in usuariosdb)
			{
				if (db.Nombre.Equals(nombre))
				{
					return true;
				}
			}
			return false;
		}

		public static bool ExisteDB(string nombre) {
			foreach (BaseDatos db in dbs) {
				if (db.Nombre.Equals(nombre)) {
					return true;
				}
			}
			return false;
		}

		public static NodoAST AST { get => ast; }
		public static ParseTreeNode Raiz { get => raiz; set => raiz = value; }
		internal static List<Usuario> Usuariosdb { get => usuariosdb; set => usuariosdb = value; }
		public static List<Error> ErroresCQL { get => erroresCQL; set => erroresCQL = value; }
		internal static List<BaseDatos> BasesDeDatos { get => dbs; set => dbs = value; }

		public static string PATH => path;

		internal static Tabla Errors { get => errors; set => errors = value; }

		internal static void AddUsuario(Usuario usu)
		{
			if (!ExisteUsuario(usu.Nombre))
			{
				Usuariosdb.Add(usu);
			}
			else
			{
				Console.WriteLine("EL USUARIO YA EXISTE");
			}
		}

	}
}
