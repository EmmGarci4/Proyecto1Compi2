using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Return : Sentencia
	{
		List<Expresion> valor;
		object valoresRetornados;

		public Return(List<Expresion> valor, int linea, int columna) : base(linea, columna)
		{
			this.valor = valor;
		}

		public object ValoresRetornados { get => valoresRetornados; set => valoresRetornados = value; }

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			this.valoresRetornados = getValores(tb,sesion);
			return this;
		}

		private object getValores(TablaSimbolos ts,Sesion sesion) {
			List<object> valores = new List<object>();
			object respuesta;
			foreach (Expresion ex in valor)
			{
				respuesta = ex.GetValor(ts,sesion);
				if (respuesta != null)
				{
					if (respuesta.GetType() == typeof(ThrowError))
					{
						return respuesta;
					}
					valores.Add(respuesta);
				}
			}
			return valores;
		}
	}
}
