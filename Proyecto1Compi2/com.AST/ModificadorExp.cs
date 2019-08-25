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
	class ModificadorExp : Expresion
	{
		bool sumar;
		Acceso acceso;
		Sesion sesion;
		private TipoOperacion tipo;

		public ModificadorExp(Acceso variable, bool sumar, int linea, int columna) : base(linea, columna)
		{
			this.acceso = variable;
			this.sumar = sumar;
		}

		public bool Sumar { get => sumar; set => sumar = value; }
		internal Acceso Acceso { get => acceso; set => acceso = value; }
		internal Sesion Sesion { get => sesion; set => sesion = value; }

		public override TipoOperacion GetTipo(TablaSimbolos ts)
		{
			return this.tipo;
		}

		public override object GetValor(TablaSimbolos tb)
		{
			//VALOR DE ACCESO
			object respuesta1 = acceso.GetValor(tb);
			if (respuesta1 != null)
			{
				if (respuesta1.GetType() == typeof(ThrowError))
				{
					return respuesta1;
				}
			}
			TipoOperacion tipo = Datos.GetTipoDatoDB(Datos.GetTipoObjetoDB(respuesta1).Tipo);
			Operacion op = null;
			if (sumar)
			{
				op = new Operacion( acceso, new Operacion(1,TipoOperacion.Numero, Linea, Columna), TipoOperacion.Suma, Linea, Columna);
			}
			else
			{
				op = new Operacion(acceso, new Operacion(1, TipoOperacion.Numero, Linea, Columna), TipoOperacion.Resta, Linea, Columna);
			}
			if (op != null)
			{
				object respuesta = op.GetValor(tb);
				if (respuesta != null)
				{
					if (respuesta.GetType() == typeof(ThrowError))
					{
						return respuesta;
					}
				}
				acceso.Asignar(respuesta, Datos.GetTipoObjetoDB(respuesta), tb, sesion);
			}
			return respuesta1;
		}

	}
}
