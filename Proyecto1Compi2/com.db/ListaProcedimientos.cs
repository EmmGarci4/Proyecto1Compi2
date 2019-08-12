using com.Analisis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class ListaProcedimientos :List<Procedimiento>
	{

		public ListaProcedimientos()
		{

		}

		public Procedimiento Buscar(String nombre)
		{
			foreach (Procedimiento tb in this)
			{
				if (tb.Nombre.Equals(nombre))
				{
					return tb;
				}
			}
			return null;
		}

		public void Eliminar(string nombre)
		{
			foreach (Procedimiento tb in this)
			{
				if (tb.Nombre.Equals(nombre))
				{
					this.Remove(tb);
				}
			}
		}

		public bool Existe(String nombre)
		{
			foreach (Procedimiento tb in this)
			{
				if (tb.Nombre.Equals(nombre))
				{
					return true;
				}
			}
			return false;
		}


		public override string ToString()
		{
			StringBuilder cadena = new StringBuilder();
			IEnumerator<Procedimiento> enumerator = GetEnumerator();
			bool hasNext = enumerator.MoveNext();
			while (hasNext)
			{
				Procedimiento i = enumerator.Current;
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
			foreach (Procedimiento pr in this)
			{
				Console.WriteLine("Procedimiento"+pr.Nombre);
				//pr.Mostrar();
			}
		}
	}
}
