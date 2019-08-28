using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Parametro:IEquatable<Parametro>
	{
		string nombre;
		TipoObjetoDB tipo;

		public Parametro(string nombre, TipoObjetoDB tipo)
		{
			this.nombre = nombre;
			this.tipo = tipo;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public TipoObjetoDB Tipo { get => tipo; set => tipo = value; }

		public bool Equals(Parametro obj)
		{
			var parametro = obj as Parametro;
			return parametro != null &&
				   nombre == parametro.nombre;
		}
	}
}
