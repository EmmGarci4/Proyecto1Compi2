using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;
namespace com.Analisis
{
	class GramaticaCql:Grammar
	{
		public GramaticaCql() : base(caseSensitive: false) {
			#region ER
			StringLiteral cadena = new StringLiteral("cadena", "\"", StringOptions.IsTemplate);
			NumberLiteral numero = new NumberLiteral("numero",NumberOptions.AllowSign);
			RegexBasedTerminal id = new RegexBasedTerminal("id", "@[a-zA-ZñÑ]([a-zA-ZñÑ0-9_])*");
			RegexBasedTerminal date = new RegexBasedTerminal("date", "'[0-9]{4}-[0-9]{2}-[0-9]{2}'");
			RegexBasedTerminal time = new RegexBasedTerminal("time", "'[0-9]{2}:[0-9]{2}:[0-9]{2}'");
			RegexBasedTerminal nombre = new RegexBasedTerminal("nombre", "[a-zA-ZñÑ]+([a-zA-ZñÑ]|_|[0-9])*");
			CommentTerminal comentario_linea = new CommentTerminal("comentario_linea", "//", "\n", "\r\n");
			CommentTerminal comentario_bloque = new CommentTerminal("comentario_bloque", "/*", "*/");

			#endregion

			#region Term
			//simbolos
			var par1 = ToTerm("(");
			var par2 = ToTerm(")");
			var mas = ToTerm("+");
			var menos = ToTerm("-");
			var por = ToTerm("*");
			var div = ToTerm("/");
			var pot = ToTerm("^");
			///mods
			var masmas = ToTerm("++");
			var menosmenos = ToTerm("--");
			//relacionales
			var mayor = ToTerm(">");
			var menor = ToTerm("<");
			var igual = ToTerm("=");
			var mayorigual = ToTerm(">=");
			var menorigual = ToTerm("<=");
			var igualigual = ToTerm("==");
			var notigual = ToTerm("!=");
			var not = ToTerm("!");
			//logicos
			var or = ToTerm("||");
			var and = ToTerm("&&");
			//puntuacion
			var llave1 = ToTerm("{");
			var llave2 = ToTerm("}");
			var cor1 = ToTerm("[");
			var cor2 = ToTerm("]");
			var puntoycoma = ToTerm(";");
			var coma = ToTerm(",");
			var dospuntos = ToTerm(":");
			var punto = ToTerm(".");
			//palabras reservadas
			var pr_true = ToTerm("true");
			var pr_false = ToTerm("false");
			var pr_text = ToTerm("string");
			var pr_integer = ToTerm("int");
			var pr_double = ToTerm("double");
			var pr_bool = ToTerm("boolean");
			var pr_date = ToTerm("date");
			var pr_time = ToTerm("time");
			var pr_null = ToTerm("null");
			var pr_counter = ToTerm("counter");
			var pr_map = ToTerm("map");
			var pr_set = ToTerm("set");
			var pr_list = ToTerm("list");
			var pr_crear = ToTerm("CREATE");
			var pr_eliminar = ToTerm("DROP");
			var pr_usar = ToTerm("USE");
			var pr_alterar = ToTerm("ALTER");
			var pr_db = ToTerm("DATABASE");
			var pr_tabla = ToTerm("TABLE");
			var pr_usuario = ToTerm("USER");
			var pr_password = ToTerm("PASSWORD");
			var pr_limit = ToTerm("LIMIT");
			var pr_insertar = ToTerm("INSERT");
			var pr_on = ToTerm("ON");
			var pr_in = ToTerm("IN");
			var pr_into = ToTerm("INTO");
			var pr_actualizar = ToTerm("UPDATE");
			var pr_valores = ToTerm("VALUES");
			var pr_donde = ToTerm("WHERE");
			var pr_seleccionar = ToTerm("SELECT");
			var pr_ordenar = ToTerm("ORDER");
			var pr_ordPor = ToTerm("BY");
			var pr_asc = ToTerm("ASC");
			var pr_desc = ToTerm("DESC");
			var pr_otorgar = ToTerm("GRANT");
			var pr_denegar = ToTerm("REVOKE");
			var pr_de = ToTerm("OF");
			var pr_agregar = ToTerm("ADD");
			var pr_type = ToTerm("TYPE");

			var pr_if = ToTerm("IF");
			var pr_else = ToTerm("ELSE");
			var pr_switch = ToTerm("SWITCH");
			var pr_case = ToTerm("CASE");
			var pr_default = ToTerm("DEFAULT");
			var pr_for = ToTerm("FOR");
			var pr_while = ToTerm("WHILE");
			var pr_break = ToTerm("BREAK");
			var pr_return = ToTerm("RETURN");
			var pr_llave = ToTerm("KEY");
			var pr_primaria = ToTerm("PRIMARY");
			var pr_not = ToTerm("NOT");
			var pr_exist = ToTerm("EXIST");
			var pr_exists = ToTerm("EXISTS");
			var pr_new = ToTerm("NEW");
			var pr_truncar = ToTerm("TRUNCATE");
			var pr_backup = ToTerm("COMMIT");
			var pr_restaurar = ToTerm("ROLLBACK");
			var pr_con = ToTerm("WITH");
			var pr_borrar = ToTerm("DELETE");
			var pr_from = ToTerm("FROM");
			var pr_begin = ToTerm("BEGIN");
			var pr_batch = ToTerm("BATCH");
			var pr_apply = ToTerm("APPLY");
			var pr_count = ToTerm("COUNT");
			var pr_min = ToTerm("MIN");
			var pr_max = ToTerm("MAX");
			var pr_sum = ToTerm("SUM");
			var pr_avg = ToTerm("AVG");

			var pr_cambiar = ToTerm("CAMBIAR");
			var pr_proc = ToTerm("PROCEDIMIENTO");
			var pr_funcion = ToTerm("FUNCION");
			var pr_declarar = ToTerm("DECLARAR");
			var pr_usqldump = ToTerm("USQLDUMP");
			var pr_completo = ToTerm("COMPLETO");
			var pr_contar = ToTerm("CONTAR");



			#endregion

			#region NoTerm
			NonTerminal INICIO = new NonTerminal("INICIO");

			NonTerminal EXPRESION = new NonTerminal("EXPRESION"),
				VALOR = new NonTerminal("VALOR"),
				CONDICION=new NonTerminal("CONDICION"),
				LLAMADAFUNCION = new NonTerminal("LLAMADAFUNCION"),
				LISTAEXPRESIONES = new NonTerminal("LISTAEXPRESIONES"),
				INFOCOLLECTIONS=new NonTerminal("INFOCOLLECTIONS"),
				INFO=new NonTerminal("INFO"),
				ACCESO = new NonTerminal("ACCESO"),
				AC_CAMPO=new NonTerminal("AC_CAMPO")
				;

			NonTerminal SENTENCIAS = new NonTerminal("SENTENCIAS"),
				SENTENCIA = new NonTerminal("SENTENCIA"),
				SENTENCIADDL = new NonTerminal("SENTENCIADDL"),
				CREAR_DB = new NonTerminal("CREAR_DB"),
				CREAR_TABLA = new NonTerminal("CREAR_TABLA"),
				LISTACAMPOSTABLA = new NonTerminal("LISTACAMPOSTABLA"),
				CAMPOTABLA = new NonTerminal("CAMPOTABLA"),
				TIPODATO = new NonTerminal("TIPODATO"),
				LLAVEPRIMARIA = new NonTerminal("LLAVEPRIMARIA"),
				RETORNO = new NonTerminal("RETORNO"),
				USAR_DB = new NonTerminal("USAR_DB"),
				ALTERAR_TABLA = new NonTerminal("ALTERAR_TABLA"),
				LISTANOMBRES = new NonTerminal("LISTANOMBRES"),
				ELIMINAR_TABLA = new NonTerminal("ELIMINAR_TABLA"),
				ELIMINAR_DB = new NonTerminal("ELIMINAR_DB"),
				TRUNCAR_TABLA = new NonTerminal("TRUNCAR_TABLA"),
				CREAR_USERTYPE = new NonTerminal("CREAR_USERTYPE"),
				ALTERAR_USERTYPE = new NonTerminal("ALTERAR_USERTYPE"),
				ELIMINAR_USERTYPE = new NonTerminal("ELIMINAR_USERTYPE"),
				ATRIBUTO = new NonTerminal("ATRIBUTO"),
				LISTAATRIBUTOS = new NonTerminal("LISTAATRIBUTOS"),
				LISTANOMBRESPURA = new NonTerminal("LISTANOMBRESPURA")
				;

			NonTerminal SENTENCIATCL = new NonTerminal("SENTENCIATCL"),
				BACKUP = new NonTerminal("COMMIT"),
				RESTAURAR = new NonTerminal("ROLLBACK");

			NonTerminal SENTENCIADCL = new NonTerminal("SENTENCIADCL"),
				CREAR_USUARIO = new NonTerminal("CREAR_USUARIO"),
				ALTERAR_USUARIO = new NonTerminal("ALTERAR_USUARIO"),
				CREAR_PROC = new NonTerminal("CREAR_PROC"),
				CREAR_FUNCION = new NonTerminal("CREAR_FUNCION"),
				ELIMINARUSUARIO = new NonTerminal("ELIMINARUSUARIO"),
				BLOQUESENTENCIAS = new NonTerminal("BLOQUESENTENCIAS"),
				SENTENCIABLOQUE = new NonTerminal("SENTENCIABLOQUE"),
				LISTAPARAMETROS = new NonTerminal("LISTAPARAMETROS"),
				PARAMETRO = new NonTerminal("PARAMETRO");


			NonTerminal SENTENCIADML = new NonTerminal("SENTENCIADML"),
				SELECCIONAR = new NonTerminal("SELECCIONAR"),
				BORRAR = new NonTerminal("BORRAR"),
				ACTUALIZAR = new NonTerminal("ACTUALIZAR"),
				INSERTAR = new NonTerminal("INSERTAR"),
				PROPIEDADSELECCIONAR = new NonTerminal("PROPIEDADSELECCIONAR"),
				PROPIEDADDONDE = new NonTerminal("PROPIEDADDONDE"),
				PROPIEDADORDENAR = new NonTerminal("PROPIEDADORDENAR"),
				ASCDESC = new NonTerminal("ASCDESC"),
				LISTA_ASIGNACIONES = new NonTerminal("LISTA_ASIGNACIONES"),
				PROPSELECT = new NonTerminal("PROPSELECT"),
				PROPIEDADLIMIT = new NonTerminal("PROPIEDADLIMIT"),
				PROPORDER = new NonTerminal("PROPORDER"),
				BATCH = new NonTerminal("BATCH"),
				SENTENCIASDML = new NonTerminal("SENTENCIASDML"),
				FUNCIONAGREGACION = new NonTerminal("FUNCIONAGREGACION"),
				NOMBREFUNCION = new NonTerminal("NOMBREFUNCION"),
				ASIGNACIONAC=new NonTerminal("ASIGNACIONAC")
				;

			NonTerminal OTORGAR = new NonTerminal("OTORGAR"),
				DENEGAR = new NonTerminal("DENEGAR"),
				ASIGNACION = new NonTerminal("ASIGNACION"),
				DECLARACION = new NonTerminal("DECLARACION"),
				IF = new NonTerminal("IF"),
				ELSE = new NonTerminal("ELSE"),
				SWITCH = new NonTerminal("SWITCH"),
				CASE = new NonTerminal("CASE"),
				DEFAULT = new NonTerminal("DEFAULT"),
				LISTACASE = new NonTerminal("LISTACASE"),
				FOR = new NonTerminal("FOR"),
				BREAK = new NonTerminal("BREAK"),
				LISTAVARIABLES = new NonTerminal("LISTAVARIABLES"),
				OPPFOR = new NonTerminal("OPPFOR"),
				WHILE = new NonTerminal("WHILE"), 
				CONTAR = new NonTerminal("CONTAR")
				;

			#endregion

			#region Gramatica
			INICIO.Rule = SENTENCIAS;

			SENTENCIAS.Rule = MakeStarRule(SENTENCIAS, SENTENCIA);

			SENTENCIA.Rule = SENTENCIADDL | SENTENCIATCL | SENTENCIADCL | SENTENCIASDML | BATCH
				| FUNCIONAGREGACION;

			#region expresion

			EXPRESION.Rule = //aritmeticos
				EXPRESION + mas + EXPRESION
				| EXPRESION + menos + EXPRESION
				| EXPRESION + por + EXPRESION
				| EXPRESION + div + EXPRESION
				| EXPRESION + pot + EXPRESION
				//otros
				| CONDICION
				| VALOR;

			VALOR.Rule = cadena
				| numero
				| date
				| time
				| pr_null
				| id
				| par1 + EXPRESION + par2
				| menos + EXPRESION
				| LLAMADAFUNCION
				| ACCESO
				| llave1 + INFOCOLLECTIONS + llave2
				| cor1 + LISTAEXPRESIONES + cor2
				//|FUNCIONAGREGACION
				;

			ACCESO.Rule = MakePlusRule(ACCESO, punto, AC_CAMPO);
			
			AC_CAMPO.Rule = nombre + cor1 + EXPRESION + cor2
				| nombre;

			INFOCOLLECTIONS.Rule = MakePlusRule(INFOCOLLECTIONS, coma, INFO)
				| MakePlusRule(INFOCOLLECTIONS,coma,cadena); 

			INFO.Rule = cadena + dospuntos + EXPRESION;

			CONDICION.Rule = EXPRESION + mayor + EXPRESION
				| EXPRESION + menor + EXPRESION
				| EXPRESION + mayorigual + EXPRESION
				| EXPRESION + menorigual + EXPRESION
				| EXPRESION + igualigual + EXPRESION
				| EXPRESION + igual + EXPRESION
				| EXPRESION + notigual + EXPRESION
				| EXPRESION + and + EXPRESION
				| EXPRESION + or + EXPRESION
				| not + CONDICION
				| pr_true
				| pr_false;

			LLAMADAFUNCION.Rule = nombre + par1 + LISTAEXPRESIONES + par2
				|CONTAR;

			LISTANOMBRES.Rule = MakePlusRule(LISTANOMBRES, coma, ACCESO);

			LISTAEXPRESIONES.Rule = MakeStarRule(LISTAEXPRESIONES, coma, EXPRESION);

			LISTANOMBRESPURA.Rule = MakePlusRule(LISTANOMBRESPURA,coma,nombre);

			#endregion

			#region sentencias DDL/TCL/DCL

			SENTENCIADDL.Rule = CREAR_DB
				| USAR_DB
				| ELIMINAR_DB
				| CREAR_TABLA
				| ALTERAR_TABLA
				| ELIMINAR_TABLA
				| TRUNCAR_TABLA
				| CREAR_USERTYPE
				| ALTERAR_USERTYPE
				| ELIMINAR_USERTYPE;

			SENTENCIATCL.Rule =BACKUP
				| RESTAURAR;

			SENTENCIADCL.Rule =CREAR_USUARIO
				|OTORGAR
				|DENEGAR;
			;

			USAR_DB.Rule = pr_usar + nombre + puntoycoma;

			CREAR_DB.Rule = pr_crear + pr_db + nombre + puntoycoma
				| pr_crear + pr_db +pr_if+pr_not+pr_exist+ nombre + puntoycoma;

			ELIMINAR_DB.Rule = pr_eliminar + pr_db + nombre + puntoycoma;

			CREAR_TABLA.Rule = pr_crear + pr_tabla + nombre + par1 + LISTACAMPOSTABLA + par2 + puntoycoma
				| pr_crear + pr_tabla + pr_if + pr_not + pr_exist + nombre + par1 + LISTACAMPOSTABLA + par2 + puntoycoma;

			LISTACAMPOSTABLA.Rule = MakePlusRule(LISTACAMPOSTABLA, coma, CAMPOTABLA);

			CAMPOTABLA.Rule = nombre+TIPODATO + pr_primaria+pr_llave
				|nombre+TIPODATO
				| pr_primaria + pr_llave+par1+LLAVEPRIMARIA+par2;

			TIPODATO.Rule = pr_text
				| pr_integer
				| pr_double
				| pr_bool
				| pr_date
				| pr_time
				| pr_counter
				| pr_map+menor+TIPODATO+coma+TIPODATO+mayor
				| pr_set+menor + TIPODATO+ mayor
				| pr_list + menor + TIPODATO + mayor
				| nombre;


			LLAVEPRIMARIA.Rule =MakePlusRule(LLAVEPRIMARIA,coma,nombre);//CAMBIAR SI CAMPO DE OBJETO PUEDE SER LLAVE PRIMARIA

			ALTERAR_TABLA.Rule = pr_alterar + pr_tabla + nombre + pr_agregar +  LISTACAMPOSTABLA  + puntoycoma
				| pr_alterar + pr_tabla + nombre + pr_eliminar + LISTANOMBRESPURA + puntoycoma; 
			//CAMBIAR SI SE PUEDE ELIMINAR CAMPO DE OBJETO DESDE ACA
			//CAMBIAR LISTANOMBRES A LISTA ID SIN @

			ELIMINAR_TABLA.Rule = pr_eliminar + pr_tabla + nombre + puntoycoma 
				| pr_eliminar + pr_tabla + pr_if + pr_exists + nombre + puntoycoma;


			TRUNCAR_TABLA.Rule = pr_truncar + pr_tabla + nombre+puntoycoma;

			BACKUP.Rule = pr_backup + puntoycoma;

			RESTAURAR.Rule = pr_restaurar+ puntoycoma;

			CREAR_USUARIO.Rule = pr_crear + pr_usuario + nombre + pr_con + pr_password + cadena + puntoycoma;

			OTORGAR.Rule = pr_otorgar +nombre +pr_on+ nombre + puntoycoma;

			DENEGAR.Rule = pr_denegar + nombre + pr_on + nombre + puntoycoma;

			CREAR_USERTYPE.Rule = pr_crear + pr_type + nombre + par1 + LISTAATRIBUTOS + par2 + puntoycoma
				| pr_crear + pr_type + pr_if + pr_not + pr_exists + nombre + par1 + LISTAATRIBUTOS + par2 + puntoycoma;

			LISTAATRIBUTOS.Rule = MakePlusRule(LISTAATRIBUTOS,coma,ATRIBUTO);

			ATRIBUTO.Rule = nombre + TIPODATO;

			ALTERAR_USERTYPE.Rule = pr_alterar + pr_type + nombre + pr_agregar + par1 + LISTAATRIBUTOS + par2 + puntoycoma
				| pr_alterar + pr_type + nombre + pr_borrar + par1 + LISTANOMBRES + par2 + puntoycoma;

			ELIMINAR_USERTYPE.Rule =pr_borrar+pr_type+nombre+puntoycoma;

			/*
			ALTERAR_USUARIO.Rule=pr_alterar+pr_usuario+nombre+pr_cambiar+pr_password+igual+cadena+puntoycoma;


			ELIMINARUSUARIO.Rule = pr_eliminar + pr_usuario + nombre + puntoycoma;
			*/

			#endregion

			#region sentencias DML

			SENTENCIASDML.Rule = MakePlusRule(SENTENCIASDML, SENTENCIADML);

			SENTENCIADML.Rule =INSERTAR
				|ACTUALIZAR
				|BORRAR
				|SELECCIONAR+puntoycoma;

			//cambiar si se puede insertar valores en objeto desde aca

			INSERTAR.Rule =pr_insertar+pr_into+nombre+pr_valores +par1 + LISTAEXPRESIONES+par2+puntoycoma
				|pr_insertar+pr_into+nombre+par1+LISTANOMBRESPURA+par2+pr_valores+par1+LISTAEXPRESIONES+par2+puntoycoma; //validar cantidad de columnas y expresiones


			ACTUALIZAR.Rule = pr_actualizar + nombre + pr_set + LISTA_ASIGNACIONES + puntoycoma
				| pr_actualizar + nombre + pr_set + LISTA_ASIGNACIONES + pr_donde + CONDICION + puntoycoma;

			LISTA_ASIGNACIONES.Rule = MakePlusRule(LISTA_ASIGNACIONES,coma,ASIGNACIONAC);

			ASIGNACIONAC.Rule = ACCESO + igual + EXPRESION;

			//cambiar listas si se puede eliminar campos de objetos desde aca
			BORRAR.Rule =pr_borrar + pr_from + nombre + puntoycoma
				| pr_borrar+LISTANOMBRESPURA + pr_from + nombre + puntoycoma;

			SELECCIONAR.Rule =pr_seleccionar+LISTANOMBRES+pr_from+nombre+PROPIEDADSELECCIONAR
				| pr_seleccionar + por + pr_from + nombre + PROPIEDADSELECCIONAR;

			PROPIEDADSELECCIONAR.Rule = MakeStarRule(PROPIEDADSELECCIONAR,PROPSELECT);

			PROPSELECT.Rule = PROPIEDADDONDE
				| pr_ordenar + pr_ordPor+PROPIEDADORDENAR
				| PROPIEDADLIMIT;

			PROPIEDADDONDE.Rule = pr_donde + CONDICION
				|pr_donde+ACCESO+pr_in+par1+LISTAEXPRESIONES+par2;

			PROPIEDADORDENAR.Rule = MakePlusRule(PROPIEDADORDENAR,coma,PROPORDER);

			PROPORDER.Rule = ACCESO + ASCDESC;

			ASCDESC.Rule =pr_asc|pr_desc|Empty;

			PROPIEDADLIMIT.Rule =pr_limit+EXPRESION;

			BATCH.Rule = pr_begin + pr_batch + SENTENCIASDML + pr_apply + pr_batch + puntoycoma;

			FUNCIONAGREGACION.Rule = NOMBREFUNCION + par1+menor + SELECCIONAR +mayor+par2 + puntoycoma;

			NOMBREFUNCION.Rule = pr_count 
				| pr_min 
				| pr_max 
				| pr_sum 
				| pr_avg;

			#endregion
		
			#region sentencias ssl

			BLOQUESENTENCIAS.Rule = MakeStarRule(BLOQUESENTENCIAS, SENTENCIABLOQUE);

			SENTENCIABLOQUE.Rule = RETORNO
				| SENTENCIA;

			DECLARACION.Rule = pr_declarar + LISTAVARIABLES + TIPODATO + puntoycoma
				| pr_declarar + LISTAVARIABLES + TIPODATO + igual + EXPRESION + puntoycoma;
				
			LISTAVARIABLES.Rule =MakePlusRule(LISTAVARIABLES,coma,id);
			/*
			ASIGNACION.Rule = id + igual + EXPRESION + puntoycoma
				| id + punto +  +igual + EXPRESION + puntoycoma;
				*/
			IF.Rule = pr_if + par1 + CONDICION + par2 + llave1 + BLOQUESENTENCIAS + llave2
				| pr_if + par1 + EXPRESION + par2 + llave1 + BLOQUESENTENCIAS + llave2 + pr_else + llave1 + BLOQUESENTENCIAS + llave2;

			SWITCH.Rule = pr_switch + par1 + EXPRESION + par2 + llave1 + LISTACASE + DEFAULT + llave2
				| pr_switch + par1 + EXPRESION + par2 + llave1 + LISTACASE + llave2;

			LISTACASE.Rule = MakePlusRule(LISTACASE,CASE);

			CASE.Rule = pr_case + EXPRESION + dospuntos + BLOQUESENTENCIAS;

			DEFAULT.Rule = pr_default + dospuntos + BLOQUESENTENCIAS;

			BREAK.Rule = pr_break + puntoycoma;

			FOR.Rule = pr_for + par1 + pr_declarar + id + igual + EXPRESION + puntoycoma + CONDICION + puntoycoma + OPPFOR + par2 + llave1 + BLOQUESENTENCIAS + llave2;

			OPPFOR.Rule = mas + mas
				| menos + menos;

			WHILE.Rule =pr_while+par1+CONDICION+par2+llave1+BLOQUESENTENCIAS+llave2;

			CONTAR.Rule = pr_contar + par1  +SELECCIONAR + par2;


			CREAR_PROC.Rule = pr_crear + pr_proc + nombre + par1 + LISTAPARAMETROS + par2 + llave1 + BLOQUESENTENCIAS + llave2;

			LISTAPARAMETROS.Rule = MakeStarRule(LISTAPARAMETROS, coma, PARAMETRO);

			PARAMETRO.Rule = TIPODATO + id;

			CREAR_FUNCION.Rule = pr_crear + pr_funcion + nombre + par1 + LISTAPARAMETROS + par2 + TIPODATO + llave1 + BLOQUESENTENCIAS + llave2;

			RETORNO.Rule = pr_return + EXPRESION + puntoycoma;

			#endregion
			//revisar lista de acceso a objetos basado en si existen objetos con objetos dentro y si son accesibles
			//revisar contar con seleccionar con expresion al final y >>
			#endregion

			#region Ajustes
			//NO TERMINAL DE INICIO
			this.Root = INICIO;
			//PALABRAS RESERVADAS
			MarkReservedWords(pr_crear.ToString(),pr_db.ToString(),pr_tabla.ToString(),pr_text.ToString(),pr_integer.ToString(),pr_double.ToString(),pr_bool.ToString(),
				pr_date.ToString(),pr_proc.ToString(),pr_funcion.ToString(),pr_return.ToString(),pr_usuario.ToString(),pr_into.ToString(),pr_limit.ToString(),pr_apply.ToString(),
				pr_con.ToString(),pr_password.ToString(),pr_usar.ToString(),pr_alterar.ToString(),pr_agregar.ToString(),pr_cambiar.ToString(),pr_begin.ToString(),pr_batch.ToString(),
				pr_insertar.ToString(),pr_on.ToString(),pr_actualizar.ToString(),pr_valores.ToString(),pr_donde.ToString(),pr_borrar.ToString(),pr_seleccionar.ToString(),
				pr_ordenar.ToString(),pr_ordPor.ToString(),pr_asc.ToString(),pr_desc.ToString(),pr_otorgar.ToString(),pr_denegar.ToString(),pr_de.ToString(),pr_count.ToString(),
				pr_declarar.ToString(),pr_if.ToString(),pr_else.ToString(),pr_switch.ToString(),pr_case.ToString(),pr_default.ToString(),pr_for.ToString(),pr_while.ToString(),
				pr_break.ToString(),pr_backup.ToString(),pr_usqldump.ToString(),pr_completo.ToString(),pr_restaurar.ToString(),pr_contar.ToString(),pr_true.ToString(),pr_false.ToString(),
				pr_time.ToString(), pr_null.ToString(), pr_counter.ToString(), pr_map.ToString(), pr_set.ToString(), pr_list.ToString(), pr_type.ToString(),pr_type.ToString(),
				pr_not.ToString(), pr_exist.ToString(), pr_new.ToString(),pr_llave.ToString(),pr_primaria.ToString(),pr_truncar.ToString(),pr_exists.ToString(),
				pr_min.ToString(),pr_max.ToString(),pr_sum.ToString(),pr_avg.ToString(),pr_in.ToString());
			//NODOS A OMITIR
			MarkTransient(SENTENCIADDL,SENTENCIATCL, SENTENCIADCL,SENTENCIADML, SENTENCIASDML, ASCDESC,NOMBREFUNCION);
			//TERMINALES IGNORADO
			MarkPunctuation(par1,par2,coma,puntoycoma,igual,llave1,llave2,punto,dospuntos,cor1,cor2,
				pr_crear,pr_db,pr_eliminar,pr_usuario,pr_con,pr_password,pr_tabla,pr_alterar,pr_cambiar, pr_usar,pr_proc,pr_funcion,pr_insertar,pr_on,
				pr_valores,pr_actualizar,pr_donde,pr_seleccionar,pr_de,pr_ordenar,pr_ordPor,pr_otorgar,pr_denegar,pr_declarar,pr_if,pr_switch,pr_for,pr_while,
				pr_backup,pr_restaurar,pr_else,pr_case,pr_default);		
			//COMENTA	RIOS IGNORADOS
			NonGrammarTerminals.Add(comentario_bloque);
			NonGrammarTerminals.Add(comentario_linea);
			//PRECEDENCIA DE OPERADORES
			RegisterOperators(1, Associativity.Left, or);
			RegisterOperators(2, Associativity.Left, and);
			RegisterOperators(3, Associativity.Neutral, mayor, menor, notigual, igualigual, menorigual, mayorigual);
			RegisterOperators(4, Associativity.Left, mas, menos);
			RegisterOperators(5, Associativity.Left,por, div);
			RegisterOperators(6, Associativity.Right,pot);
			RegisterOperators(7, Associativity.Right, not);
			#endregion
		}
	}
}
 