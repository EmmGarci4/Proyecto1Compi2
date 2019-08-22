using Proyecto1Compi2.com.AST;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Proyecto1Compi2.com.AST.Operacion;

namespace Proyecto1Compi2.com.Util
{
	static class Datos
	{
		public static bool IsTipoCompatible(TipoObjetoDB tipo, object v)
		{
			switch (tipo.Tipo)
			{
				case TipoDatoDB.BOOLEAN:
					return v.GetType() == typeof(bool);
				case TipoDatoDB.COUNTER:
					return v.GetType() == typeof(double) || v.GetType() == typeof(int);
				case TipoDatoDB.DOUBLE:
					return v.GetType() == typeof(double);
				case TipoDatoDB.INT:
					return v.GetType() == typeof(int);
				case TipoDatoDB.DATE:
					if (v.GetType() == typeof(string))
					{
						return Regex.IsMatch(v.ToString(), "'[0-9]{4}-[0-9]{2}-[0-9]{2}'");
					}
					return false;
				case TipoDatoDB.NULO:
					if (v.GetType() == typeof(string))
					{
						return v.ToString().ToLower().Equals("null");
					}
					return false;
				case TipoDatoDB.STRING:
					return true;
				case TipoDatoDB.TIME:
					if (v.GetType() == typeof(string))
					{
						return Regex.IsMatch(v.ToString(), "'[0-9]{2}:[0-9]{2}:[0-9]{2}'");
					}
					return false;
				case TipoDatoDB.LISTA_PRIMITIVO:
				case TipoDatoDB.SET_PRIMITIVO:
					if (v.GetType() == typeof(CollectionListCql))
					{
						CollectionListCql list = (CollectionListCql)v;
						if (list.IsLista)
						{
							return list.IsAllBool() ||
								list.IsAllDate() ||
								list.IsAllDouble() ||
								list.IsAllInteger() ||
								list.IsAllObjeto() ||
								list.IsAllString() ||
								list.IsAllTime();
						}
					}
					break;
				case TipoDatoDB.MAP_PRIMITIVO:
				case TipoDatoDB.MAP_OBJETO:
					if (v.GetType() == typeof(CollectionMapCql))
					{
						//comparar tipos
						return true;
					}
					break;
				case TipoDatoDB.OBJETO:
					return true;
			}

			return false;
		}

		internal static bool IsTipoCompatibleParaAsignar(TipoObjetoDB tipoDato, object v)
		{
			switch (tipoDato.Tipo)
			{
				case TipoDatoDB.BOOLEAN:
					return v.GetType() == typeof(bool);
				case TipoDatoDB.COUNTER:
					return v.GetType() == typeof(double) || v.GetType() == typeof(int);
				case TipoDatoDB.DOUBLE:
					return v.GetType() == typeof(double);
				case TipoDatoDB.INT:
					return v.GetType() == typeof(int);
				case TipoDatoDB.DATE:
					if (v.GetType() == typeof(string))
					{
						return Regex.IsMatch(v.ToString(), "'[0-9]{4}-[0-9]{2}-[0-9]{2}'");
					}
					return false;
				case TipoDatoDB.NULO:
					if (v.GetType() == typeof(string))
					{
						return v.ToString().ToLower().Equals("null");
					}
					return false;
				case TipoDatoDB.STRING:
					return true;
				case TipoDatoDB.TIME:
					if (v.GetType() == typeof(string))
					{
						return Regex.IsMatch(v.ToString(), "'[0-9]{2}:[0-9]{2}:[0-9]{2}'");
					}
					return false;
				case TipoDatoDB.LISTA_PRIMITIVO:
				case TipoDatoDB.SET_PRIMITIVO:
					if (v.GetType() == typeof(CollectionListCql))
					{
						return true;
					}
					break;
				case TipoDatoDB.MAP_PRIMITIVO:
				case TipoDatoDB.MAP_OBJETO:
					if (v.GetType() == typeof(CollectionMapCql))
					{
						return true;
					}
					break;
				case TipoDatoDB.OBJETO:
					return true;
			}
			return false;
		}

		internal static TipoObjetoDB GetTipoObjetoDB(object respuesta)
		{
			//entero o decimal
			if (respuesta.GetType() == typeof(double))
			{
				if (respuesta.ToString().Contains("."))
				{
					return new TipoObjetoDB(TipoDatoDB.DOUBLE, "double");
				}
				else {
					return new TipoObjetoDB(TipoDatoDB.INT, "int");
				}
			} else if (respuesta.GetType() == typeof(string)) {
				return new TipoObjetoDB(TipoDatoDB.STRING, "string");
			}
			else if (respuesta.GetType() == typeof(MyDateTime))
			{
				MyDateTime dt = (MyDateTime)respuesta;
				if (dt.Tipo == TipoDatoDB.TIME)
				{
					return new TipoObjetoDB(TipoDatoDB.TIME, "time");
				}
				else {
					return new TipoObjetoDB(TipoDatoDB.DATE, "hour");
				}
			}else if (respuesta.GetType() == typeof(CollectionListCql))
			{
				CollectionListCql list = (CollectionListCql)respuesta;
				if (IsPrimitivo(GetTipoDatoDB(list.TipoDato.Tipo)))
				{
					if (list.IsLista) return new TipoObjetoDB(TipoDatoDB.LISTA_PRIMITIVO, "list<"+list.TipoDato.ToString()+">");
					return new TipoObjetoDB(TipoDatoDB.SET_PRIMITIVO, "set<"+list.TipoDato.ToString()+">");
				}
				else {
					if (list.IsLista) return new TipoObjetoDB(TipoDatoDB.LISTA_OBJETO, "list<"+list.TipoDato.ToString()+">");
					return new TipoObjetoDB(TipoDatoDB.SET_OBJETO, "set<"+list.TipoDato.ToString()+">");
				}
			}
			else if (respuesta.GetType() == typeof(CollectionMapCql))
			{
				CollectionMapCql list = (CollectionMapCql)respuesta;
				if (IsPrimitivo(GetTipoDatoDB(list.TipoValor.Tipo)))
				{
					return new TipoObjetoDB(TipoDatoDB.MAP_PRIMITIVO, "map<"+list.TipoLlave.ToString()+","+list.TipoValor.ToString()+">");

				}
				else {
					return new TipoObjetoDB(TipoDatoDB.MAP_OBJETO, "map<"+list.TipoLlave.ToString()+","+list.TipoValor.ToString()+">");

				}
			}


			return new TipoObjetoDB(TipoDatoDB.NULO, "null");
		}

		internal static TipoObjetoDB GetTipoObjetoDBPorCadena(string nombre)
		{
			switch (nombre)
			{
				case "string":
					return new TipoObjetoDB(TipoDatoDB.STRING,"string");
				case "int":
					return new TipoObjetoDB(TipoDatoDB.INT,"int");
				case "double":
					return new TipoObjetoDB(TipoDatoDB.DOUBLE,"double");
				case "boolean":
					return new TipoObjetoDB(TipoDatoDB.BOOLEAN,"boolean");
				case "date":
					return new TipoObjetoDB(TipoDatoDB.DATE,"date");
				case "time":
					return new TipoObjetoDB(TipoDatoDB.TIME,"time");
				case "counter":
					return new TipoObjetoDB(TipoDatoDB.COUNTER,"counter");
				//listas
				case "list<string>":
				case "list<int>":
				case "list<double>":
				case "list<boolean>":
				case "list<date>":
				case "list<time>":
					return new TipoObjetoDB(TipoDatoDB.LISTA_PRIMITIVO,nombre.Replace("list|<|>",string.Empty));
				//sets
				case "set<string>":
				case "set<int>":
				case "set<double>":
				case "set<boolean>":
				case "set<date>":
				case "set<time>":
					return new TipoObjetoDB(TipoDatoDB.SET_PRIMITIVO, nombre.Replace("´set|<|>", string.Empty));
				//maps
				case "map<string>":
				case "map<int>":
				case "map<double>":
				case "map<boolean>":
				case "map<date>":
				case "map<time>":
					return new TipoObjetoDB(TipoDatoDB.MAP_PRIMITIVO, nombre.Replace("map|<|>", string.Empty));
				default:
					if (nombre.Equals("list") ||
						nombre.StartsWith("list"))
					{
						return new TipoObjetoDB(TipoDatoDB.LISTA_OBJETO, nombre.Replace("list|<|>", string.Empty));
					}
					else if (nombre.Equals("set") ||
						nombre.StartsWith("set"))
					{
						return new TipoObjetoDB(TipoDatoDB.LISTA_OBJETO, nombre.Replace(">", string.Empty));
					}
					else if (nombre.Equals("map") ||
						nombre.StartsWith("map"))
					{
						return new TipoObjetoDB(TipoDatoDB.LISTA_OBJETO, nombre.Replace("map|<|>", string.Empty));
					}
					else {
						return new TipoObjetoDB(TipoDatoDB.OBJETO,nombre);
					}

			}
		}

		public static object GetValor(string valueString)
		{
			if (valueString.Contains("."))
			{
				//decimal
				if (double.TryParse(valueString, out double val1))
				{
					return val1;
				}
				else
				{
					return valueString;
				}
			}
			//entero
			if (int.TryParse(valueString, out int val2))
			{
				return val2;
			}

			//booleano
			else if (bool.TryParse(valueString, out bool val3))
			{
				return val3;
			}
			else
			{
				return valueString;
			}

		}

		public static bool IsLista(TipoDatoDB td)
		{
			switch (td)
			{
				case TipoDatoDB.LISTA_OBJETO:
				case TipoDatoDB.SET_OBJETO:
				case TipoDatoDB.MAP_OBJETO:
				case TipoDatoDB.LISTA_PRIMITIVO:
				case TipoDatoDB.SET_PRIMITIVO:
				case TipoDatoDB.MAP_PRIMITIVO:
					return true;
				case TipoDatoDB.OBJETO:
				case TipoDatoDB.BOOLEAN:
				case TipoDatoDB.DOUBLE:
				case TipoDatoDB.INT:
				case TipoDatoDB.STRING:
				case TipoDatoDB.COUNTER:
				case TipoDatoDB.TIME:
				case TipoDatoDB.DATE:
				case TipoDatoDB.NULO:
					return false;
				default:
					return false;
			}
		}

		public static bool Equivale(TipoObjetoDB tipo, string valor, TipoOperacion tipoClave)
		{
			switch (tipo.Tipo)
			{
				case TipoDatoDB.BOOLEAN:
					if (tipoClave == TipoOperacion.Booleano)
					{
						return true;
					}
					break;
				case TipoDatoDB.DATE:
					if (tipoClave == TipoOperacion.Fecha)
					{
						return true;
					}
					break;
				case TipoDatoDB.DOUBLE:
					if (tipoClave == TipoOperacion.Numero && valor.Contains("."))
					{
						return true;
					}
					break;
				case TipoDatoDB.INT:
					if (tipoClave == TipoOperacion.Numero && !valor.Contains("."))
					{
						return true;
					}
					break;
				case TipoDatoDB.LISTA_OBJETO:
				case TipoDatoDB.LISTA_PRIMITIVO:
				case TipoDatoDB.MAP_OBJETO:
				case TipoDatoDB.MAP_PRIMITIVO:
				case TipoDatoDB.OBJETO:
				case TipoDatoDB.SET_OBJETO:
				case TipoDatoDB.SET_PRIMITIVO:
					if (tipoClave == TipoOperacion.Objeto)
					{
						return true;
					}
					break;
				case TipoDatoDB.STRING:
					if (tipoClave == TipoOperacion.String)
					{
						return true;
					}
					break;
				case TipoDatoDB.TIME:
					if (tipoClave == TipoOperacion.Hora)
					{
						return true;
					}
					break;
			}
			return false;
		}

		public static bool IsPrimitivo(TipoOperacion tipoClave)
		{
			switch (tipoClave)
			{
				case TipoOperacion.Booleano:
				case TipoOperacion.String:
				case TipoOperacion.Numero:
				case TipoOperacion.Hora:
				case TipoOperacion.Fecha:
					return true;
				default:
					return false;
			}
		}

		public static TipoOperacion GetTipoDatoDB(TipoDatoDB tipo)
		{
			switch (tipo)
			{
				case TipoDatoDB.BOOLEAN:
					return TipoOperacion.Booleano;
				case TipoDatoDB.DATE:
					return TipoOperacion.Fecha;
				case TipoDatoDB.COUNTER:
				case TipoDatoDB.DOUBLE:
				case TipoDatoDB.INT:
					return TipoOperacion.Numero;
				case TipoDatoDB.STRING:
					return TipoOperacion.String;
				case TipoDatoDB.TIME:
					return TipoOperacion.Hora;
				case TipoDatoDB.NULO:
					return TipoOperacion.Nulo;
				default:
					return TipoOperacion.Objeto;
			}
		}

		internal static string GetDate()
		{
			return "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
		}

		internal static string GetTime()
		{
			return "'" + DateTime.Now.ToString("HH:mm:ss") + "'";
		}

		internal static bool IsPrimitivo(TipoDatoDB ti)
		{
			switch (ti) {
				case TipoDatoDB.BOOLEAN:
				case TipoDatoDB.COUNTER:
				case TipoDatoDB.DATE:
				case TipoDatoDB.DOUBLE:
				case TipoDatoDB.INT:
				case TipoDatoDB.STRING:
				case TipoDatoDB.TIME:
					return true;
				case TipoDatoDB.LISTA_OBJETO:
				case TipoDatoDB.LISTA_PRIMITIVO:
				case TipoDatoDB.MAP_OBJETO:
				case TipoDatoDB.MAP_PRIMITIVO:
				case TipoDatoDB.OBJETO:
				case TipoDatoDB.SET_OBJETO:
				case TipoDatoDB.SET_PRIMITIVO:
					return false;
			}
			return false;
		}
	}
}
