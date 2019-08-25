using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;

namespace Proyecto1Compi2.com.AST
{
	class If:Sentencia
	{
		private Expresion condicion;
		private List<ElseIf> elseIfs;
		private List<Sentencia> cuerpoVerdadero;
		private List<Sentencia> cuerpoFalso;

		public If(Expresion condicion, List<ElseIf> elseIfs, List<Sentencia> cuerpoVerdadero, List<Sentencia> cuerpoFalso,int linea,int columna):base(linea,columna)
		{
			this.condicion = condicion;
			this.elseIfs = elseIfs;
			this.cuerpoVerdadero = cuerpoVerdadero;
			this.cuerpoFalso = cuerpoFalso;
		}

		internal Expresion Condicion { get => condicion; set => condicion = value; }
		internal List<ElseIf> ElseIfs { get => elseIfs; set => elseIfs = value; }
		internal List<Sentencia> CuerpoVerdadero { get => cuerpoVerdadero; set => cuerpoVerdadero = value; }
		internal List<Sentencia> CuerpoFalso { get => cuerpoFalso; set => cuerpoFalso = value; }

		public override object Ejecutar(TablaSimbolos ts)
		{
			TablaSimbolos tsLocal = new TablaSimbolos(ts);

			object respuesta = condicion.GetValor(tsLocal);
			if (respuesta.GetType()==typeof(ThrowError)) {
				return respuesta;
			}
			if ((bool)respuesta)
			{
				//EJECUTANDO SENTENCIAS ******************************************************************
				respuesta = EjecutarSentencias(CuerpoVerdadero, tsLocal);
				if (respuesta != null) return respuesta;
			}
			else {
				bool evaluado = false;
				if (elseIfs!=null) {
					foreach (ElseIf elseif in elseIfs) {
						respuesta = elseif.Condicion.GetValor(tsLocal);
						if (respuesta.GetType() == typeof(ThrowError))
						{
							return respuesta;
						}else 
						//evaluando condicion de if
						if ((bool)respuesta) {
							evaluado = true;
							if (respuesta!=null) {
								//EJECUTANDO SENTENCIAS ******************************************************************
								respuesta = elseif.Ejecutar( tsLocal);
								if (respuesta != null) return respuesta;
							}
							break;
						}
					}
				}

				if (CuerpoFalso!=null && !evaluado) {
					//EJECUTANDO SENTENCIAS ******************************************************************
					respuesta = EjecutarSentencias(CuerpoFalso, tsLocal);
					if (respuesta != null) return respuesta;
				}
			}
			return null;
		}

		public static object EjecutarSentencias(List<Sentencia> MisSentencias, TablaSimbolos tsLocal)
		{
			object respuesta;
			List<ThrowError> errores = new List<ThrowError>();
			foreach (Sentencia sentencia in MisSentencias)
			{
				respuesta = sentencia.Ejecutar( tsLocal);
				if (respuesta != null) {
					if (respuesta.GetType() == typeof(ThrowError))
					{
						errores.Add((ThrowError)respuesta);
					}
					else if (respuesta.GetType() == typeof(List<ThrowError>))
					{
						errores.AddRange((List<ThrowError>)respuesta);
					}
					else {
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
