using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
				case TipoDatoDB.LISTA_BOOLEAN:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							return list.IsAllBool();
						}
					}
					break;
				case TipoDatoDB.LISTA_DATE:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							return list.IsAllDate();
						}
					}
					break;
				case TipoDatoDB.LISTA_DOUBLE:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							return list.IsAllDouble();
						}
					}
					break;
				case TipoDatoDB.LISTA_INT:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							return list.IsAllInteger();
						}
					}
					break;
				case TipoDatoDB.LISTA_OBJETO:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							return list.IsAllObjeto();
						}
					}
					break;
				case TipoDatoDB.LISTA_STRING:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							return list.IsAllString();
						}
					}
					break;
				case TipoDatoDB.LISTA_TIME:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							return list.IsAllTime();
						}
					}
					break;
				case TipoDatoDB.SET_BOOLEAN:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							list.IsLista = false;
							list.Ordenar();
							return list.IsAllBool();
						}
					}
					break;
				case TipoDatoDB.SET_DATE:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							list.IsLista = false;
							return list.IsAllDate();
						}
					}
					break;
				case TipoDatoDB.SET_DOUBLE:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							list.IsLista = false;
							list.Ordenar();
							return list.IsAllDouble();
						}
					}
					break;
				case TipoDatoDB.SET_INT:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							list.IsLista = false;
							list.Ordenar();
							return list.IsAllInteger();
						}
					}
					break;
				case TipoDatoDB.SET_OBJETO:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							list.IsLista = false;
							return list.IsAllObjeto();
						}
					}
					break;
				case TipoDatoDB.SET_STRING:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							list.IsLista = false;
							list.Ordenar();
							return list.IsAllString();
						}
					}
					break;
				case TipoDatoDB.SET_TIME:
					if (v.GetType() == typeof(CollectionLista))
					{
						CollectionLista list = (CollectionLista)v;
						if (list.IsLista)
						{
							return list.IsAllTime();
						}
					}
					break;
				case TipoDatoDB.MAP_OBJETO:
				case TipoDatoDB.OBJETO:
					return true;
			}

			return false;
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
		
		internal static string GetDate()
		{
			return "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
		}

		internal static string GetTime()
		{
			return "'" + DateTime.Now.ToString("HH:mm:ss") + "'";
		}
	}
}
