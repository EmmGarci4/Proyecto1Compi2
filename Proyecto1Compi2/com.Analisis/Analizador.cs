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

namespace com.Analisis
{
	static class Analizador
	{
		private static List<Error> errores= new List<Error>();
		private static NodoAST ast= null;
		private static ParseTreeNode raiz;

		public static bool AnalizarUsql(String texto){
			GramaticaSql gramatica = new GramaticaSql();
			LanguageData ldata = new LanguageData(gramatica);
			Parser parser = new Parser(ldata);
			ParseTree arbol = parser.Parse(texto);
			Analizador.errores.Clear();
			Analizador.raiz = arbol.Root;
			if (raiz!=null) {
				Analizador.ast = GeneradorAstSql.GetAST(arbol.Root);
				Expresion ex = (Expresion)Analizador.ast;
				Console.WriteLine("Valor:" + ex.GetValor(new TablaSimbolos(0, "global")).ToString());
			}
			foreach (Irony.LogMessage mensaje in arbol.ParserMessages)
			{
				errores.Add(new Error(TipoError.Lexico, mensaje.Message, mensaje.Location.Line,mensaje.Location.Column));
			}

			return Analizador.raiz != null;
		}

		public static bool AnalizarXml(String texto)
		{
			GramaticaXml gramatica = new GramaticaXml();
			LanguageData ldata = new LanguageData(gramatica);
			Parser parser = new Parser(ldata);
			ParseTree arbol = parser.Parse(texto);
			Analizador.errores.Clear();
			Analizador.raiz = arbol.Root;

			foreach (Irony.LogMessage mensaje in arbol.ParserMessages)
			{

				errores.Add(new Error(TipoError.Lexico, mensaje.Message, mensaje.Location.Line, mensaje.Location.Column));
			}

			return Analizador.raiz != null;
		}

		public static List<Error> Errores { get => errores; }
		public static NodoAST AST { get => ast; }
		public static ParseTreeNode Raiz { get => raiz; set => raiz = value; }
	}
}
