using com.Analisis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class UserType
	{
		string nombre;
		Dictionary<string, TipoDatoDB> atributos;

		public UserType(string nombre, Dictionary<string, TipoDatoDB> atributos)
		{
			this.Nombre = nombre;
			this.Atributos = atributos;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public Dictionary<string, TipoDatoDB> Atributos { get => atributos; set => atributos = value; }

		internal void Mostrar()
		{
			Console.WriteLine("______________________________________________");
			Console.WriteLine("|	UserType:"+Nombre+"						|");
			Console.WriteLine("______________________________________________");
			foreach (KeyValuePair<string, TipoDatoDB> kvp in atributos)
			{
				Console.WriteLine("|Nombre = {0}|	| Tipo = {1}|",
					kvp.Key, kvp.Value);
			}
		}
	}
}
