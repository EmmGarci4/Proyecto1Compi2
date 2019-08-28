using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;

namespace Proyecto1Compi2.com.AST
{
	class Case:Sentencia
	{
		Expresion exp;
		List<Sentencia> sentencias;

		public Case(Expresion exp, List<Sentencia> sentencias,int linea,int columna):base(linea,columna)
		{
			this.exp = exp;
			this.sentencias = sentencias;
		}

		internal Expresion Exp { get => exp; set => exp = value; }
		internal List<Sentencia> Sentencias { get => sentencias; set => sentencias = value; }

		public override object Ejecutar(TablaSimbolos ts,Sesion sesion)
		{
			TablaSimbolos local = new TablaSimbolos(ts);
			List<ThrowError> errores = new List<ThrowError>();
			object respuesta;
			//EJECUTANDO SENTENCIAS ******************************************************************
			foreach (Sentencia sentencia in sentencias)
			{
				respuesta = sentencia.Ejecutar(local,sesion);
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
