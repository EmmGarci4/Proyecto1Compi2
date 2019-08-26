using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;

namespace Proyecto1Compi2.com.AST
{
	class TryCatch:Sentencia
	{
		List<Sentencia> sentenciasTry;
		List<Sentencia> sentenciasCatch;
		string tipoException;
		string nombreVariable;

		public TryCatch(List<Sentencia> sentenciasTry, List<Sentencia> sentenciasCatch, string tipoException, string nombreVariable,int linea,int columna):base(linea,columna)
		{
			this.sentenciasTry = sentenciasTry;
			this.sentenciasCatch = sentenciasCatch;
			this.tipoException = tipoException;
			this.nombreVariable = nombreVariable;
		}

		public override object Ejecutar(TablaSimbolos ts)
		{
			//object respuesta;
			//List<ThrowError> errores = new List<ThrowError>();
			//foreach (Sentencia sentencia in MisSentencias)
			//{
			//	respuesta = sentencia.Ejecutar(tsLocal);
			//	if (respuesta != null)
			//	{
			//		if (respuesta.GetType() == typeof(ThrowError))
			//		{
			//			errores.Add((ThrowError)respuesta);
			//		}
			//		else if (respuesta.GetType() == typeof(List<ThrowError>))
			//		{
			//			errores.AddRange((List<ThrowError>)respuesta);
			//		}
			//		else
			//		{
			//			//return-break-continue
			//			if (errores.Count > 0) return errores;
			//			return respuesta;
			//		}

			//	}
			//}
			//if (errores.Count > 0) return errores;
			//return null;
			return null;
		}
	}
}
