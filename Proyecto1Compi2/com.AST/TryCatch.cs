using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.Util;

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
			object respuesta;
			ThrowError ERROR=null;
			TablaSimbolos tsLocal = new TablaSimbolos(ts);
			foreach (Sentencia sentencia in sentenciasTry)
			{
				respuesta = sentencia.Ejecutar(tsLocal);
				if (respuesta != null)
				{
					if (respuesta.GetType() == typeof(ThrowError))
					{
						ThrowError tr= (ThrowError)respuesta;
						if (tr.Tipo != TipoThrow.Exception)
						{
							ERROR = tr;
						}
						else {
							Analizador.ErroresCQL.Add(new Error(tr));
						}
					}
					else if (respuesta.GetType() == typeof(List<ThrowError>))
					{
						foreach (ThrowError err in (List<ThrowError>)respuesta) {
							if (ERROR==null && err.Tipo!=TipoThrow.Exception) {
								ERROR = err;
							}
							Analizador.ErroresCQL.Add(new Error(err));
						}
					} else if (respuesta.GetType()==typeof(Throw)) {
						Throw tr = (Throw)respuesta;
						if (Datos.GetExceptcion(tr.NombreExeption)!=TipoThrow.Exception) {
							ERROR = new ThrowError(Datos.GetExceptcion(tr.NombreExeption), 
								"Se ha producido una excepción", tr.Linea, tr.Columna);
							break;
						}
					}
					else
					{
						//return-break-continue
						return respuesta;
					}

				}
			}
			//EVALUAR SI HAY ERRORES
			if (ERROR!=null) {
				TablaSimbolos local = new TablaSimbolos(ts);
				//AGREGANDO VARIABLE DE EXCEPCION
				if (!local.ExisteSimbolo(nombreVariable))
				{
					Objeto error = new Objeto(Analizador.ErrorCatch);
					error.Atributos.Add("message",ERROR.Tipo.ToString()+":"+ERROR.Mensaje+" en línea "+ERROR.Linea);
					local.AgregarSimbolo(new Simbolo(nombreVariable,error,new TipoObjetoDB(TipoDatoDB.OBJETO,"errorCatch"),ERROR.Linea,ERROR.Columna));
				}
				else {
					return new ThrowError(TipoThrow.Exception,
						"La variable '" + nombreVariable + "' ya existe",
						Linea, Columna);
				}
				//EJECUTANDO SENTENCIAS
				foreach (Sentencia sentencia in sentenciasCatch)
				{
					respuesta = sentencia.Ejecutar(local);
					if (respuesta != null)
					{
						if (respuesta.GetType() == typeof(ThrowError))
						{
							Analizador.ErroresCQL.Add(new Error((ThrowError)respuesta));
						}
						else if (respuesta.GetType() == typeof(List<ThrowError>))
						{
							foreach (ThrowError err in (List<ThrowError>)respuesta) {
								Analizador.ErroresCQL.Add(new Error(err));
							}
						}
						else
						{
							//return-break-continuew
							return respuesta;
						}
					}
				}
			}
			return null;
		}
	}
}
