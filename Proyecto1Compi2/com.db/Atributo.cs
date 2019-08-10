using com.Analisis.Util;
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
		TipoDatoDB tipo;

		public Atributo(string nombre, string valor, TipoDatoDB tipo)
		{
			this.Nombre = nombre;
			this.Valor = valor;
			this.Tipo = tipo;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public string Valor { get => valor; set => valor = value; }
		internal TipoDatoDB Tipo { get => tipo; set => tipo = value; }
	}
}
