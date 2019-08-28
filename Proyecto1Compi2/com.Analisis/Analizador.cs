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
		private static List<BaseDatos> BasesDeDatos = new List<BaseDatos>();
		private static List<Usuario> Usuariosdb = new List<Usuario>();
		private static List<Funcion> funciones = new List<Funcion>();
		private static List<Error> erroresCQL = new List<Error>();
		private static NodoAST ast = null;
		static List<Error> errors = new List<Error>();

		internal static bool IniciarSesion(string usuario, string passwd)
		{
			if (ExisteUsuario(usuario)) {
				Usuario us = BuscarUsuario(usuario);
				return us.Password.Equals(passwd);
			}
			return false;
		}

		static private ParseTreeNode raiz;
		static Sesion sesion;
		static string codigoAnalizado;
		static UserType errorCatch = GetErrorCatch();
		private static List<Sentencia> sentenciasMain= new List<Sentencia>();

		private static UserType GetErrorCatch()
		{
			Dictionary<string, TipoObjetoDB> at = new Dictionary<string, TipoObjetoDB>();
			at.Add("message", new TipoObjetoDB(TipoDatoDB.STRING,"string"));
			return new UserType("error",at);
		}

		internal static bool ExisteFuncion(string nombre)
		{
			if (nombre.ToLower().Equals("today")|| nombre.ToLower().Equals("now")) {
				return true;
			}
			foreach (Funcion fun in funciones) {
				if (fun.GetLlave()==nombre) {
					return true;
				}
			}
			return false;
		}

		internal static Funcion BuscarFuncion(string llave)
		{
			foreach (Funcion fun in funciones)
			{
				if (fun.GetLlave() == llave)
				{
					return fun;
				}
			}
			return null;
		}

		internal static void ElminarPermisoDeUsuario(string nombre)
		{
			foreach (Usuario us in Usuariosdb) {
				if (us.ExistePermiso(nombre)) {
					us.Permisos.Remove(nombre);
				}
			}
		}

		internal static Usuario BuscarUsuario(string usuario)
		{
			foreach (Usuario usu in Usuariosdb) {
				if (usu.Nombre.Equals(usuario)) {
					return usu;
				}
			}
			return null;
		}

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
			codigoAnalizado = texto;
			GramaticaCql gramatica = new GramaticaCql();
			LanguageData ldata = new LanguageData(gramatica);
			Parser parser = new Parser(ldata);
			ParseTree arbol = parser.Parse(texto);
			Analizador.ErroresCQL.Clear();
			Analizador.funciones.Clear();
			Analizador.raiz = arbol.Root;
			if (raiz!=null) {
				//PRUEBAS DE EXPRESIONES
				generadorDOT.GenerarDOT(Analizador.Raiz, "C:\\Users\\Emely\\Desktop\\CQL.dot");
				//Expresion ex = GeneradorAstCql.GetAST(arbol.Root);
				//funciones.Add(new Funcion("llamada",new TipoObjetoDB(TipoDatoDB.STRING,"string"),1,1));
				//TablaSimbolos ts = new TablaSimbolos("Global");

				//object respuesta = ex.GetValor(ts);
				//if (respuesta.GetType() == typeof(ThrowError)) {
				//	erroresCQL.Add(new Error((ThrowError)respuesta));
				//}
				//else {
				//	Console.WriteLine(respuesta.ToString());
				//}

				List<Sentencia> sentencias = GeneradorAstCql.GetAST(arbol.Root);
				sentenciasMain = sentencias;
				if (Analizador.ErroresCQL.Count == 0)
				{
					Analizador.AddUsuario(new Usuario("admin", "admin"));
					sesion = new Sesion("admin", null);
					TablaSimbolos ts = new TablaSimbolos();
					foreach (Sentencia sentencia in sentencias)
					{
						object respuesta = sentencia.Ejecutar(ts);
						if (respuesta != null)
						{
							if (respuesta.GetType() == typeof(ThrowError))
							{
								ErroresCQL.Add(new Error((ThrowError)respuesta));
							}
							else if (respuesta.GetType() == typeof(List<ThrowError>))
							{
								//AGREGANDO ERRORES A LISTA PRINCIPAL
								List<ThrowError> errores = (List<ThrowError>)respuesta;
								foreach (ThrowError error in errores)
								{
									ErroresCQL.Add(new Error(error));
								}
							}
							else if (respuesta.GetType() == typeof(Throw))
							{
								ErroresCQL.Add(new Error(TipoError.Semantico,
									"La sentencia throw de tipo "+((Throw)respuesta).NombreExeption+" no está contenida en una estructura try-catch",
									((Throw)respuesta).Linea, ((Throw)respuesta).Columna));
								break;
							}
							else if(respuesta.GetType() == typeof(Break)|| respuesta.GetType() == typeof(Continue)||
								respuesta.GetType() == typeof(Return)) {
								Sentencia sent = (Sentencia)respuesta;
								ErroresCQL.Add(new Error(TipoError.Semantico, 
									"La sentencia no está en un bloque de código adecuado",
									sent.Linea, sent.Columna));
							}
						}
					}
					//MostrarReporteDeEstado(sesion);
				}
			}
			foreach (Irony.LogMessage mensaje in arbol.ParserMessages)
			{
				ErroresCQL.Add(new Error(TipoError.Lexico, mensaje.Message, mensaje.Location.Line,mensaje.Location.Column));
			}

			return Analizador.raiz != null && arbol.ParserMessages.Count == 0 && erroresCQL.Count == 0;
		}

		internal static void AddFuncion(Funcion n)
		{
			funciones.Add(n);
		}

		public static bool AnalizarChison(String texto)
		{
			GramaticaChison gramatica = new GramaticaChison();
			LanguageData ldata = new LanguageData(gramatica);
			Parser parser = new Parser(ldata);
			//IMPORTAR 
			texto = Importar(texto);
			ParseTree arbol = parser.Parse(texto);
			Analizador.ErroresChison.Clear();
			Analizador.raiz = arbol.Root;
			
			if (raiz != null && arbol.ParserMessages.Count==0)
			{
				generadorDOT.GenerarDOT(Analizador.Raiz, "C:\\Users\\Emely\\Desktop\\chison.dot");
				GeneradorDB.GuardarInformación(raiz);
				MostrarReporteDeEstadoChison();

			}
			foreach (Irony.LogMessage mensaje in arbol.ParserMessages)
			{

				//INSERTANDO ERROR EN TABLA ERRORS
				ErroresChison.Add(new Error(
					TipoError.Semantico,
					mensaje.Message,
					mensaje.Location.Line,
					mensaje.Location.Column,
					Datos.GetDate(), 
					Datos.GetTime()
					));

				Console.WriteLine("ERROR "+mensaje.Message+" En línea: "+mensaje.Location.Line," y Columna:"+mensaje.Location.Column);
			}
			return Analizador.raiz != null && arbol.ParserMessages.Count == 0 && ErroresChison.Count==0;
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
			HandlerFiles.guardarArchivo(cadena.ToString(), v);
		}

		internal static void Clear()
		{
			ErroresCQL.Clear();
			ErroresChison.Clear();
			Usuariosdb.Clear();
			BasesDeDatos.Clear();
			funciones.Clear();
			ast = null;
			raiz = null;
			Console.WriteLine("****************************************************************************");
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
				generadorDOT.GenerarDOT(Analizador.Raiz, "C:\\Users\\Emely\\Desktop\\LUP.dot");
				GeneradorLup.AnalizarEntrada(raiz);
			}
			foreach (Irony.LogMessage mensaje in arbol.ParserMessages)
			{
				ErroresCQL.Add(new Error(TipoError.Lexico, mensaje.Message, mensaje.Location.Line, mensaje.Location.Column));
			}

			return Analizador.raiz != null;
		}

		public static bool ExisteUsuario(string nombre)
		{
			foreach (Usuario db in Usuariosdb)
			{
				if (db.Nombre.Equals(nombre))
				{
					return true;
				}
			}
			return false;
		}

		public static bool ExisteDB(string nombre) {
			foreach (BaseDatos db in BasesDeDatos) {
				if (db.Nombre.Equals(nombre)) {
					return true;
				}
			}
			return false;
		}

		public static NodoAST AST { get => ast; }

		internal static List<Sentencia> GetSentenciasCQL(String codigo)
		{
			codigoAnalizado = codigo;
			GramaticaCql gramatica = new GramaticaCql();
			LanguageData ldata = new LanguageData(gramatica);
			Parser parser = new Parser(ldata);
			ParseTree arbol = parser.Parse(codigo);
			Analizador.ErroresCQL.Clear();
			Analizador.funciones.Clear();
			Analizador.raiz = arbol.Root;
			return GeneradorAstCql.GetAST(arbol.Root);
		}

		public static ParseTreeNode Raiz { get => raiz; set => raiz = value; }
		public static List<Error> ErroresCQL { get => erroresCQL; set => erroresCQL = value; }
		public static string PATH => path;
		internal static List<Error> ErroresChison { get => errors; set => errors = value; }
		internal static List<Funcion> Funciones { get => funciones; set => funciones = value; }
		internal static Sesion Sesion { get => sesion; set => sesion = value; }
		public static string CodigoAnalizado { get => codigoAnalizado; set => codigoAnalizado = value; }
		internal static UserType ErrorCatch { get => errorCatch; set => errorCatch = value; }

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

		internal static BaseDatos BuscarDB(string nombre)
		{
			foreach (BaseDatos db in BasesDeDatos)
			{
				if (db.Nombre == nombre)
				{
					return db;
				}
			}
			return null;
		}

		internal static void EliminarDB(string nombre)
		{
			try
			{
				BasesDeDatos.Remove(BuscarDB(nombre));
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error removiendo bases de datos");
			}
		}

		private static void MostrarReporteDeEstado(Sesion sesion) {
			Console.WriteLine("********************************************************************************************");
			Console.WriteLine(Datos.GetDate()+"="+Datos.GetTime());
			Console.WriteLine("------------------------------------------------------");
			Console.WriteLine("Bases de Datos: ");
			foreach (BaseDatos db in BasesDeDatos) {
				Console.WriteLine(db.Nombre);
			}
			Console.WriteLine("------------------------------------------------------");
			Console.WriteLine("Usuarios: ");
			foreach (Usuario usu in Usuariosdb) {
				Console.WriteLine(usu.Nombre+"=>"+usu.Password);
				Console.WriteLine("Permisos:");
				foreach (string per in usu.Permisos) {
					Console.WriteLine(per);
				}
			}
			if ( Analizador.Sesion.DBActual!= null)
			{
				BaseDatos dbActual = BuscarDB(Analizador.Sesion.DBActual);
				if (dbActual!=null) {
					Console.WriteLine("------------------------------------------------------");
					Console.WriteLine("Base de datos en uso: " + dbActual.Nombre);
					Console.WriteLine("-----------------");
					foreach (Tabla tb in dbActual.Tablas)
					{
						Console.WriteLine("Tabla: " + tb.Nombre);
						foreach (Columna cl in tb.Columnas)
						{
							Console.WriteLine(cl.Nombre + "=>" + cl.Tipo);
						}
						Console.WriteLine("*********");
						tb.MostrarDatos();
					}
					Console.WriteLine("-----------------");
					foreach (UserType ut in dbActual.UserTypes)
					{
						Console.WriteLine("UserType: " + ut.Nombre);
						foreach (KeyValuePair<string, TipoObjetoDB> atributos in ut.Atributos)
						{
							Console.WriteLine(atributos.Key + "=>" + atributos.Value);
						}
					}
					Console.WriteLine("-----------------");
					foreach (Procedimiento pr in dbActual.Procedimientos)
					{
						Console.WriteLine("Procedimiento: " + pr.Nombre);
					}
				}
			}
			else {
				Console.WriteLine("NO HAY BASE DE DATOS EN USO");
			}
		}

		private static void MostrarReporteDeEstadoChison()
		{
			Console.WriteLine("********************************************************************************************");
			Console.WriteLine(Datos.GetDate() + "=" + Datos.GetTime());
			Console.WriteLine("********************************************************************************************");
			Console.WriteLine("Bases de Datos: ");
			foreach (BaseDatos dbActual in BasesDeDatos)
			{
				Console.WriteLine("------------------------------------------------------");
				Console.WriteLine("" + dbActual.Nombre);
				Console.WriteLine("-----------------");
				foreach (Tabla tb in dbActual.Tablas)
				{
					Console.WriteLine("Tabla: " + tb.Nombre);
					foreach (Columna cl in tb.Columnas)
					{
						Console.WriteLine("\t"+cl.Nombre + "=>" + cl.Tipo);
					}
					Console.WriteLine("*********");
					tb.MostrarDatos();
				}
				Console.WriteLine("-----------------");
				foreach (UserType ut in dbActual.UserTypes)
				{
					Console.WriteLine("UserType: " + ut.Nombre);
					foreach (KeyValuePair<string, TipoObjetoDB> atributos in ut.Atributos)
					{
						Console.WriteLine(atributos.Key + "=>" + atributos.Value);
					}
				}
				Console.WriteLine("-----------------");
				foreach (Procedimiento pr in dbActual.Procedimientos)
				{
					Console.WriteLine("Procedimiento: " + pr.Nombre);
					Console.WriteLine("Parametros:");
					foreach (Parametro par in pr.Parametros) {
						Console.WriteLine("\t"+par.Nombre+"=>"+par.Tipo);
					}
					Console.WriteLine("Retornos:");
					foreach (Parametro par in pr.Retornos)
					{
						Console.WriteLine("\t" + par.Nombre + "=>" + par.Tipo);
					}
				}
			}
			Console.WriteLine("********************************************************************************************");
			Console.WriteLine("Usuarios: ");
			Console.WriteLine("-----------------");
			foreach (Usuario usu in Usuariosdb)
			{
				Console.WriteLine(usu.Nombre + "=>" + usu.Password);
				Console.WriteLine("Permisos:");
				foreach (string per in usu.Permisos)
				{
					Console.WriteLine("\t"+per);
				}
				Console.WriteLine("-----------------");
			}
		}

	}
}
