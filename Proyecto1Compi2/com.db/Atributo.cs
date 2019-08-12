using com.Analisis.Util;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class Atributo
	{
		string nombre;
		string valor;
		TipoObjetoDB tipo;

		public Atributo(string nombre, string valor, TipoObjetoDB tipo)
		{
			this.Nombre = nombre;
			this.Valor = valor;
			this.Tipo = tipo;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public string Valor { get => valor; set => valor = value; }
		internal TipoObjetoDB Tipo { get => tipo; set => tipo = value; }
	}
}
