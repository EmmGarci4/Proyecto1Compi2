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
	class OperacionAsignacion:Sentencia
	{
		Acceso acceso;
		Expresion exp;
		TipoOperacion operacion;

		public OperacionAsignacion(Acceso acceso,TipoOperacion op, Expresion exp,int linea,int columna):base(linea,columna)
		{
			this.operacion = op;
			this.acceso = acceso;
			this.exp = exp;
		}

		public TipoOperacion Operacion { get => operacion; set => operacion = value; }
		internal Acceso Acceso { get => acceso; set => acceso = value; }
		internal Expresion Exp { get => exp; set => exp = value; }

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			//VALOR DE ACCESO
			object respuesta=acceso.GetValor(tb,sesion);
			if (respuesta!=null) {
				if (respuesta.GetType()==typeof(ThrowError)) {
					return respuesta;
				}
			}
			TipoOperacion tipo = Datos.GetTipoDatoDB(Datos.GetTipoObjetoDB(respuesta).Tipo);
			Operacion op = null;
			switch (operacion) {
				case TipoOperacion.Suma:
					op =new Operacion(new Operacion(respuesta, tipo, Linea, Columna), exp, TipoOperacion.Suma, Linea, Columna);
					break;
				case TipoOperacion.Resta:
					op = new Operacion(new Operacion(respuesta, tipo, Linea, Columna), exp, TipoOperacion.Resta, Linea, Columna);
					break;
				case TipoOperacion.Multiplicacion:
					op = new Operacion(new Operacion(respuesta, tipo, Linea, Columna), exp, TipoOperacion.Multiplicacion, Linea, Columna);
					break;
				case TipoOperacion.Division:
					op = new Operacion(new Operacion(respuesta, tipo, Linea, Columna), exp, TipoOperacion.Division, Linea, Columna);
					break;
			}
			if (op!=null) {
				respuesta = op.GetValor(tb,sesion);
				if (respuesta != null)
				{
					if (respuesta.GetType() == typeof(ThrowError))
					{
						return respuesta;
					}
				}
				acceso.Asignar(respuesta, Datos.GetTipoObjetoDB(respuesta), tb,sesion);
			}
			return null;
		}
	}
}
