using Irony.Parsing;
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
			CommentTerminal instrucciones = new CommentTerminal("instrucciones", "$", "$");
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
			var pr_databases = ToTerm("\"databases\"");
			var pr_users = ToTerm("\"users\"");
			var pr_nombre = ToTerm("\"name\"");
			var pr_passwd = ToTerm("\"password\"");
			var pr_permisos = ToTerm("\"permissions\"");
			var pr_data = ToTerm("\"data\"");
			var pr_cqlType = ToTerm("\"cql-type\"");
			var pr_columns = ToTerm("\"columns\"");
			var pr_table = ToTerm("\"table\"");
			var pr_type = ToTerm("\"type\"");
			var pr_pk = ToTerm("\"PK\"");
			var pr_obj = ToTerm("\"OBJECT\"");
			var pr_attribs = ToTerm("\"ATTRS\"");
			var pr_proc = ToTerm("\"PROCEDURE\"");
			var pr_params = ToTerm("\"PARAMETERS\"");
			var pr_inst = ToTerm("\"INSTR\"");
			var pr_as = ToTerm("\"AS\"");
			var pr_in = ToTerm("in");
			var pr_out = ToTerm("out");
			#endregion

			#region NoTerm
			NonTerminal INICIO = new NonTerminal("INICIO"),
				DATABASE = new NonTerminal("DATABASE"),
				USER = new NonTerminal("USER"),
				OTRONOMBRE = new NonTerminal("OTRONOMBRE"),
				LISTA_USERS = new NonTerminal("LISTA_USERS"),
				LISTA_DATABASES = new NonTerminal("LISTA_DATABASES"),
				LISTA_OTRONOMBRES = new NonTerminal("LISTA_OTRONOMBRES"),
				LISTA_OBJDB = new NonTerminal("LISTA_OBJDB"),
				OBJDB = new NonTerminal("OBJDB"),
				TABLA = new NonTerminal("TABLA"),
				COLUMNA = new NonTerminal("COLUMNA"),
				LISTA_COLUMNAS = new NonTerminal("LISTA_COLUMNAS"),
				TIPO = new NonTerminal("TIPO"),
				ISPRIMARY = new NonTerminal("ISPRIMARY"),
				OBJETO = new NonTerminal("OBJETO"),
				LISTA_ATRIBUTOS = new NonTerminal("LISTA_ATRIBUTOS"),
				ATRIBUTO = new NonTerminal("ATRIBUTO"),
				DATATABLE = new NonTerminal("DATATABLE"),
				LISTA_DATATABLE = new NonTerminal("LISTA_DATATABLE"),
				LISTA_FILA = new NonTerminal("LISTA_FILA"),
				FILA = new NonTerminal("FILA"),
				PROCEDIMIENTO = new NonTerminal("PROCEDIMIENTO"),
				LISTA_PARAMETROS = new NonTerminal("LISTA_PARAMETROS"),
				PARAMETRO = new NonTerminal("PARAMETRO"),
				INOUT = new NonTerminal("INOUT"),
				LISTA_DATOS = new NonTerminal("LISTA_DATOS"),
				DATO=new NonTerminal("DATO")
				;
			#endregion

			#region Gramatcia

			INICIO.Rule = dolar + menor + pr_databases + igual + cor1 + LISTA_DATABASES + cor2 + coma + pr_users + igual + cor1 + LISTA_USERS + cor2 + mayor + dolar;

			#region baseDeDatos

			LISTA_DATABASES.Rule = MakeStarRule(LISTA_DATABASES, coma, DATABASE);

			LISTA_USERS.Rule = MakeStarRule(LISTA_USERS, coma, USER);

			
			DATABASE.Rule =menor+ pr_nombre + igual + cadena + coma+ pr_data + igual + cor1 + LISTA_OBJDB + cor2 + mayor;

			LISTA_OBJDB.Rule =MakeStarRule(LISTA_OBJDB,coma,OBJDB);

			OBJDB.Rule =TABLA
				|OBJETO
				|PROCEDIMIENTO
				| SyntaxError + mayor
				| SyntaxError + coma; ;

			#endregion

			#region Tabla

			TABLA.Rule =menor+ pr_cqlType + igual + pr_table + coma+ pr_nombre + igual + cadena + coma + pr_columns + igual + cor1 + LISTA_COLUMNAS + cor2 + coma+pr_data+igual+cor1+LISTA_FILA+cor2+mayor;

			LISTA_FILA.Rule =MakeStarRule(LISTA_FILA,coma,FILA);

			FILA.Rule =menor+LISTA_DATATABLE+mayor;

			LISTA_DATATABLE.Rule = MakePlusRule(LISTA_DATATABLE,coma,DATATABLE);

			DATATABLE.Rule = cadena + igual + cadena
				|cadena+igual+numero
				|cadena+igual+date
				|cadena+igual+time
				|cadena+igual+pr_true
				|cadena+igual+pr_false
				|cadena+igual+cor1+LISTA_DATOS+cor2
				|cadena+igual+FILA
				|SyntaxError+mayor
				|SyntaxError+coma;

			LISTA_DATOS.Rule =MakeStarRule(LISTA_DATOS,coma,DATO);

			DATO.Rule =cadena
				|numero
				|cor1+LISTA_DATOS+cor2
				|FILA
				|SyntaxError+coma;

			LISTA_COLUMNAS.Rule = MakeStarRule(LISTA_COLUMNAS,coma,COLUMNA);

			COLUMNA.Rule = menor + pr_nombre + igual + cadena + coma + TIPO + coma + ISPRIMARY + mayor
				| SyntaxError + mayor
				| SyntaxError + coma;

			TIPO.Rule =pr_type+igual+cadena;

			ISPRIMARY.Rule = pr_pk + igual + pr_true
				| pr_pk + igual + pr_false;
			#endregion

			#region Objetos
			OBJETO.Rule =menor+ pr_cqlType + igual + pr_obj+coma + pr_nombre + igual + cadena + coma+pr_attribs+igual+cor1+LISTA_ATRIBUTOS+cor2+mayor;

			LISTA_ATRIBUTOS.Rule =MakeStarRule(LISTA_ATRIBUTOS,coma,ATRIBUTO);

			ATRIBUTO.Rule =menor+ pr_nombre + igual + cadena + coma+TIPO+mayor
				| SyntaxError + mayor
				| SyntaxError + coma; ;

			#endregion

			#region Usuarios
			
			USER.Rule = menor+ pr_nombre + igual + cadena + coma+ pr_passwd + igual + cadena + coma+ pr_permisos + igual + cor1 + LISTA_OTRONOMBRES + cor2 + mayor;

			LISTA_OTRONOMBRES.Rule = MakeStarRule(LISTA_OTRONOMBRES,coma,OTRONOMBRE);

			OTRONOMBRE.Rule =menor+ pr_nombre + igual + cadena+mayor;
			#endregion

			#region procedimiento
			PROCEDIMIENTO.Rule = menor + pr_cqlType + igual + pr_proc + coma + pr_nombre + igual + cadena + coma + 
				pr_params+igual+cor1+LISTA_PARAMETROS+cor2 + coma + pr_inst + igual + instrucciones + mayor;

			LISTA_PARAMETROS.Rule =MakeStarRule(LISTA_PARAMETROS,coma,PARAMETRO);

			PARAMETRO.Rule = menor+ pr_nombre + igual + cadena + coma + TIPO + coma + pr_as + igual+INOUT+mayor;

			INOUT.Rule = pr_in | pr_out;

			#endregion

			#endregion

			#region Ajustes
			//NO TERMINAL DE INICIO
			this.Root = INICIO;
			//PALABRAS RESERVADAS
			MarkReservedWords(pr_nombre.ToString(),pr_null.ToString(),pr_true.ToString(),pr_false.ToString(),pr_passwd.ToString(),pr_permisos.ToString(),pr_data.ToString(),
				pr_cqlType.ToString(),pr_columns.ToString(),pr_table.ToString(),pr_type.ToString(),pr_pk.ToString(),pr_obj.ToString(),pr_attribs.ToString(),pr_proc.ToString(),
				pr_params.ToString(),pr_inst.ToString(),pr_as.ToString(),pr_in.ToString(),pr_out.ToString(),pr_databases.ToString(),pr_users.ToString());
			//NODOS A OMITIR
			MarkTransient(OBJDB,FILA,INOUT);
			//TERMINALES IGNORADO
			MarkPunctuation(mayor,menor,igual,dolar,coma,dospuntos,cor1,cor2,
				pr_databases,pr_users,pr_nombre,pr_passwd,pr_permisos,pr_data,pr_cqlType,pr_table,pr_columns,pr_obj,pr_attribs,
				pr_type,pr_pk,pr_proc,pr_params,pr_inst,pr_as);
			//COMENTA	RIOS IGNORADOS
			NonGrammarTerminals.Add(comentario_bloque);
			NonGrammarTerminals.Add(comentario_linea);
			//PRECEDENCIA DE OPERADORES
			//RegisterOperators(1, Associativity.Right, igual);
			
			#endregion
		}
	}
}
