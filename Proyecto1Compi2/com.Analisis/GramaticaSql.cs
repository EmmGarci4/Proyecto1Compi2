using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;
namespace com.Analisis
{
	class GramaticaSql:Grammar
	{
		public GramaticaSql() : base(caseSensitive: false) {
			#region ER
			StringLiteral cadena = new StringLiteral("cadena", "\"", StringOptions.IsTemplate);
			NumberLiteral entero = new NumberLiteral("entero");
			RegexBasedTerminal decimal_ = new RegexBasedTerminal("decimal", "[0-9]+.[0-9]+");
			RegexBasedTerminal date = new RegexBasedTerminal("date", "[0-9]{2}-[0-9]{2}-[0-9]{4}");
			RegexBasedTerminal datetime = new RegexBasedTerminal("datetime", "[0-9]{2}-[0-9]{2}-[0-9]{4}\\s+[0-9]{2}:[0-9]{2}:[0-9]{2}");
			RegexBasedTerminal id = new RegexBasedTerminal("id", "@[a-zA-ZñÑ]([a-zA-ZñÑ0-9_])*");
			RegexBasedTerminal nombre = new RegexBasedTerminal("nombre", "[a-zA-ZñÑ]+([a-zA-ZñÑ]|_|[0-9])*");
			CommentTerminal comentario_linea = new CommentTerminal("comentario_linea", "#", "\n", "\r\n");
			CommentTerminal comentario_bloque = new CommentTerminal("comentario_bloque", "#*", "*#");

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
			var puntoycoma = ToTerm(";");
			var coma = ToTerm(",");
			var dospuntos = ToTerm(":");
			var punto = ToTerm(".");
			//palabras reservadas
			var pr_crear = ToTerm("CREAR");
			var pr_db = ToTerm("BASE_DATOS");
			var pr_tabla = ToTerm("TABLA");
			var pr_text = ToTerm("Text");
			var pr_integer = ToTerm("Integer");
			var pr_double = ToTerm("Double");
			var pr_bool = ToTerm("Bool");
			var pr_date = ToTerm("Date");
			var pr_datetime = ToTerm("DateTime");
			var pr_nulo = ToTerm("Nulo");
			var pr_noNulo = ToTerm("No Nulo");
			var pr_autoincrementable = ToTerm("Autoincrementable");
			var pr_llavePrimaria = ToTerm("Llave_Primaria");
			var pr_llaveForanea = ToTerm("Llave_Foranea");
			var pr_unico = ToTerm("Unico");
			var pr_objeto = ToTerm("OBJETO");
			var pr_proc = ToTerm("PROCEDIMIENTO");
			var pr_funcion = ToTerm("FUNCION");
			var pr_retorno = ToTerm("RETORNO");
			var pr_usuario = ToTerm("USUARIO");
			var pr_colocar = ToTerm("COLOCAR");
			var pr_password = ToTerm("password");
			var pr_usar = ToTerm("USAR");
			var pr_alterar = ToTerm("ALTERAR");
			var pr_agregar = ToTerm("AGREGAR");
			var pr_quitar = ToTerm("QUITAR");
			var pr_cambiar = ToTerm("CAMBIAR");
			var pr_eliminar = ToTerm("ELIMINAR");

			var pr_insertar = ToTerm("INSERTAR");
			var pr_en = ToTerm("EN");
			var pr_actualizar = ToTerm("ACTUALIZAR");
			var pr_valores = ToTerm("VALORES");
			var pr_donde = ToTerm("DONDE");
			var pr_borrar = ToTerm("BORRAR");
			var pr_seleccionar = ToTerm("SELECCIONAR");
			var pr_ordenarPor = ToTerm("ORDENAR_POR");
			var pr_asc = ToTerm("ASC");
			var pr_desc = ToTerm("DESC");
			var pr_otorgar = ToTerm("OTORGAR");
			var pr_permisos = ToTerm("PERMISOS");
			var pr_denegar = ToTerm("DENEGAR");
			var pr_de = ToTerm("DE");

			var pr_declarar = ToTerm("DECLARAR");
			var pr_if = ToTerm("SI");
			var pr_else = ToTerm("SINO");
			var pr_switch = ToTerm("SELECCIONA");
			var pr_case = ToTerm("CASO");
			var pr_default = ToTerm("DEFECTO");
			var pr_for = ToTerm("PARA");
			var pr_while = ToTerm("MIENTRAS");
			var pr_break = ToTerm("DETENER");
			var pr_backup = ToTerm("BACKUP");
			var pr_usqldump = ToTerm("USQLDUMP");
			var pr_completo = ToTerm("COMPLETO");
			var pr_restaurar = ToTerm("RESTAURAR");
			var pr_contar = ToTerm("CONTAR");
			#endregion

			#region NoTerm
			NonTerminal INICIO = new NonTerminal("INICIO");

			NonTerminal EXPRESION = new NonTerminal("EXPRESION"),
				VALOR = new NonTerminal("VALOR"),
				LLAMADAFUNCION = new NonTerminal("LLAMADAFUNCION"),
				LISTAEXPRESIONES = new NonTerminal("LISTAEXPRESIONES"),
				LISTAACCESO = new NonTerminal("LISTACCESO");

			NonTerminal SENTENCIAS = new NonTerminal("SENTENCIAS"),
				SENTENCIA = new NonTerminal("SENTENCIA"),
				CREAR_DB = new NonTerminal("CREAR_DB"),
				CREAR_TABLA = new NonTerminal("CREAR_TABLA"),
				LISTACAMPOSTABLA = new NonTerminal("LISTACAMPOSTABLA"),
				CAMPOTABLA = new NonTerminal("CAMPOTABLA"),
				TIPODATO = new NonTerminal("TIPODATO"),
				COMPLEMENTOCAMPOTABLA = new NonTerminal("COMPLEMENTOCAMPOTABLA"),
				LISTACOMPLEMENTOSCT = new NonTerminal("LISTACOMPLEMENTOSCT"),
				CREAR_OBJETO = new NonTerminal("CREAR_OBJETO"),
				CREAR_PROC = new NonTerminal("CREAR_PROC"),
				CREAR_FUNCION = new NonTerminal("CREAR_FUNCION"),
				RETORNO = new NonTerminal("RETORNO"),
				CREAR_USUARIO = new NonTerminal("CREAR_USUARIO"),
				USAR_DB = new NonTerminal("USAR_DB"),
				LISTAATRIBUTOS = new NonTerminal("LISTAATRIBUTOS"),
				ATRIBUTOOBJETO = new NonTerminal("ATRIBUTOOBJETO"),
				BLOQUESENTENCIAS = new NonTerminal("BLOQUESENTENCIAS"),
				SENTENCIABLOQUE = new NonTerminal("SENTENCIABLOQUE"),
				LISTAPARAMETROS = new NonTerminal("LISTAPARAMETROS"),
				PARAMETRO = new NonTerminal("PARAMETRO"),
				ALTERAR_TABLA = new NonTerminal("ALTERAR_TABLA"),
				ALTERAR_USUARIO = new NonTerminal("ALTERAR_USUARIO"),
				ALTERAR_OBJETO = new NonTerminal("ALTERAR_OBJETO"),
				LISTANOMBRES = new NonTerminal("LISTANOMBRES"),
				ELIMINARTABLA = new NonTerminal("ELIMINARTABLA"),
				ELIMINARUSUARIO = new NonTerminal("ELIMINARUSUARIO"),
				ELIMINAROBJETO = new NonTerminal("ELIMINAROBJETO"),
				ELIMINARDB = new NonTerminal("ELIMINARDB")
				;

			NonTerminal SELECCIONAR = new NonTerminal("SELECCIONAR"),
				BORRAR = new NonTerminal("BORRAR"),
				ACTUALIZAR = new NonTerminal("ACTUALIZAR"),
				INSERTAR = new NonTerminal("INSERTAR"),
				PROPIEDADSELECCIONAR = new NonTerminal("PROPIEDADSELECCIONAR"),
				PROPIEDADDONDE = new NonTerminal("PROPIEDADDONDE"),
				PROPIEDADORDENAR = new NonTerminal("PROPIEDADORDENAR"),
				ASCDESC = new NonTerminal("ASCDESC")
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
				WHILE = new NonTerminal("WHILE"), //uimprimir y fecha y fechahora
				CONTAR = new NonTerminal("CONTAR"),
				BACKUP = new NonTerminal("BACKUP"),
				RESTAURAR = new NonTerminal("RESTAURAR")	
				;

			#endregion

			#region Gramatica
			INICIO.Rule = SENTENCIAS;

			#region expresion
			EXPRESION.Rule = //aritmeticos
				EXPRESION + mas + EXPRESION
				| EXPRESION + menos + EXPRESION
				| EXPRESION + por + EXPRESION
				| EXPRESION + div + EXPRESION
				| EXPRESION + pot + EXPRESION
				//logicos
				| EXPRESION + mayor + EXPRESION
				| EXPRESION + menor + EXPRESION
				| EXPRESION + mayorigual + EXPRESION
				| EXPRESION + menorigual + EXPRESION
				| EXPRESION + igualigual + EXPRESION
				| EXPRESION + notigual + EXPRESION
				| EXPRESION + and + EXPRESION
				| EXPRESION + or + EXPRESION
				//otros
				| par1 + EXPRESION + par2
				| VALOR;

			VALOR.Rule = cadena
				| decimal_
				| entero
				| id
				|nombre
				|date
				|datetime
				|nombre+punto+LISTAACCESO
				| menos + EXPRESION
				| not + EXPRESION
				| LLAMADAFUNCION;

			LLAMADAFUNCION.Rule = nombre + par1 + LISTAEXPRESIONES + par2
				|CONTAR;

			LISTANOMBRES.Rule = MakePlusRule(LISTANOMBRES, coma, nombre);

			LISTAEXPRESIONES.Rule = MakeStarRule(LISTAEXPRESIONES, coma, EXPRESION);

			LISTAACCESO.Rule =MakePlusRule(LISTAACCESO,punto,nombre);

			#endregion

			#region sentencias DDL

			SENTENCIAS.Rule = MakeStarRule(SENTENCIAS, SENTENCIA);

			SENTENCIA.Rule = CREAR_DB
				| CREAR_TABLA
				| CREAR_OBJETO
				| CREAR_PROC
				| CREAR_FUNCION
				| CREAR_USUARIO
				| USAR_DB
				| ALTERAR_TABLA
				| ALTERAR_USUARIO
				| ALTERAR_OBJETO
				| ELIMINARDB
				| ELIMINAROBJETO
				| ELIMINARTABLA
				| ELIMINARUSUARIO
				| INSERTAR
				| ACTUALIZAR
				| BORRAR
				| SELECCIONAR + puntoycoma
				| OTORGAR
				| DENEGAR
				| DECLARACION
				| ASIGNACION
				| IF
				| BACKUP
				| RESTAURAR
				| WHILE
				| FOR
				| SWITCH
				|LLAMADAFUNCION+puntoycoma
				|BREAK
				;

			CREAR_DB.Rule = pr_crear + pr_db + nombre + puntoycoma;

			CREAR_TABLA.Rule = pr_crear + pr_tabla + nombre + par1 + LISTACAMPOSTABLA + par2 + puntoycoma;

			LISTACAMPOSTABLA.Rule = MakePlusRule(LISTACAMPOSTABLA, coma, CAMPOTABLA);

			CAMPOTABLA.Rule = TIPODATO + nombre + LISTACOMPLEMENTOSCT
				| TIPODATO + nombre + pr_llaveForanea + nombre;

			TIPODATO.Rule = pr_text
				| pr_integer
				| pr_double
				| pr_bool
				| pr_date
				| pr_datetime
				| nombre;

			LISTACOMPLEMENTOSCT.Rule = MakeStarRule(LISTACOMPLEMENTOSCT, COMPLEMENTOCAMPOTABLA);

			COMPLEMENTOCAMPOTABLA.Rule = pr_nulo
				| pr_noNulo
				| pr_autoincrementable
				| pr_llavePrimaria
				| pr_unico;

			CREAR_OBJETO.Rule = pr_crear + pr_objeto + nombre + par1 + LISTAATRIBUTOS + par2 + puntoycoma;

			LISTAATRIBUTOS.Rule = MakePlusRule(LISTAATRIBUTOS, coma, ATRIBUTOOBJETO);

			ATRIBUTOOBJETO.Rule = TIPODATO + nombre;

			CREAR_PROC.Rule = pr_crear + pr_proc + nombre + par1 + LISTAPARAMETROS + par2 + llave1+BLOQUESENTENCIAS+llave2;

			LISTAPARAMETROS.Rule =MakeStarRule(LISTAPARAMETROS,coma,PARAMETRO);

			PARAMETRO.Rule =TIPODATO+id;

			CREAR_FUNCION.Rule =pr_crear+pr_funcion+nombre+par1+LISTAPARAMETROS+par2+TIPODATO+llave1+BLOQUESENTENCIAS+llave2;

			RETORNO.Rule =pr_retorno+EXPRESION+puntoycoma;

			CREAR_USUARIO.Rule =pr_crear+pr_usuario+nombre+pr_colocar+pr_password+igual+cadena+puntoycoma;

			USAR_DB.Rule =pr_usar+nombre+puntoycoma;

			ALTERAR_TABLA.Rule =pr_alterar+pr_tabla+nombre+pr_agregar+par1+LISTACAMPOSTABLA +par2+puntoycoma
				| pr_alterar + pr_tabla + nombre + pr_quitar + LISTANOMBRES+puntoycoma;

			ALTERAR_OBJETO.Rule= pr_alterar + pr_objeto + nombre + pr_agregar + par1 + LISTAATRIBUTOS + par2 + puntoycoma
				| pr_alterar + pr_objeto + nombre + pr_quitar + LISTANOMBRES+puntoycoma;

			ALTERAR_USUARIO.Rule=pr_alterar+pr_usuario+nombre+pr_cambiar+pr_password+igual+cadena+puntoycoma;

			ELIMINARTABLA.Rule =pr_eliminar+ pr_tabla + nombre+puntoycoma;

			ELIMINARDB.Rule = pr_eliminar + pr_db + nombre + puntoycoma;

			ELIMINAROBJETO.Rule = pr_eliminar + pr_objeto + nombre + puntoycoma;

			ELIMINARUSUARIO.Rule = pr_eliminar + pr_usuario + nombre + puntoycoma;


			#endregion

			#region sentencias DML?

			INSERTAR.Rule =pr_insertar+pr_en+pr_tabla+nombre +par1 + LISTAEXPRESIONES+par2+puntoycoma
				|pr_insertar+pr_en+pr_tabla+nombre+par1+LISTAEXPRESIONES+par2+pr_valores+par1+LISTAEXPRESIONES+par2+puntoycoma;


			ACTUALIZAR.Rule =pr_actualizar+pr_tabla+nombre+par1 + LISTANOMBRES + par2 + pr_valores + par1 + LISTAEXPRESIONES + par2 + puntoycoma
				| pr_actualizar + pr_tabla + nombre + par1 + LISTANOMBRES + par2 + pr_valores + par1 + LISTAEXPRESIONES + par2+pr_donde+ EXPRESION+ puntoycoma;

			BORRAR.Rule =pr_borrar+pr_en+pr_tabla+nombre+pr_donde+EXPRESION+puntoycoma
				|pr_borrar + pr_en + pr_tabla + nombre +puntoycoma;

			SELECCIONAR.Rule =pr_seleccionar+LISTANOMBRES+pr_de+LISTANOMBRES+PROPIEDADSELECCIONAR
				| pr_seleccionar + por + pr_de + LISTANOMBRES + PROPIEDADSELECCIONAR;

			PROPIEDADSELECCIONAR.Rule = PROPIEDADDONDE + PROPIEDADORDENAR
				| PROPIEDADORDENAR + PROPIEDADDONDE
				| PROPIEDADDONDE
				| PROPIEDADORDENAR
				|Empty;

			PROPIEDADDONDE.Rule = pr_donde + EXPRESION;

			PROPIEDADORDENAR.Rule = pr_ordenarPor + nombre + ASCDESC;

			ASCDESC.Rule =pr_asc|pr_desc|Empty;

			//sentencias ssl
			OTORGAR.Rule =pr_otorgar+pr_permisos+nombre+coma+nombre+punto+nombre+puntoycoma
				| pr_otorgar + pr_permisos + nombre + coma + nombre + punto +por+ puntoycoma;

			DENEGAR.Rule = pr_denegar + pr_permisos + nombre + coma + nombre + punto + nombre + puntoycoma
				| pr_denegar + pr_permisos + nombre + coma + nombre + punto + por + puntoycoma;

			BLOQUESENTENCIAS.Rule = MakeStarRule(BLOQUESENTENCIAS, SENTENCIABLOQUE);

			SENTENCIABLOQUE.Rule = RETORNO
				| SENTENCIA;

			DECLARACION.Rule = pr_declarar + LISTAVARIABLES + TIPODATO + puntoycoma
				| pr_declarar + LISTAVARIABLES + TIPODATO + igual + EXPRESION + puntoycoma;
				
			LISTAVARIABLES.Rule =MakePlusRule(LISTAVARIABLES,coma,id);

			ASIGNACION.Rule = id + igual + EXPRESION + puntoycoma
				| id + punto + LISTAACCESO +igual + EXPRESION + puntoycoma;

			IF.Rule = pr_if + par1 + EXPRESION + par2 + llave1 + BLOQUESENTENCIAS + llave2
				| pr_if + par1 + EXPRESION + par2 + llave1 + BLOQUESENTENCIAS + llave2 + pr_else + llave1 + BLOQUESENTENCIAS + llave2;

			SWITCH.Rule = pr_switch + par1 + EXPRESION + par2 + llave1 + LISTACASE + DEFAULT + llave2
				| pr_switch + par1 + EXPRESION + par2 + llave1 + LISTACASE + llave2;

			LISTACASE.Rule = MakePlusRule(LISTACASE,CASE);

			CASE.Rule = pr_case + EXPRESION + dospuntos + BLOQUESENTENCIAS;

			DEFAULT.Rule = pr_default + dospuntos + BLOQUESENTENCIAS;

			BREAK.Rule = pr_break + puntoycoma;

			FOR.Rule = pr_for + par1 + pr_declarar + id + igual + EXPRESION + puntoycoma + EXPRESION + puntoycoma + OPPFOR + par2 + llave1 + BLOQUESENTENCIAS + llave2;

			OPPFOR.Rule = mas + mas
				| menos + menos;

			WHILE.Rule =pr_while+par1+EXPRESION+par2+llave1+BLOQUESENTENCIAS+llave2;

			CONTAR.Rule = pr_contar + par1  +SELECCIONAR + par2;

			BACKUP.Rule = pr_backup + pr_usqldump + nombre + nombre + puntoycoma
				| pr_backup + pr_completo + nombre + nombre + puntoycoma;

			RESTAURAR.Rule = pr_restaurar + pr_usqldump + cadena + puntoycoma
				| pr_restaurar + pr_completo + cadena + puntoycoma;


			#endregion
			//revisar lista de acceso a objetos basado en si existen objetos con objetos dentro y si son accesibles
			//revisar contar con seleccionar con expresion al final y >>
			#endregion

			#region Ajustes
			//NO TERMINAL DE INICIO
			this.Root = INICIO;
			//PALABRAS RESERVADAS
			MarkReservedWords(pr_crear.ToString(),pr_db.ToString(),pr_tabla.ToString(),pr_text.ToString(),pr_integer.ToString(),pr_double.ToString(),pr_bool.ToString(),
				pr_date.ToString(),pr_datetime.ToString(),pr_nulo.ToString(),pr_noNulo.ToString(),pr_autoincrementable.ToString(),pr_llavePrimaria.ToString(),
				pr_llaveForanea.ToString(),pr_unico.ToString(),pr_objeto.ToString(),pr_proc.ToString(),pr_funcion.ToString(),pr_retorno.ToString(),pr_usuario.ToString(),
				pr_colocar.ToString(),pr_password.ToString(),pr_usar.ToString(),pr_alterar.ToString(),pr_agregar.ToString(),pr_quitar.ToString(),pr_cambiar.ToString(),
				pr_insertar.ToString(),pr_en.ToString(),pr_actualizar.ToString(),pr_valores.ToString(),pr_donde.ToString(),pr_borrar.ToString(),pr_seleccionar.ToString(),
				pr_ordenarPor.ToString(),pr_asc.ToString(),pr_desc.ToString(),pr_otorgar.ToString(),pr_permisos.ToString(),pr_denegar.ToString(),pr_de.ToString(),
				pr_declarar.ToString(),pr_if.ToString(),pr_else.ToString(),pr_switch.ToString(),pr_case.ToString(),pr_default.ToString(),pr_for.ToString(),pr_while.ToString(),
				pr_break.ToString(),pr_backup.ToString(),pr_usqldump.ToString(),pr_completo.ToString(),pr_restaurar.ToString(),pr_contar.ToString());
			//NODOS A OMITIR
			MarkTransient(SENTENCIA,ASCDESC);
			//TERMINALES IGNORADO
			MarkPunctuation(par1,par2,coma,puntoycoma,igual,llave1,llave2,punto,dospuntos,
				pr_crear,pr_db,pr_eliminar,pr_usuario,pr_colocar,pr_password,pr_objeto,pr_tabla,pr_alterar,pr_cambiar, pr_usar,pr_proc,pr_funcion,pr_insertar,pr_en,
				pr_valores,pr_actualizar,pr_donde,pr_seleccionar,pr_de,pr_ordenarPor,pr_otorgar,pr_permisos,pr_denegar,pr_declarar,pr_if,pr_switch,pr_for,pr_while,
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
 