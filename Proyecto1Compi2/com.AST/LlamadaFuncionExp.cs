using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis;
using com.Analisis.Util;

namespace Proyecto1Compi2.com.AST
{
	class LlamadaFuncionExp : Expresion
	{
		string nombre;
		List<Expresion> parametros;
		private bool ejecutado = false;
		private object resultado;

		public LlamadaFuncionExp(string nombre, List<Expresion> parametros,int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
			this.parametros = parametros;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		internal List<Expresion> Parametros { get => parametros; set => parametros = value; }

		public override TipoOperacion GetTipo(TablaSimbolos ts)
		{
			String llave = GetLlave(ts);
			if (Analizador.ExisteFuncion(llave))
			{
				Funcion funcion = Analizador.BuscarFuncion(llave);
				return Operacion.GetTipoDatoDB(funcion.TipoRetorno.Tipo);
			}
			return TipoOperacion.Nulo;
		}

		public override object GetValor(TablaSimbolos ts)
		{
			String llave = GetLlave(ts);
			if (Analizador.ExisteFuncion(llave))
			{
				Funcion funcion = Analizador.BuscarFuncion(llave);
				return 1;
			}
			else {
				
				return new ThrowError(Util.TipoThrow.Exception,
					"La función '"+llave+"' no existe",
					Linea, Columna);
			}
		}

		private string GetLlave(TablaSimbolos ts)
		{
			StringBuilder llave = new StringBuilder();
			llave.Append(Nombre + "(");
			int contador = 0;
			foreach (Expresion ex in parametros)
			{
				llave.Append(ex.GetTipo(ts).ToString().ToLower());
				if (contador < parametros.Count - 1)
				{
					llave.Append(",");
				}
			}
			llave.Append(")");
			return llave.ToString();
		}
	}
}
