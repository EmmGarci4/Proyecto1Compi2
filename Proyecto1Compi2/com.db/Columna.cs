using com.Analisis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class Columna
	{
		String nombre;
		TipoDatoDB tipo;
		bool isPrimary;

		public string Nombre { get => nombre; set => nombre = value; }
		public TipoDatoDB Tipo { get => tipo; set => tipo = value; }
		public bool IsPrimary { get => isPrimary; set => isPrimary = value; }

		public Columna(String titulo,TipoDatoDB tipo,bool isp) {
			this.tipo = tipo;
			this.nombre = titulo;
			this.isPrimary = isp;
		}

	}
}
