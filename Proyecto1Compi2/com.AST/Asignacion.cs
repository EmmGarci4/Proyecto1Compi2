using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.Analisis;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class Asignacion:Sentencia
	{
		Acceso izquierda;
		Expresion derecha;

		public Asignacion(Acceso izquierda, Expresion derecha,int linea,int columna):base(linea,columna)
		{
			this.izquierda = izquierda;
			this.derecha = derecha;
		}

		internal Acceso Izquierda { get => izquierda; set => izquierda = value; }
		internal Expresion Derecha { get => derecha; set => derecha = value; }

		public override object Ejecutar(TablaSimbolos ts,Sesion sesion)
		{
			//OBTENIENDO RESPUESTA DE EXPRESION
			object respuesta = derecha.GetValor(ts,sesion);
			TipoOperacion tipoRespuesta = derecha.GetTipo(ts,sesion);
			if (respuesta != null)
			{
				if (respuesta.GetType() == typeof(ThrowError))
				{
					return respuesta;
				}
			}
			//ASIGNANDO
			TipoObjetoDB tipo = Datos.GetTipoObjetoDB(respuesta);
			object posibleError=izquierda.Asignar(respuesta,tipo, ts, sesion);
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
