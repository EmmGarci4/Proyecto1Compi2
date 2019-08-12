using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class ListaTablas: List<Tabla>
	{
		public ListaTablas()
		{

		}

		public Tabla Buscar(String nombre)
		{
			foreach (Tabla tb in this)
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
			foreach (Tabla tb in this)
			{
				if (tb.Nombre.Equals(nombre))
				{
					this.Remove(tb);
				}
			}
		}

		public bool Existe(String nombre)
		{
			foreach (Tabla tb in this)
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
			IEnumerator<Tabla> enumerator = GetEnumerator();
			bool hasNext = enumerator.MoveNext();
			while (hasNext)
			{
				Tabla i = enumerator.Current;
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
			foreach (Tabla tb in this)
			{
				Console.WriteLine("Tabla: " + tb.Nombre);
				//tb.MostrarCabecera();
				//tb.MostrarDatos();
			}
		}
	}
}
