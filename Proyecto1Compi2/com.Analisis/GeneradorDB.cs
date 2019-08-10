using com.Analisis;
using com.Analisis.Util;
using Irony.Parsing;
using Proyecto1Compi2.com.AST;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Analisis
{
	class GeneradorDB
	{
		public static void GuardarInformación(ParseTreeNode raiz)
		{
			//raiz
			//databases=0-- users=1
			GuardarBasesDeDatos(raiz.ChildNodes.ElementAt(0));
			GuardarUsuarios(raiz.ChildNodes.ElementAt(1));
		}

		private static void GuardarBasesDeDatos(ParseTreeNode raiz)
		{
			foreach (ParseTreeNode nodo in raiz.ChildNodes)
			{
				
				BaseDatos db = GetBaseDatos(nodo);
				//validar que las bases de datos existan y luego agregar
				if (!Analizador.ExisteDB(db.Nombre))
				{
					Analizador.BasesDeDatos.Add(db);
				}
				else {
					Console.WriteLine("ERROR YA EXISTE LA BASE DE DATOS");
				}
			}
		}

		private static BaseDatos GetBaseDatos(ParseTreeNode nodo)
		{
			List<object> objetosdb = GetObjetosDb(nodo.ChildNodes.ElementAt(1));
			return new BaseDatos(nodo.ChildNodes.ElementAt(0).Token.ValueString, objetosdb) ;
		}

		private static List<object> GetObjetosDb(ParseTreeNode raiz)
		{
			List<object> objetosdb=new List<object>();
			foreach (ParseTreeNode nodo in raiz.ChildNodes)
			{
				switch (nodo.Term.Name) {
					case "TABLA":
						Tabla tb = GetTabla(nodo);
						tb.MostrarCabecera();
						//tb.MostrarDatos();
						objetosdb.Add(tb);
						break;
					case "OBJETO":
						UserType obj = GetObjeto(nodo);
						//obj.Mostrar();
						objetosdb.Add(obj);
						break;
					case "PROCEDIMIENTO":
						Procedimiento proc = GetProcedimiento(nodo);
						//proc.Mostrar();
						objetosdb.Add(proc);
						break;
				}

			}
			return objetosdb;
		}

		private static Procedimiento GetProcedimiento(ParseTreeNode nodo)
		{
			Dictionary<string, TipoDatoDB> par = GetParametros(nodo.ChildNodes.ElementAt(1));
			Dictionary<string, TipoDatoDB> ret = GeRetornos(nodo.ChildNodes.ElementAt(1));
			String codigo = nodo.ChildNodes.ElementAt(2).Token.ValueString;
			codigo=codigo.TrimStart('$');
			codigo=codigo.TrimEnd('$');
			Analizador.AnalizarCql(codigo);
			List<Error> erroresInst = (Analizador.ErroresCQL);
			//cambiar el numero de linea 
			Analizador.ErroresCHISON.AddRange(erroresInst);
			
			return new Procedimiento(nodo.ChildNodes.ElementAt(0).Token.ValueString, par,ret,null);
		}

		private static Dictionary<string, TipoDatoDB> GeRetornos(ParseTreeNode parseTreeNode)
		{
			Dictionary<string, TipoDatoDB> ret = new Dictionary<string, TipoDatoDB>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
			{
				if (nodo.ChildNodes.ElementAt(2).Token.ValueString.ToLower().Equals("out"))
				{
					try
					{
						ret.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString, GetTipo(nodo.ChildNodes.ElementAt(1)));
					}
					catch (ArgumentException ex)
					{
						Console.WriteLine("ERROR LEYENDO DATOS DE PARAMETROS DE PROCEDIMIENTO");
					}
				}
			}
			return ret;
		}

		private static Dictionary<string, TipoDatoDB> GetParametros(ParseTreeNode parseTreeNode)
		{
			Dictionary<string, TipoDatoDB> param = new Dictionary<string, TipoDatoDB>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes) {
				if (nodo.ChildNodes.ElementAt(2).Token.ValueString.ToLower().Equals("in")) {
					try {
						param.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString, GetTipo(nodo.ChildNodes.ElementAt(1)));
					}
					catch (ArgumentException ex)
					{
						Console.WriteLine("ERROR LEYENDO DATOS DE PARAMETROS DE PROCEDIMIENTO");
					}
				}
			}
			return param;
		}

		private static UserType GetObjeto(ParseTreeNode nodo)
		{
			return new UserType(nodo.ChildNodes.ElementAt(0).Token.ValueString, 
				GetAtributos(nodo.ChildNodes.ElementAt(1)));
		}

		private static Dictionary<string, TipoDatoDB> GetAtributos(ParseTreeNode parseTreeNode)
		{
			Dictionary<string, TipoDatoDB> atributos = new Dictionary<string, TipoDatoDB>();
			foreach (ParseTreeNode nodo in parseTreeNode.ChildNodes)
			{
				try {
					atributos.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString, GetTipo(nodo.ChildNodes.ElementAt(1)));
				}
				catch (ArgumentException ex)
				{
					Console.WriteLine("ERROR YA EXISTE UN ATRIBUTO CON ESE NOMBRE");
				}
			}
			return atributos;
		}

		private static Tabla GetTabla(ParseTreeNode nodo)
		{
			List<Columna> columnas = GetColumnas(nodo.ChildNodes.ElementAt(1));
			Tabla tb = new Tabla(nodo.ChildNodes.ElementAt(0).Token.ValueString);//nombre como parámetro
			//asignando columnas
			tb.Columnas = columnas;
			//agregando datos a la tabla
			AddDataTabla(tb,nodo.ChildNodes.ElementAt(2));
			return tb;
		}

		private static void AddDataTabla(Tabla tb,ParseTreeNode raiz)
		{
			foreach (ParseTreeNode nodo in raiz.ChildNodes)
			{
				Dictionary<string, string> fila = GetFila(nodo);
				if (fila != null)
				{
					try
					{
						if (fila.Count == tb.Columnas.Count)
						{
							bool bandera = true;
							List<object> valores = new List<object>();
							foreach (Columna cl in tb.Columnas) {
								if (!fila.ContainsKey(cl.Nombre))
								{
									bandera = false;
								}
								else {
									if (IsTipoCompatible(cl.Tipo, fila[cl.Nombre]))
									{
										valores.Add(fila[cl.Nombre]);
									}
									else {
										bandera = false;
									}
								}
							}
							if (bandera) {
									tb.AgregarFila(valores);
							}
						}
						else
						{
							Console.WriteLine("ERROR CANTIDAD DE COLUMNAS NO CONCUERDA CON DATOS DE FILA");
						}
					}
					catch (KeyNotFoundException ex)
					{
						Console.WriteLine("ERROR COLUMNA EN DATOS NO EXISTE EN BASE DE DATOS");
					}

				}
			}
		}

		private static bool IsTipoCompatible(TipoDatoDB tipo, string v)
		{
			switch (tipo) {
				case TipoDatoDB.BOOLEAN:
					return bool.TryParse(v, out bool res);
				case TipoDatoDB.COUNTER:
					return int.TryParse(v, out int r1)|| double.TryParse(v, out double r1_1);
				case TipoDatoDB.DATE:
					return Regex.IsMatch(v, "'[0-9]{4}-[0-9]{2}-[0-9]{2}'");
				case TipoDatoDB.DOUBLE:
					return double.TryParse(v, out double r3);
				case TipoDatoDB.INT:
					return int.TryParse(v, out int r4);
				case TipoDatoDB.NULO:
					return v.ToLower().Equals("null");
				//	case TipoDatoDB.OBJETO:
				//		return bool.TryParse(v, out bool r6);
				case TipoDatoDB.STRING:
					return true;
				case TipoDatoDB.TIME:
					return Regex.IsMatch(v, "'[0-9]{2}:[0-9]{2}:[0-9]{2}'");
				default:
					return false;
			}
		}

		private static Dictionary<string, string> GetFila(ParseTreeNode fila)
		{
			Dictionary<string, string> datos= new Dictionary<string, string>();
			try {
				foreach (ParseTreeNode nod in fila.ChildNodes)
				{
					datos.Add(nod.ChildNodes.ElementAt(0).Token.ValueString, nod.ChildNodes.ElementAt(1).Token.ValueString);
				}
			}
			catch (ArgumentException ex) {
				Console.WriteLine("ERROR LEYENDO DATOS DE FILAS DE TABLA DE BASE DE DATOS");
				return null;
			}
			return datos;
		}

		private static List<Columna> GetColumnas(ParseTreeNode raiz)
		{
			List<Columna> columnas = new List<Columna>();
			foreach (ParseTreeNode nodo in raiz.ChildNodes) {
				Columna cl = GetColumna(nodo);
				columnas.Add(cl);
			}

			return columnas;
		}

		private static Columna GetColumna(ParseTreeNode nodo)
		{
			return new Columna(nodo.ChildNodes.ElementAt(0).Token.ValueString, GetTipo(nodo.ChildNodes.ElementAt(1)), IsPk(nodo.ChildNodes.ElementAt(2)));
		}

		private static bool IsPk(ParseTreeNode parseTreeNode)
		{
			if (bool.TryParse(parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString, out bool res)) {
				return res;
			}
			return false;
		}

		private static TipoDatoDB GetTipo(ParseTreeNode parseTreeNode)
		{

			switch (parseTreeNode.ChildNodes.ElementAt(0).Token.ValueString.ToLower()) {
				case "string":
					return TipoDatoDB.STRING;
				case "int":
					return TipoDatoDB.INT;
				case "double":
					return TipoDatoDB.DOUBLE;
				case "boolean":
					return TipoDatoDB.BOOLEAN;
				case "date":
					return TipoDatoDB.DATE;
				case "time":
					return TipoDatoDB.TIME;
				case "counter":
					return TipoDatoDB.COUNTER;
				default:
					return TipoDatoDB.OBJETO;
			}
		}

		private static void GuardarUsuarios(ParseTreeNode lista_usuarios)
		{
			foreach (ParseTreeNode usuario in lista_usuarios.ChildNodes) {
				Usuario usu=GetUsuario(usuario);
				if (!Analizador.ExisteUsuario(usu.Nombre))
				{
					Analizador.Usuariosdb.Add(usu);
				}
				else {
					Console.WriteLine("EL USUARIO YA EXISTE");
				}
			}
		}

		private static Usuario GetUsuario(ParseTreeNode raiz)
		{
			List<string> permisos = GetListaOtroNombre(raiz.ChildNodes.ElementAt(2));
			foreach (string per in permisos) {
				if (!Analizador.ExisteDB(per)) {
					permisos.Remove(per);
					Console.WriteLine("ERROR NO EXISTE LA BASE DE DATOS");
				}
			}
			return new Usuario(raiz.ChildNodes.ElementAt(0).Token.ValueString,
				raiz.ChildNodes.ElementAt(1).Token.ValueString,permisos);
		}

		private static List<string> GetListaOtroNombre(ParseTreeNode lista_nombres)
		{
			List<string> nombres = new List<string>();
			foreach (ParseTreeNode nodo in lista_nombres.ChildNodes) {
				nombres.Add(nodo.ChildNodes.ElementAt(0).Token.ValueString);
			}
			return nombres;
		}
	}
}
