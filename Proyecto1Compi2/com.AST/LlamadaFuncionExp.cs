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

		public override TipoOperacion GetTipo(TablaSimbolos ts,Sesion sesion)
		{
				return tipoRetorno;
		}

		public override object GetValor(TablaSimbolos ts,Sesion sesion)
		{
			object res = llamada.Ejecutar(ts,sesion);
			ejecutado = true;
			if (res != null)
			{
				if (res.GetType()==typeof(ThrowError)) {
					return res;
				}
				this.tipoRetorno = Datos.GetTipoDatoDB(Datos.GetTipoObjetoDB(res).Tipo);
			}
				return res;
		}

		private string GetLlave(TablaSimbolos ts)
		{
			return llamada.getLlave(ts,sesion);
		}
	}
}
