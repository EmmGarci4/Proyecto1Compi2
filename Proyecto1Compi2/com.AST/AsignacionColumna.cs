using System;
using System.Collections.Generic;
using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	internal class AsignacionColumna : Sentencia
	{
		Acceso izquierda;
		Expresion exp;

		public AsignacionColumna(Acceso izquierda, Expresion exp, int linea, int columna) : base(linea, columna)
		{
			this.izquierda = izquierda;
			this.exp = exp;
			tabla = null;
			posicionDato = 0;
		}

		internal Acceso Izquierda { get => izquierda; set => izquierda = value; }
		internal Expresion Exp { get => exp; set => exp = value; }

		public override object Ejecutar(TablaSimbolos ts, Sesion sesion)
		{
			//OBTENIENDO RESPUESTA DE EXPRESION
			object respuesta = exp.GetValor(ts, sesion);
			TipoOperacion tipoRespuesta = exp.GetTipo(ts, sesion);
			if (respuesta != null)
			{
				if (respuesta.GetType() == typeof(ThrowError))
				{
					return respuesta;
				}
			}
			//ASIGNANDO
			TipoObjetoDB tipo = Datos.GetTipoObjetoDB(respuesta);
			object posibleError = izquierda.Asignar(respuesta, tipo, ts, sesion);
			if (posibleError != null)
			{
				if (posibleError.GetType() == typeof(ThrowError))
				{
					return posibleError;
				}
			}
			return null;
		}
	}

}