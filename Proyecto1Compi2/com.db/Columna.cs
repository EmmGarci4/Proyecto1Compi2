using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class Columna
	{
		String titulo;
		TipoDatoDB tipo;
		List<Celda> valores;

		public string Titulo { get => titulo; set => titulo = value; }
		public TipoDatoDB TipoDato { get => tipo; set => tipo = value; }
		internal List<Celda> Valores { get => valores; set => valores = value; }

		public Columna(String titulo,TipoDatoDB tipo) {
			this.tipo = tipo;
			this.titulo = titulo;
			this.valores = new List<Celda>();
		}

		internal void AgregarValor(Celda celda)
		{
			this.valores.Add(celda);
		}
	}
}
