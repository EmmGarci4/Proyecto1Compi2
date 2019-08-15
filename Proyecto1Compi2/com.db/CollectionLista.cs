using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class CollectionLista : List<object>
	{
		bool isList;

		public CollectionLista()
		{
			isList = true;
		}

		public bool IsLista { get => isList; set => isList = value; }
		public bool IsSet { get => !isList; }

		public void AddItem(object obj) {
			if (isList)
			{
				this.Add(obj);
			}
			else {
				//insersion unica y ordenada

			}
		}

		public override string ToString()
		{
			StringBuilder cad = new StringBuilder();
			cad.Append("[");
			int i = 0;
			foreach (object ib in this) {
				cad.Append(ib.ToString());
				if (i<this.Count-1) {
					cad.Append(",");
				}
				i++;
			}
			cad.Append("]");
			return cad.ToString();
		}

		public bool IsAllInteger() {
			foreach (object ob in this) {
				if (ob.GetType()!=typeof(int)) {
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
				else {
					if (Regex.IsMatch(ob.ToString(), "'[0-9]{4}-[0-9]{2}-[0-9]{2}'")|| Regex.IsMatch(ob.ToString(), "'[0-9]{2}:[0-9]{2}:[0-9]{2}'")) {
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
					if (!Regex.IsMatch(ob.ToString(), "'[0-9]{4}-[0-9]{2}-[0-9]{2}'")) {
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
					if (!Regex.IsMatch(ob.ToString(), "'[0-9]{2}:[0-9]{2}:[0-9]{2}'")) {
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
				if (ob.GetType() == typeof(string)|| ob.GetType() == typeof(bool)|| ob.GetType() == typeof(int)
					|| ob.GetType() == typeof(double)|| ob.GetType() == typeof(CollectionLista))
				{
					Console.WriteLine("ERROR LA LISTA NO ES HOMOGENEA");
					return false;
				}
			}
			return true;
		}

		internal void Ordenar()
		{
			Console.WriteLine("ORDENANDO LISTA...");
		}
	}
}
