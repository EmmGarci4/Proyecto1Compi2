using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;

namespace Proyecto1Compi2.com.AST
{
	class LlamadaFuncionExp : Expresion
	{
		string nombre;
		List<Expresion> parametros;
		private bool ejecutado = false;

		public LlamadaFuncionExp(string nombre, List<Expresion> parametros,int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
			this.parametros = parametros;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		internal List<Expresion> Parametros { get => parametros; set => parametros = value; }

		public override TipoOperacion GetTipo(TablaSimbolos ts)
		{
			throw new NotImplementedException();
		}

		public override object GetValor(TablaSimbolos ts)
		{
			throw new NotImplementedException();
		}
	}
}
