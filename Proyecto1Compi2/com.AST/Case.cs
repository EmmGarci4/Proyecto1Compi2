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

		public override object Ejecutar(Sesion sesion, TablaSimbolos ts)
		{
			foreach (Sentencia sentencia in sentencias)
			{
				object respuesta = sentencia.Ejecutar(sesion, ts);
				if (respuesta != null)
				{
					if (respuesta.GetType() == typeof(ThrowError))
					{
						return respuesta;
					}
					else if (respuesta.GetType() == typeof(Break))
					{
						return respuesta;
					}
					{
						//EVALUAR SI ES RETURN, O CONTINUE
					}
				}
			}
			return null;
		}
	}
}
