using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class LlamadaFuncionExp : Expresion
	{
		LlamadaFuncion llamada;
		private bool ejecutado;
		private TipoOperacion tipoRetorno;
		Sesion sesion; 

		public LlamadaFuncionExp(string nombre, List<Expresion> parametros,int linea, int columna) : base(linea, columna)
		{
			llamada = new LlamadaFuncion(nombre, parametros, linea, columna);
		}

		internal Sesion Sesion { get => sesion; set => sesion = value; }

		public override TipoOperacion GetTipo(TablaSimbolos ts)
		{
			if (ejecutado)
			{
				return tipoRetorno;
			}
			return TipoOperacion.Nulo;
		}

		public override object GetValor(TablaSimbolos ts)
		{
			object res = llamada.Ejecutar( ts);
			ejecutado = true;
			if (res != null)
			{
				this.tipoRetorno = Datos.GetTipoDatoDB(Datos.GetTipoObjetoDB(res).Tipo);
			}
			else {
				this.tipoRetorno = TipoOperacion.Nulo;
			}
				return res;
		}

		private string GetLlave(TablaSimbolos ts)
		{
			return llamada.getLlave(ts);
		}
	}
}
