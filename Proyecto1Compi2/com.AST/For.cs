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
	class For:Sentencia
	{
		Asignacion asignacion;
		Declaracion declaracion;
		Condicion condicion;
		Expresion operacion;
		List<Sentencia> sentencias;

		public For(Asignacion asignacion, Declaracion declaracion, Condicion condicion, Expresion operacion,List<Sentencia> sentencias, int linea, int columna):base(linea,columna)
		{
			this.asignacion = asignacion;
			this.declaracion = declaracion;
			this.condicion = condicion;
			this.operacion = operacion;
			this.sentencias = sentencias;
		}

		internal Asignacion Asignacion { get => asignacion; set => asignacion = value; }
		internal Declaracion Declaracion { get => declaracion; set => declaracion = value; }
		internal Condicion Condicion { get => condicion; set => condicion = value; }
		internal Expresion Operacion { get => operacion; set => operacion = value; }
		internal List<Sentencia> Sentencias { get => sentencias; set => sentencias = value; }

		public override object Ejecutar(Sesion sesion, TablaSimbolos ts)
		{
			TablaSimbolos local = new TablaSimbolos(ts);
			if (asignacion != null) {
				object res = Funcion.LeerRespuesta(asignacion.Ejecutar(sesion, local));
				if (res != null) return res;
			} else if (declaracion!=null) {
				object res = Funcion.LeerRespuesta(declaracion.Ejecutar(sesion, local));
				if (res != null) return res;
			}
			int contador = 0;
			while (contador<2000) {
				object res = condicion.GetValor(local);
				if (res != null)
				{
					if (res.GetType() == typeof(ThrowError))
					{
						return res;
					}
				}
				if ((bool)res)
				{
					//ejecutar
					res = EjecutarSentencias(sentencias,sesion,local);
					if (res != null) return res;
					//ejecutando operacion
					res = operacion.GetValor(local);
					if (res != null)
					{
						if (res.GetType() == typeof(ThrowError))
						{
							return res;
						}
					}
				}
				else {
					break;
				}

				contador++;
			}
			if (contador == 5000)
			{
				//error ciclo infinito
				return new ThrowError(TipoThrow.Exception,
						"Posiblemente existe un ciclo infinito en su código",
					   Linea, Columna);
			}
			return null;
		}

		private object EjecutarSentencias(List<Sentencia> MisSentencias, Sesion sesion, TablaSimbolos tsLocal)
		{
			object respuesta;
			foreach (Sentencia sentencia in MisSentencias)
			{
				respuesta = sentencia.Ejecutar(sesion, tsLocal);
				if (respuesta != null)
				{
					if (respuesta.GetType() == typeof(ThrowError))
					{
						return respuesta;
					}
					else
					{
						//EVALUAR SI ES RETURN, BREAK O CONTINUE
					}
				}
			}
			return null;
		}
	}
}
