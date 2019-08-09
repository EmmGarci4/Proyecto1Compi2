﻿using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Analisis
{
	class GramaticaChison:Grammar
	{
		public GramaticaChison() : base(caseSensitive: false)
		{
			#region ER
			StringLiteral cadena = new StringLiteral("cadena", "\"", StringOptions.IsTemplate);
			NumberLiteral numero = new NumberLiteral("numero", NumberOptions.AllowSign);
			RegexBasedTerminal id = new RegexBasedTerminal("id", "@[a-zA-ZñÑ]([a-zA-ZñÑ0-9_])*");
			RegexBasedTerminal date = new RegexBasedTerminal("date", "'[0-9]{4}-[0-9]{2}-[0-9]{2}'");
			RegexBasedTerminal time = new RegexBasedTerminal("time", "'[0-9]{2}:[0-9]{2}:[0-9]{2}'");
			RegexBasedTerminal nombre = new RegexBasedTerminal("nombre", "[a-zA-ZñÑ]+([a-zA-ZñÑ]|_|[0-9])*");
			CommentTerminal comentario_linea = new CommentTerminal("comentario_linea", "//", "\n", "\r\n");
			CommentTerminal comentario_bloque = new CommentTerminal("comentario_bloque", "/*", "*/");
			#endregion

			#region Term
			var mayor = ToTerm(">");
			var menor = ToTerm("<");
			var igual = ToTerm("=");
			var dolar = ToTerm("$");
			var coma = ToTerm(",");
			var dospuntos = ToTerm(":");
			var cor1 = ToTerm("[");
			var cor2 = ToTerm("]");
			var pr_null = ToTerm("null");
			var pr_true = ToTerm("true");
			var pr_false = ToTerm("false");
			#endregion

			#region NoTerm
			NonTerminal INICIO = new NonTerminal("INICIO"),
				OBJETO = new NonTerminal("OBJETO"),
				LISTA_ATRIBUTOS=new NonTerminal("LISTA_ATRIBUTOS"),
				ATRIBUTO=new NonTerminal("ATRIBUTO"),
				VALOR=new NonTerminal("VALOR"),
				LISTA=new NonTerminal("LISTA")
				;
			#endregion

			#region Gramatcia

			INICIO.Rule = dolar+ OBJETO+ dolar;

			OBJETO.Rule =  menor + LISTA_ATRIBUTOS + mayor ;

			LISTA_ATRIBUTOS.Rule =MakePlusRule(LISTA_ATRIBUTOS,coma,ATRIBUTO);

			ATRIBUTO.Rule =cadena+igual+VALOR;

			VALOR.Rule = cadena
				| numero
				| date
				| time
				| pr_null
				| id
				|pr_true
				|pr_false
				|cor1+LISTA+cor2
				|OBJETO
				;

			LISTA.Rule = MakeStarRule(LISTA,coma,VALOR);

			#endregion

			#region Ajustes
			//NO TERMINAL DE INICIO
			this.Root = INICIO;
			//PALABRAS RESERVADAS
			MarkReservedWords(pr_null.ToString(),pr_true.ToString(),pr_false.ToString());
			//NODOS A OMITIR
			MarkTransient();
			//TERMINALES IGNORADO
			MarkPunctuation(mayor,menor,igual,dolar,coma,dospuntos);
			//COMENTA	RIOS IGNORADOS
			NonGrammarTerminals.Add(comentario_bloque);
			NonGrammarTerminals.Add(comentario_linea);
			//PRECEDENCIA DE OPERADORES
			//RegisterOperators(1, Associativity.Right, igual);
			
			#endregion
		}
	}
}
