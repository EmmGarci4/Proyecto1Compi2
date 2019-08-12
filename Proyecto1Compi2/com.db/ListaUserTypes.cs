﻿using com.Analisis.Util;
using Proyecto1Compi2.com.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class ListaUserTypes:List<UserType>
	{
		
		public ListaUserTypes() {
		}

		public override string ToString()
		{
			StringBuilder cadena = new StringBuilder();
			IEnumerator<UserType> enumerator = GetEnumerator();
			bool hasNext = enumerator.MoveNext();
			while (hasNext)
			{
				UserType i = enumerator.Current;
				cadena.Append(i.ToString());
				hasNext = enumerator.MoveNext();
				if (hasNext)
				{
					cadena.Append(",");
				}
			}
			enumerator.Dispose();
			return cadena.ToString();
		}

		internal void Mostrar()
		{
			foreach (UserType user in this)
			{
				Console.WriteLine("UserType: "+user.Nombre);
				//user.Mostrar();
			}
		}
	}
}
