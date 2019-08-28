using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Analisis
{
	class GramaticaLup : Grammar
	{
		public GramaticaLup() : base(caseSensitive: false)
		{
			#region ER

			RegexBasedTerminal contenido = new RegexBasedTerminal("contenido", "[^[]*");
			RegexBasedTerminal nombre = new RegexBasedTerminal("nombre", "[a-zA-ZñÑ][\\w]*");


			#endregion

			#region Term
			//simbolos
			var abrir1 = ToTerm("[+");
			var abrir2 = ToTerm("[-");
			var barra = ToTerm("/");
			var cerrar = ToTerm("]");
			//reservadas
			var pr_login = ToTerm("LOGIN");
			var pr_user = ToTerm("USER");
			var pr_pass = ToTerm("PASS");
			var pr_logout = ToTerm("LOGOUT");
			var pr_query = ToTerm("QUERY");
			var pr_data = ToTerm("DATA");
			var pr_struct = ToTerm("STRUC");
			#endregion

			#region NoTerm
			NonTerminal INICIO = new NonTerminal("INICIO");

			NonTerminal PAQUETE = new NonTerminal("PAQUETE"),
				LOGIN=new NonTerminal("LOGIN"),
				USUARIO=new NonTerminal("USUARIO"),
				PASSWD=new NonTerminal("PASSWD"),
				LOGOUT=new NonTerminal("LOGOUT"),
				CONSULTA=new NonTerminal("CONSULTA"),
				DATA=new NonTerminal("DATA"),
				STRUCT=new NonTerminal("STRUCT")
				;

			#endregion

			#region Gramatica
			INICIO.Rule = MakeStarRule(INICIO,PAQUETE);

			PAQUETE.Rule =LOGIN
				|LOGOUT
				|CONSULTA
				|STRUCT;

			STRUCT.Rule = abrir1 + pr_struct + cerrar+USUARIO + abrir2 + pr_struct + cerrar;

			CONSULTA.Rule = abrir1 + pr_query + cerrar+USUARIO+DATA + abrir2 + pr_query + cerrar;

			DATA.Rule = abrir1 + pr_data + cerrar+contenido + abrir2 + pr_data + cerrar;

			LOGIN.Rule = abrir1 + pr_login + cerrar + USUARIO + PASSWD + abrir2 + pr_login + cerrar;

			USUARIO.Rule = abrir1 + pr_user + cerrar + contenido + abrir2 + pr_user + cerrar;

			PASSWD.Rule = abrir1 + pr_pass + cerrar + contenido + abrir2 + pr_pass + cerrar;

			LOGOUT.Rule = abrir1 + pr_logout + cerrar + USUARIO + abrir2 + pr_logout + cerrar;
			
			#endregion

			#region Ajustes
			//NO TERMINAL DE INICIO
			this.Root = INICIO;
			//PALABRAS RESERVADAS
			MarkReservedWords(pr_data.ToString(), pr_login.ToString(), pr_logout.ToString(), pr_pass.ToString(), pr_query.ToString(), pr_struct.ToString(), pr_user.ToString());
			//NODOS A OMITIR
			MarkTransient(USUARIO,PASSWD,DATA);
			//TERMINALES IGNORADO
			MarkPunctuation(abrir1,abrir2, barra, cerrar,pr_user,pr_struct,pr_query,pr_pass,pr_logout,pr_login,pr_data);
			//COMENTARIOS IGNORADOS
			//NonGrammarTerminals.Add(comentario_bloque);
			//NonGrammarTerminals.Add(comentario_linea);
			//precedencia
			RegisterOperators(1, Associativity.Left, abrir2);
			RegisterOperators(2, Associativity.Left, abrir1);
			#endregion
		}
	}
}
