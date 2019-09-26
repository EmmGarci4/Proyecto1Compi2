using com.Analisis.Util;
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
		public static bool IsTipoCompatible(TipoObjetoDB tipoDato, object v)
		{
			switch (tipoDato.Tipo)
			{
				case TipoDatoDB.BOOLEAN:
					return v.GetType() == typeof(bool);
				case TipoDatoDB.DOUBLE:
					return v.GetType() == typeof(double);
				case TipoDatoDB.COUNTER:
				case TipoDatoDB.INT:
					return v.GetType() == typeof(int);
				case TipoDatoDB.DATE:
					if (v.GetType() == typeof(MyDateTime))
					{
						return ((MyDateTime)v).Tipo.Equals(TipoDatoDB.DATE);
					}
					return false;
				case TipoDatoDB.NULO:
					if (v.GetType() == typeof(string))
					{
						return v.ToString().ToLower().Equals("null");
					}
					return false;
				case TipoDatoDB.STRING:
					return (v.GetType() == typeof(string));
				case TipoDatoDB.TIME:
					if (v.GetType() == typeof(MyDateTime))
					{
						return ((MyDateTime)v).Tipo.Equals(TipoDatoDB.TIME);
					}
					return false;
				case TipoDatoDB.LISTA_OBJETO:
				case TipoDatoDB.SET_OBJETO:
				case TipoDatoDB.LISTA_PRIMITIVO:
				case TipoDatoDB.SET_PRIMITIVO:
					if (v.GetType() == typeof(CollectionListCql))
					{
						CollectionListCql collection = (CollectionListCql)v;
						if (tipoDato.Equals(collection.TipoDato))
						{
							
							return true;
						}
						else if (tipoDato.Nombre.Equals("null"))
						{
							return true;
						}
					}
					else if ((v.GetType() == typeof(string) && v.ToString().Equals("null")))
					{
						return true;
					}
					else if (tipoDato.Nombre.Equals("null"))
					{
						if (v.GetType() == typeof(List<Expresion>))
						{
							return true;
						}
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
					if (v.GetType() == typeof(Objeto))
					{
						return ((Objeto)v).IsObjetoTipo(tipoDato.Nombre);
					}
					return false;
			}
			return false;
		}

		internal static TipoThrow GetExceptcion(string nombreExeption)
		{
			switch (nombreExeption.ToLower())
			{
				case "bdalreadyexists":
					return TipoThrow.BDAlreadyExists;
				case "typealreadyexists":
					return TipoThrow.TypeAlreadyExists;
				case "typeDontexists":
					return TipoThrow.TypeDontExists;
				case "bddontexists":
					return TipoThrow.BDDontExists;
				case "usebdexception":
					return TipoThrow.UseBDException;
				case "tablealreadyexists":
					return TipoThrow.TableAlreadyExists;
				case "tabledontexists":
					return TipoThrow.TableDontExists;
				case "countertypeexception":
					return TipoThrow.CounterTypeException;
				case "useraleadyexists":
					return TipoThrow.UserAleadyExists;
				case "userdontexists":
					return TipoThrow.UserDontExists;
				case "valuesexception":
					return TipoThrow.ValuesException;
				case "columnexception":
					return TipoThrow.ColumnException;
				case "batchexception":
					return TipoThrow.BatchException;
				case "indexoutexception":
					return TipoThrow.IndexOutException;
				case "arithmeticexception":
					return TipoThrow.ArithmeticException;
				case "nullpointerexception":
					return TipoThrow.NullPointerException;
				case "numerreturnsexception":
					return TipoThrow.NumerReturnsException;
				case "functionalreadyexists":
					return TipoThrow.FunctionAlreadyExists;
				case "procedurealreadyexists":
					return TipoThrow.ProcedureAlreadyExists;
				case "objectalreadyexists":
					return TipoThrow.ObjectAlreadyExists;
				case "exception":
				default:
					return TipoThrow.Exception;
			}
			throw new NotImplementedException();
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
				case TipoDatoDB.INT:
					return v.GetType() == typeof(double) || v.GetType() == typeof(int);
				case TipoDatoDB.DATE:
					if (v.GetType() == typeof(MyDateTime))
					{
						MyDateTime myDate = (MyDateTime)v;
						if (myDate.IsNull) {
							return true;
						}
						return myDate.Tipo.Equals(TipoDatoDB.DATE);
					} else if (v.Equals("null")) {
						return true;
					}
					return false;
				case TipoDatoDB.NULO:
					if (v.GetType() == typeof(string))
					{
						return v.ToString().ToLower().Equals("null");
					}
					else if (v.Equals("null"))
					{
						return true;
					}
					return false;
				case TipoDatoDB.STRING:
					return (v.GetType() == typeof(string));
				case TipoDatoDB.TIME:
					if (v.GetType() == typeof(MyDateTime))
					{
						MyDateTime myDate = (MyDateTime)v;
						if (myDate.IsNull)
						{
							return true;
						}
						return ((MyDateTime)v).Tipo.Equals(TipoDatoDB.TIME);
					}
					else if (v.Equals("null"))
					{
						return true;
					}
					break;
				case TipoDatoDB.SET_OBJETO:
				case TipoDatoDB.SET_PRIMITIVO:
					if (v.GetType() == typeof(CollectionListCql))
					{
						CollectionListCql cl = (CollectionListCql)v;
						if (!cl.IsLista)
						{
							return true;
						}
					}
					else if (v.Equals("null"))
					{
						return true;
					}
					break;
				case TipoDatoDB.LISTA_OBJETO:
				case TipoDatoDB.LISTA_PRIMITIVO:
					if (v.GetType() == typeof(CollectionListCql))
					{
						CollectionListCql cl = (CollectionListCql)v;
						if (cl.IsLista) {
							return true;
						}
					}
					else if (v.Equals("null"))
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
					else if (v.Equals("null"))
					{
						return true;
					}
					break;
				case TipoDatoDB.OBJETO:
					if (v.GetType() == typeof(Objeto))
					{
						return ((Objeto)v).IsObjetoTipo(tipoDato.Nombre);
					}else if (v.Equals("null"))
					{
						return true;
					}
					return false;
				case TipoDatoDB.CURSOR:
					return v.GetType() == typeof(Cursor);
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
				else
				{
					return new TipoObjetoDB(TipoDatoDB.INT, "int");
				}
			}
			//entero
			if (respuesta.GetType() == typeof(int))
			{
				return new TipoObjetoDB(TipoDatoDB.INT, "int");
			}
			//cadena
			else if (respuesta.GetType() == typeof(string))
			{
				if ((string)respuesta == "null") return new TipoObjetoDB(TipoDatoDB.NULO, "null");
				return new TipoObjetoDB(TipoDatoDB.STRING, "string");
			}
			//fecha u hora
			else if (respuesta.GetType() == typeof(MyDateTime))
			{
				MyDateTime dt = (MyDateTime)respuesta;
				if (dt.Tipo == TipoDatoDB.TIME)
				{
					return new TipoObjetoDB(TipoDatoDB.TIME, "time");
				}
				else
				{
					return new TipoObjetoDB(TipoDatoDB.DATE, "date");
				}
			}
			//lista o set
			else if (respuesta.GetType() == typeof(CollectionListCql))
			{
				CollectionListCql list = (CollectionListCql)respuesta;
				return list.TipoDato;
			}
			//map
			else if (respuesta.GetType() == typeof(CollectionMapCql))
			{
				CollectionMapCql list = (CollectionMapCql)respuesta;
				return list.TipoValor;
			}
			else if (respuesta.GetType() == typeof(Objeto))
			{
				return new TipoObjetoDB(TipoDatoDB.OBJETO, ((Objeto)respuesta).Plantilla.Nombre);
			}
			else if (respuesta.GetType() == typeof(bool)) {
				return new TipoObjetoDB(TipoDatoDB.BOOLEAN, "boolean");
			} else if (respuesta.GetType()==typeof(Cursor)) {
				return new TipoObjetoDB(TipoDatoDB.CURSOR,"cursor");
			}

			return new TipoObjetoDB(TipoDatoDB.NULO, "null");
		}

		internal static TipoObjetoDB GetTipoObjetoDBPorCadena(string nombre)
		{
			switch (nombre.ToLower())
			{
				case "string":
					return new TipoObjetoDB(TipoDatoDB.STRING, "string");
				case "int":
					return new TipoObjetoDB(TipoDatoDB.INT, "int");
				case "double":
					return new TipoObjetoDB(TipoDatoDB.DOUBLE, "double");
				case "boolean":
					return new TipoObjetoDB(TipoDatoDB.BOOLEAN, "boolean");
				case "date":
					return new TipoObjetoDB(TipoDatoDB.DATE, "date");
				case "time":
					return new TipoObjetoDB(TipoDatoDB.TIME, "time");
				case "counter":
					return new TipoObjetoDB(TipoDatoDB.COUNTER, "counter");
				//listas
				case "list<string>":
				case "list<int>":
				case "list<double>":
				case "list<boolean>":
				case "list<date>":
				case "list<time>":
					string n = Regex.Replace(nombre, @"^[^<]*", string.Empty);
					n = n.TrimStart('<');
					n = n.TrimEnd('>');
					return new TipoObjetoDB(TipoDatoDB.LISTA_PRIMITIVO, n);
				//sets
				case "set<string>":
				case "set<int>":
				case "set<double>":
				case "set<boolean>":
				case "set<date>":
				case "set<time>":
					n = Regex.Replace(nombre, @"^[^<]*", string.Empty);
					n = n.TrimStart('<');
					n = n.TrimEnd('>');
					return new TipoObjetoDB(TipoDatoDB.SET_PRIMITIVO, n);
				default:
					if (nombre.StartsWith("list"))
					{
						n = Regex.Replace(nombre, @"^[^<]*", string.Empty);
						n = n.TrimStart('<');
						n = n.TrimEnd('>');
						if (n.Contains('<')) {
							n = n + ">";
						}
						return new TipoObjetoDB(TipoDatoDB.LISTA_OBJETO, n);
					}
					else if (nombre.StartsWith("set"))
					{
						n = Regex.Replace(nombre, @"^[^<]*", string.Empty);
						n = n.TrimStart('<');
						n = n.TrimEnd('>');
						if (n.Contains('<'))
						{
							n = n + ">";
						}
						return new TipoObjetoDB(TipoDatoDB.SET_OBJETO, n);
					}
					else if (nombre.StartsWith("map"))
					{
						n = Regex.Replace(nombre, @"^[^<]*", string.Empty);
						n = n.TrimStart('<');
						n = n.TrimEnd('>');
						if (n.Contains('<'))
						{
							n = n + ">";
						}
						string[] tipos = n.Split(',');
						TipoObjetoDB tipoMap = GetTipoObjetoDBPorCadena(tipos[1]);
						if (IsPrimitivo(tipoMap.Tipo))
						{
							return new TipoObjetoDB(TipoDatoDB.MAP_PRIMITIVO, n);
						}
						return new TipoObjetoDB(TipoDatoDB.MAP_OBJETO, n);
					}
					else
					{
						return new TipoObjetoDB(TipoDatoDB.OBJETO, nombre);
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
					if (tipoClave == TipoOperacion.Boolean)
					{
						return true;
					}
					break;
				case TipoDatoDB.DATE:
					if (tipoClave == TipoOperacion.Date)
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
					if (tipoClave == TipoOperacion.Time)
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
				case TipoOperacion.Boolean:
				case TipoOperacion.String:
				case TipoOperacion.Numero:
				case TipoOperacion.Time:
				case TipoOperacion.Date:
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
					return TipoOperacion.Boolean;
				case TipoDatoDB.DATE:
					return TipoOperacion.Date;
				case TipoDatoDB.COUNTER:
				case TipoDatoDB.DOUBLE:
				case TipoDatoDB.INT:
					return TipoOperacion.Numero;
				case TipoDatoDB.STRING:
					return TipoOperacion.String;
				case TipoDatoDB.TIME:
					return TipoOperacion.Time;
				case TipoDatoDB.NULO:
					return TipoOperacion.Nulo;
				case TipoDatoDB.MAP_PRIMITIVO:
				case TipoDatoDB.MAP_OBJETO:
					return TipoOperacion.Map;
				case TipoDatoDB.LISTA_OBJETO:
				case TipoDatoDB.LISTA_PRIMITIVO:
					return TipoOperacion.List;
				case TipoDatoDB.SET_OBJETO:
				case TipoDatoDB.SET_PRIMITIVO:
					return TipoOperacion.Set;
				default:
					return TipoOperacion.Objeto;
			}
		}

		internal static string GetDate()
		{
			return DateTime.Now.ToString("yyyy-MM-dd") ;
		}

		internal static string GetTime()
		{
			return  DateTime.Now.ToString("HH:mm:ss");
		}

		internal static bool IsPrimitivo(TipoDatoDB ti)
		{
			switch (ti)
			{
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

		public static object CasteoImplicito(TipoObjetoDB tipo, object res, TablaSimbolos ts, Sesion sesion, int linea, int columna)
		{
			switch (tipo.Tipo)
			{
				case TipoDatoDB.INT:
					{
						if (double.TryParse(res.ToString(), out double d2))
						{
							return (int)d2;
						}

						break;
					}

				case TipoDatoDB.DOUBLE:
					{
						if (double.TryParse(res.ToString(), out double d2))
						{
							return d2;
						}

						break;
					}
				case TipoDatoDB.DATE:
				case TipoDatoDB.TIME:
					if (res.Equals("null")) {
						return new MyDateTime();
					}
					break;
				case TipoDatoDB.STRING:
					if (res.Equals("null")) {
						return "$%_null_%$";
					}
					break;
				case TipoDatoDB.MAP_OBJETO:
				case TipoDatoDB.MAP_PRIMITIVO:
					if (res.Equals("null"))
					{
						return new CollectionMapCql();
					}
					break;
				case TipoDatoDB.LISTA_PRIMITIVO:
				case TipoDatoDB.LISTA_OBJETO:
					if (res.Equals("null"))
					{
						return new CollectionListCql(true);
					}
					break;
				case TipoDatoDB.SET_PRIMITIVO:
				case TipoDatoDB.SET_OBJETO:
					if (res.Equals("null"))
					{
						return new CollectionListCql(false);
					}
					break;
			}

			return res;
		}
	}
}
