using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;
namespace com.Analisis
{
	class GramaticaXml:Grammar
	{
		public GramaticaXml() : base(caseSensitive: false) {
			#region ER

			RegexBasedTerminal contenido = new RegexBasedTerminal("contenido", "[^<\\/]+");
			RegexBasedTerminal nombre = new RegexBasedTerminal("nombre", "[a-zA-ZñÑ][\\w]*");
	

			#endregion

			#region Term
			//simbolos
			var abrir = ToTerm("<");
			var barra = ToTerm("/");
			var cerrar = ToTerm(">");
			
			#endregion

			#region NoTerm
			NonTerminal INICIO = new NonTerminal("INICIO");

			NonTerminal LISTA = new NonTerminal("LISTA"),
				ETIQUETA = new NonTerminal("ETIQUETA");

			#endregion

			#region Gramatica
			INICIO.Rule = LISTA;

			LISTA.Rule =MakePlusRule(LISTA,ETIQUETA);

			ETIQUETA.Rule = abrir + nombre + cerrar + abrir + barra + nombre + cerrar
				| abrir + nombre + cerrar + contenido + abrir + barra + nombre + cerrar
				| abrir + nombre + cerrar + LISTA + abrir + barra + nombre + cerrar;

			#endregion

			#region Ajustes
			//NO TERMINAL DE INICIO
			this.Root = INICIO;
			//PALABRAS RESERVADAS
			MarkReservedWords();
			//NODOS A OMITIR
			//MarkTransient(SENTENCIA,ASCDESC);
			//TERMINALES IGNORADO
			MarkPunctuation(abrir,barra,cerrar);
			//COMENTARIOS IGNORADOS
			//NonGrammarTerminals.Add(comentario_bloque);
			//NonGrammarTerminals.Add(comentario_linea);
			//precedencia
			RegisterOperators(4, Associativity.Neutral, abrir);
			RegisterOperators(5, Associativity.Neutral, barra);
			#endregion
		}
	}
}
 