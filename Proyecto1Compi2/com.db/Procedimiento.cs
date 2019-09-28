using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.AST;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class Procedimiento:Sentencia
	{
		string nombre;
		List<Parametro> parametros;
		List<Parametro> retornos;
		List<Sentencia> sentencias;
		string instrucciones;
		List<object> valoresParametros;

		public Procedimiento(string nombre, List<Parametro> parametros, List<Parametro> retornos, List<Sentencia> sentencias, string instrucciones,int linea,int columna):base(linea,columna)
		{
			this.nombre = nombre;
			this.parametros = parametros;
			this.retornos = retornos;
			this.sentencias = sentencias;
			this.instrucciones = instrucciones;
		}

		public Procedimiento(int linea, int columna) : base(linea, columna)
		{
			this.nombre = null;
			this.parametros = new List<Parametro>();
			this.retornos = new List<Parametro>();
			this.sentencias = null;
			this.instrucciones = null;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public List<Parametro> Parametros { get => parametros; set => parametros = value; }
		public List<Parametro>  Retornos { get => retornos; set => retornos = value; }
		internal string Instrucciones { get => instrucciones; set => instrucciones = value; }
		internal List<Sentencia> Sentencias { get => sentencias; set => sentencias = value; }

		internal void pasarParametros(List<object> parametros)
		{
			valoresParametros = parametros;
		}

		public override object Ejecutar(TablaSimbolos ts,Sesion sesion)
		{
			TablaSimbolos local = new TablaSimbolos(ts);
			int contador = 0;
			object RETORNO = null;
			//AGREGANDO PARAMETROS
			foreach (Parametro param in parametros)
			{
				if (!local.ExisteSimboloEnAmbito(param.Nombre))
				{
					local.AgregarSimbolo(new Simbolo(param.Nombre, valoresParametros.ElementAt(contador), param.Tipo, Linea, Columna));
				}
				else
				{
					//ERROR
					return new ThrowError(TipoThrow.Exception,
						"La variable '" + param.Nombre + "' ya existe",
						Linea, Columna);
				}
				contador++;
			}
			//EJECUTANDO SENTENCIAS ******************************************************************
			object respuesta;
			foreach (Sentencia sentencia in sentencias)
			{
				respuesta = sentencia.Ejecutar(local,sesion);
				if (respuesta != null)
				{
					if (respuesta.GetType() == typeof(ThrowError))
					{
						Analizador.ErroresCQL.Add(new Error((ThrowError)respuesta));
					}
					else if (respuesta.GetType() == typeof(List<ThrowError>))
					{
						//AGREGANDO ERRORES A LISTA PRINCIPAL
						List<ThrowError> errores = (List<ThrowError>)respuesta;
						foreach (ThrowError error in errores)
						{
							Analizador.ErroresCQL.Add(new Error(error));
						}
					}
					else if (respuesta.GetType() == typeof(Return))
					{
						Return ret = (Return)respuesta;
						respuesta = ret.ValoresRetornados;
						if (respuesta.GetType() == typeof(ThrowError)) return respuesta;
						List<object> valoresRetornados = (List<object>)respuesta;
						if (valoresRetornados.Count == this.retornos.Count)
						{
							RETORNO = valoresRetornados;
						}
						else
						{
							return new ThrowError(TipoThrow.NumerReturnsException,
								"La cantidad de valores retornados es incorrecta, se deben retornar "+retornos.Count+" valores",
								Linea, Columna);
						}
						break;
					}
					else if (respuesta.GetType() == typeof(Throw))
					{
						return respuesta;
					}
					else if (respuesta.GetType() == typeof(ResultadoConsulta))
					{
						Analizador.ResultadosConsultas.Add(((ResultadoConsulta)respuesta).ToString());
					}
					else if (respuesta.GetType() == typeof(Break)|| respuesta.GetType() == typeof(Continue))
					{
						//break - continue
						Sentencia sent = (Sentencia)respuesta;
						Analizador.ErroresCQL.Add(new Error(TipoError.Semantico,
							"La sentencia no está en un bloque de código adecuado",
							sent.Linea, sent.Columna));
					}
				}
			}
			//******************************************************************************************
			//EVALUAR RETORNOS
			if (RETORNO != null)
			{
				List<object> valoresFinales = new List<object>();
				List<object> valoresRetornados = (List<object>)RETORNO;
				int indice = 1;
				foreach (object ret in valoresRetornados) {
					TipoObjetoDB ti = Datos.GetTipoObjetoDB(ret);
					if (Datos.IsTipoCompatibleParaAsignar(ti, ret))
					{
						object nuevaRespuesta = Datos.CasteoImplicito(ti, ret,ts,sesion,Linea,Columna);
						if (nuevaRespuesta != null)
						{
							if (nuevaRespuesta.GetType() == typeof(ThrowError))
							{
								return nuevaRespuesta;
							}
							valoresFinales.Add(nuevaRespuesta);
						}
					}
					else
					{
						return new ThrowError(TipoThrow.Exception,
								"El retorno de la función debe ser de tipo '" + ti.ToString() + "' en el valor "+indice,
								Linea, Columna);
					}
					indice++;
				}
				return valoresFinales;
			}
			if (this.retornos.Count>0)
			{
				string msj = "La función debe retornar " + retornos.Count + " valores";
				if(this.retornos.Count==1) msj = "La función debe retornar " + retornos.Count + " valor";
				return new ThrowError(TipoThrow.Exception,
							msj,
							Linea, Columna);
			}
			return null;
		}

		public string GetLlave()
		{
			StringBuilder llave = new StringBuilder();
			llave.Append(Nombre + "(");
			int contador = 0;
			foreach (Parametro par in this.parametros)
			{
				if (par.Tipo.Tipo == TipoDatoDB.INT || par.Tipo.Tipo == TipoDatoDB.DOUBLE)
				{
					llave.Append("numero");
				}
				else
				{
					llave.Append(par.Tipo.ToString());
				}
				if (contador < parametros.Count - 1)
				{
					llave.Append(",");
				}
				contador++;
			}
			llave.Append(")");
			return llave.ToString();
		}

		internal void LimpiarParametros()
		{
			valoresParametros = null;
		}

		public override string ToString()
		{
			StringBuilder cadena = new StringBuilder();
			cadena.Append("\n<\n");
			cadena.Append("\"CQL-TYPE\" = \"PROCEDURE\",\n");
			cadena.Append("\"NAME\" = \"" + Nombre + "\",\n");
			cadena.Append("\"PARAMETERS\" = [");
			int i = 0;
			//PARAMETROS
			foreach (Parametro kvp in Parametros)
			{
				cadena.Append("\n<");
				cadena.Append("\"NAME\"=\"" + kvp.Nombre + "\",");
				cadena.Append("\"TYPE\"=\"" + kvp.Tipo.ToString() + "\",");
				cadena.Append("\"AS\" = IN>");

				if (i < Parametros.Count - 1)
				{
					cadena.Append(",");
				}
				i++;
			}
			//RETORNOS
			if (retornos.Count > 0)
			{
				cadena.Append(",");
			}
			i = 0;
			foreach (Parametro kvp in Retornos)
			{
				cadena.Append("\n<");
				cadena.Append("\"NAME\"=\"" + kvp.Nombre + "\",");
				cadena.Append("\"TYPE\"=\"" + kvp.Tipo.ToString() + "\",");

				cadena.Append("\"AS\" = OUT>");

				if (i < Retornos.Count - 1)
				{
					cadena.Append(",");
				}
				i++;
			}
			cadena.Append("],\n");
			cadena.Append("\"INSTR\" = $\n" + instrucciones + "$\n");
			cadena.Append("\n>");

			return cadena.ToString();
		}

		internal bool isValido()
		{
			return this.nombre != null&&
			this.parametros != null&&
			this.retornos != null&&
			this.sentencias != null&&
			this.instrucciones != null;
		}

		internal bool existeParametro(string nombre)
		{
			foreach (Parametro para in this.parametros) {
				if (para.Nombre == nombre)
					return true;
				
			}
			return false;
		}

		internal bool existeRetorno(string nombre)
		{
			foreach (Parametro para in this.retornos)
			{
				if (para.Nombre == nombre)
					return true;
				
			}
			return false;
		}
	}
}
