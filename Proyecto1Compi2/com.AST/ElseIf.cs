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
	class ElseIf:Sentencia
	{
		Condicion condicion;
		List<Sentencia> sentencias;
		public ElseIf(Condicion condicion, List<Sentencia> sentencias,int linea,int columna):base(linea,columna)
		{
			this.condicion = condicion;
			this.sentencias = sentencias;
		}

		internal Condicion Condicion { get => condicion; set => condicion = value; }
		internal List<Sentencia> Sentencias { get => sentencias; set => sentencias = value; }

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			TablaSimbolos tlocal = new TablaSimbolos(tb);
			object res =EjecutarSentencias(sentencias, tlocal,sesion);
			if (res != null) return res;
			return null;
		}

		public static object EjecutarSentencias(List<Sentencia> MisSentencias, TablaSimbolos tsLocal,Sesion sesion)
		{
			object respuesta;
			foreach (Sentencia sentencia in MisSentencias)
			{
				respuesta = sentencia.Ejecutar(tsLocal,sesion);
				if (respuesta != null)
				{
					if (respuesta.GetType() == typeof(ThrowError))
					{
						return respuesta;
					}
					else if (respuesta.GetType() == typeof(ResultadoConsulta))
					{

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
