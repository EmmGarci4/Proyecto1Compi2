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
	class ElseIf:Sentencia
	{
		Expresion condicion;
		List<Sentencia> sentencias;
		public ElseIf(Expresion condicion, List<Sentencia> sentencias,int linea,int columna):base(linea,columna)
		{
			this.condicion = condicion;
			this.sentencias = sentencias;
		}

		internal Expresion Condicion { get => condicion; set => condicion = value; }
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
			List<ThrowError> errores = new List<ThrowError>();
			foreach (Sentencia sentencia in MisSentencias)
			{
				respuesta = sentencia.Ejecutar(tsLocal, sesion);
				if (respuesta != null)
				{
					if (respuesta.GetType() == typeof(ThrowError))
					{
						errores.Add((ThrowError)respuesta);
					}
					else if (respuesta.GetType() == typeof(List<ThrowError>))
					{
						errores.AddRange((List<ThrowError>)respuesta);
					}
					else if (respuesta.GetType() == typeof(ResultadoConsulta))
					{
						Analizador.ResultadosConsultas.Add(((ResultadoConsulta)respuesta).ToString());
					}
					else
					{
						//return-break-continue
						if (errores.Count > 0) return errores;
						return respuesta;
					}

				}
			}
			if (errores.Count > 0) return errores;
			return null;
		}
	}
}
