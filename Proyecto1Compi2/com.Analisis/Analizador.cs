using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Ast;
using com.Analisis.Util;

namespace com.Analisis
{
	static class Analizador
	{
		private static List<Error> errores= new List<Error>();
		private static ParseTreeNode raiz= null;

		public static bool AnalizarUsql(String texto){
			Gramatica gramatica = new Gramatica();
			LanguageData ldata = new LanguageData(gramatica);
			Parser parser = new Parser(ldata);
			ParseTree arbol = parser.Parse(texto);
			Analizador.errores.Clear();
			Analizador.raiz = arbol.Root;

			foreach (Irony.LogMessage mensaje in arbol.ParserMessages)
			{
				
				errores.Add(new Error(TipoError.Lexico, mensaje.Message, mensaje.Location.Line,mensaje.Location.Column));
			}

			return Analizador.raiz != null;
		}

		public static List<Error> Errores { get => errores; }
		public static ParseTreeNode Raiz { get => raiz; }
	}
}
