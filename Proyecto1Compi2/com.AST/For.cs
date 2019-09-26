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
	class For:Sentencia
	{
		public static int ITERACIONESMAXIMAS = 2000;
		Asignacion asignacion;
		Declaracion declaracion;
		Expresion condicion;
		Expresion operacion;
		List<Sentencia> sentencias;
		string variableLocal;

		public For(Asignacion asignacion, Declaracion declaracion, Expresion condicion, Expresion operacion,List<Sentencia> sentencias, int linea, int columna):base(linea,columna)
		{
			this.asignacion = asignacion;
			this.declaracion = declaracion;
			this.condicion = condicion;
			this.operacion = operacion;
			this.sentencias = sentencias;
		}

		internal Asignacion Asignacion { get => asignacion; set => asignacion = value; }
		internal Declaracion Declaracion { get => declaracion; set => declaracion = value; }
		internal Expresion Condicion { get => condicion; set => condicion = value; }
		internal Expresion Operacion { get => operacion; set => operacion = value; }
		internal List<Sentencia> Sentencias { get => sentencias; set => sentencias = value; }

		public override object Ejecutar(TablaSimbolos ts,Sesion sesion)
		{
			List<ThrowError> errores = new List<ThrowError>();
			if (asignacion != null) {
				object res = asignacion.Ejecutar(ts,sesion);
				if (res != null)if (res.GetType() == typeof(ThrowError))
						return res;
				
			} else if (declaracion!=null) {
				if (declaracion.Variables.Count == 1)
				{
					this.variableLocal = declaracion.Variables.ElementAt(0);
					object res = declaracion.Ejecutar(ts, sesion);
					if (res != null) if (res.GetType() == typeof(ThrowError))
							return res;
				}
				else {
					return new ThrowError(TipoThrow.Exception,
						"cantidad de variables incorrecta en un ciclo for",
					   Linea, Columna);
				}
				
			}
			int contador = 0;
			bool ejecutar = true;
			while (contador<ITERACIONESMAXIMAS && ejecutar && errores.Count==0) {
				TablaSimbolos local = new TablaSimbolos(ts);
				object res = condicion.GetValor(local,sesion);
				if (res != null)
				{
					if (res.GetType() == typeof(ThrowError))
					{
						return res;
					}
					if (res.GetType() != typeof(bool))
					{
						return new ThrowError(Util.TipoThrow.ArithmeticException,
							"No se puede evaluar una condición con un valor no booleano",
							Linea, Columna);
					}
					if ((bool)res)
					{
						//EJECUTANDO SENTENCIAS ******************************************************************
						object respuesta;
						foreach (Sentencia sentencia in sentencias)
						{
							respuesta = sentencia.Ejecutar(local, sesion);
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
								else if (respuesta.GetType() == typeof(Break))
								{
									ejecutar = false;
									break;
								}
								else if (respuesta.GetType() == typeof(Continue))
								{
									break;
								}
								else if (respuesta.GetType() == typeof(ResultadoConsulta))
								{
									Analizador.ResultadosConsultas.Add(((ResultadoConsulta)respuesta).ToString());
								}
								else if (respuesta.GetType() == typeof(Sentencia))
								{
									//return 
									if (errores.Count > 0) return errores;
									return respuesta;
								}
							}
						}
						//******************************************************************************************
						//ejecutando operacion
						if (ejecutar)
						{
							res = operacion.GetValor(ts, sesion);
							if (res != null)
							{
								if (res.GetType() == typeof(ThrowError))
								{
									return res;
								}
							}
						}
					}
					else
					{
						break;
					}
				}
				else {
					break;
				}
				

				contador++;
			}
			if (this.declaracion!=null) {
				ts.Remove(this.variableLocal);
			}
			if (contador == ITERACIONESMAXIMAS && ejecutar)
			{
				//error ciclo infinito
				return new ThrowError(TipoThrow.Exception,
						"Posiblemente existe un ciclo infinito en su código",
					   Linea, Columna);
			}
			if (errores.Count > 0) return errores;
			return null;
		}
	}
}
