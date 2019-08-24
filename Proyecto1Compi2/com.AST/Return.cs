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

		public Return(List<Expresion> valor, int linea, int columna) : base(linea, columna)
		{
			this.valor = valor;
		}

		public override object Ejecutar(Sesion sesion, TablaSimbolos tb)
		{
			List<object> valores = new List<object>();
			object respuesta;
			foreach (Expresion ex in valor) {
				respuesta = ex.GetValor(tb);
				if (respuesta != null) {
					if (respuesta.GetType()==typeof(ThrowError)) {
						return respuesta;
					}
					valores.Add(respuesta);
				} 
			}
			return valores;
		}
	}
}
