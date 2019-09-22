using Proyecto1Compi2.com.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Util
{
	class CollectionListCql : List<object>
	{
		bool isList;
		TipoObjetoDB tipoDato;

		public CollectionListCql(TipoObjetoDB tipo, bool isList)
		{
			this.tipoDato = tipo;
			this.isList = isList;
		}
		public bool IsLista { get => isList; set => isList = value; }
		public bool IsSet { get => !isList; }
		public TipoObjetoDB TipoDato { get => tipoDato; set => tipoDato = value; }

		public object AddItem(object obj, int linea, int columna)
		{
			if (isList)
			{
				this.Add(obj);
			}
			else
			{
				//insersion unica
				foreach (object objeto in this)
				{
					if (objeto.Equals(obj))
					{
						return new ThrowError(TipoThrow.Exception, "El elemento ya existe en el Set",
							linea, columna);
					}
				}
				switch (Datos.GetTipoObjetoDBPorCadena(tipoDato.Nombre).Tipo)
				{
					case TipoDatoDB.BOOLEAN:
					case TipoDatoDB.STRING:
					case TipoDatoDB.DOUBLE:
					case TipoDatoDB.INT:
					case TipoDatoDB.TIME:
					case TipoDatoDB.DATE:
						//insersion ordenada
						this.Add(obj);
						this.Sort();
						break;
					case TipoDatoDB.LISTA_OBJETO:
					case TipoDatoDB.LISTA_PRIMITIVO:
					case TipoDatoDB.MAP_OBJETO:
					case TipoDatoDB.MAP_PRIMITIVO:
					case TipoDatoDB.OBJETO:
					case TipoDatoDB.SET_OBJETO:
					case TipoDatoDB.SET_PRIMITIVO:
						//insersion al final
						this.Add(obj);
						break;
				}
			}
			return null;
		}

		public override string ToString()
		{
			StringBuilder cad = new StringBuilder();
			cad.Append("[");
			int i = 0;
			foreach (object ib in this)
			{
					if (this.tipoDato.Tipo.Equals(TipoDatoDB.STRING))
					{
						cad.Append("\"" + ib.ToString() + "\"");
					}
					else if (this.tipoDato.Tipo.Equals(TipoDatoDB.DATE) || this.tipoDato.Tipo.Equals(TipoDatoDB.TIME))
					{
						if (ib.ToString().Equals("null"))
						{
							cad.Append("null");
						}
						else
						{
							cad.Append("\'" + ib.ToString() + "\'");
						}
					}
					else
					{
						cad.Append(ib.ToString());
					}
				
				if (i < this.Count - 1)
				{
					cad.Append(",");
				}
				i++;
			}
			cad.Append("]");
			return cad.ToString();
		}
		
		//Metodos usados desde GeneradorDB
		public bool IsAllInteger()
		{
			foreach (object ob in this)
			{
				if (ob.GetType() != typeof(int))
				{
					Console.WriteLine("ERROR LA LISTA NO ES HOMOGENEA");
					return false;
				}
			}
			return true;
		}

		public bool IsAllDouble()
		{
			foreach (object ob in this)
			{
				if (ob.GetType() != typeof(double))
				{
					Console.WriteLine("ERROR LA LISTA NO ES HOMOGENEA");
					return false;
				}
			}
			return true;
		}

		public bool IsAllBool()
		{
			foreach (object ob in this)
			{
				if (ob.GetType() != typeof(bool))
				{
					Console.WriteLine("ERROR LA LISTA NO ES HOMOGENEA");
					return false;
				}
			}
			return true;
		}

		public bool IsAllString()
		{
			foreach (object ob in this)
			{
				if (ob.GetType() != typeof(string))
				{
					return false;
				}
				else
				{
					if (Regex.IsMatch(ob.ToString(), "\b'[0-9]{4}-[0-9]{2}-[0-9]{2}'") || Regex.IsMatch(ob.ToString(), "\b'[0-9]{2}:[0-9]{2}:[0-9]{2}'"))
					{
						Console.WriteLine("ERROR LA LISTA NO ES HOMOGENEA");
						return false;
					}
				}
			}
			return true;
		}

		public bool IsAllDate()
		{
			foreach (object ob in this)
			{
				if (ob.GetType() == typeof(string))
				{
					if (!Regex.IsMatch(ob.ToString(), "\b'[0-9]{4}-[0-9]{2}-[0-9]{2}'"))
					{
						Console.WriteLine("ERROR LA LISTA NO ES HOMOGENEA");
						return false;
					}
				}
			}
			return true;
		}

		public bool IsAllTime()
		{
			foreach (object ob in this)
			{
				if (ob.GetType() == typeof(string))
				{
					if (!Regex.IsMatch(ob.ToString(), "\b'[0-9]{2}:[0-9]{2}:[0-9]{2}'"))
					{
						Console.WriteLine("ERROR LA LISTA NO ES HOMOGENEA");
						return false;
					}
				}
			}
			return true;
		}

		public bool IsAllObjeto()
		{
			foreach (object ob in this)
			{
				if (ob.GetType() == typeof(string) || ob.GetType() == typeof(bool) || ob.GetType() == typeof(int)
					|| ob.GetType() == typeof(double) || ob.GetType() == typeof(CollectionListCql))
				{
					Console.WriteLine("ERROR LA LISTA NO ES HOMOGENEA");
					return false;
				}
			}
			return true;
		}

		internal object SetItem(int posicion, object nuevoValor, int linea, int columna)
		{
			if (isList)
			{
				this[posicion] = nuevoValor;
				return null;
			}
			else {
				//insersion unica
				foreach (object objeto in this)
				{
					if (objeto.Equals(nuevoValor))
					{
						return new ThrowError(TipoThrow.Exception, "El elemento ya existe en el Set",
							linea, columna);
					}
				}
				//insersion ordenada
				switch (tipoDato.Tipo)
				{
					case TipoDatoDB.BOOLEAN:
					case TipoDatoDB.STRING:
					case TipoDatoDB.DOUBLE:
					case TipoDatoDB.INT:
					case TipoDatoDB.TIME:
					case TipoDatoDB.DATE:
						this[posicion] = nuevoValor;
						this.Sort();
						break;
					case TipoDatoDB.LISTA_OBJETO:
					case TipoDatoDB.LISTA_PRIMITIVO:
					case TipoDatoDB.MAP_OBJETO:
					case TipoDatoDB.MAP_PRIMITIVO:
					case TipoDatoDB.OBJETO:
					case TipoDatoDB.SET_OBJETO:
					case TipoDatoDB.SET_PRIMITIVO:
						//insersion al final
						this[posicion] = nuevoValor;
						break;
				}				
				return null;
			}
		}

		internal void EliminarItem(int posicion)
		{
			this.RemoveAt(posicion);
		}

		public string GetLinealizado() {
			StringBuilder cad = new StringBuilder();
			cad.Append("[");
			int i = 0;
			foreach (object ib in this)
			{
				if (ib.GetType() == typeof(CollectionListCql))
				{
					cad.Append(((CollectionListCql)ib).GetLinealizado());
					if (i < this.Count - 1)
					{
						cad.Append(",");
					}
					i++;
				} else if (ib.GetType() == typeof(CollectionMapCql))
				{
					cad.Append(((CollectionMapCql)ib).GetLinealizado());
					if (i < this.Count - 1)
					{
						cad.Append(",");
					}
					i++;
				} else if (ib.GetType()==typeof(Objeto)) {
					cad.Append(((Objeto)ib).GetLinealizado());
					if (i < this.Count - 1)
					{
						cad.Append(",");
					}
					i++;
				}
				else
				{
					cad.Append(ib.ToString());
					if (i < this.Count - 1)
					{
						cad.Append(",");
					}
					i++;
				}
			}
			cad.Append("]");
			return cad.ToString();
		}

		internal void Ordenar()
		{
			this.Ordenar();
		}
	}
}
