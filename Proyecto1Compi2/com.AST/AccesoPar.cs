using com.Analisis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	public class AccesoPar
	{
		object value;
		TipoAcceso tipo;

		public AccesoPar(object value, TipoAcceso tipo)
		{
			this.value = value;
			this.tipo = tipo;
		}

		public object Value { get => value; set => this.value = value; }
		public TipoAcceso Tipo { get => tipo; set => tipo = value; }
	}

}
