using com.Analisis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class Celda
	{
		TipoDatoDB tipo;
		String valor;

		public string Valor { get => valor; set => valor = value; }
		internal TipoDatoDB TipoDato { get => tipo; set => tipo = value; }

		public Celda(string valor,TipoDatoDB tipo) {
			this.tipo = tipo;
			this.valor = valor;
		}

	}
}
