using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class Modificador:Sentencia 
	{

		ModificadorExp modificador;
		private TipoObjetoDB tipo;

		public Modificador(ModificadorExp modificador) : base(modificador.Linea, modificador.Columna)
		{
			this.modificador = modificador;
		}

		public TipoObjetoDB Tipo { get => tipo; set => tipo = value; }

		public override object Ejecutar(Sesion sesion, TablaSimbolos ts)
		{
			modificador.Sesion = sesion;
			object respuesta = modificador.GetValor(ts);
			modificador.Sesion = null;
			if (respuesta!=null) {
				if (respuesta.GetType()==typeof(ThrowError)) {
					return respuesta;
				}
			}
			return null;
		}
	}
}
