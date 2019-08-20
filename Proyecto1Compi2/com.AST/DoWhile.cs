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
	class DoWhile : Sentencia
	{
		Condicion condicion;
		List<Sentencia> sentencias;

		public DoWhile(Condicion condicion, List<Sentencia> sentencias, int linea, int columna) : base(linea, columna)
		{
			this.condicion = condicion;
			this.sentencias = sentencias;
		}

		internal Condicion Condicion { get => condicion; set => condicion = value; }
		internal List<Sentencia> Sentencias { get => sentencias; set => sentencias = value; }

		public override object Ejecutar(Sesion sesion, TablaSimbolos ts)
		{
			TablaSimbolos tsLocal = new TablaSimbolos(ts);
			int contador = 0;
			object respuesta;
			//repetir y evaluar
			while (contador < 2000)
			{
				//EJECUTANDO SENTENCIAS
				foreach (Sentencia sentencia in sentencias)
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
				//EVALUANDO CONDICION
				respuesta = condicion.GetValor(tsLocal);
				if (respuesta != null)
				{
					if (respuesta.GetType() == typeof(ThrowError))
					{
						return respuesta;
					}
					if (!(bool)respuesta)
					{
						break;
					}
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
	}
}
