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
		Condicion condicion;
		Expresion operacion;
		List<Sentencia> sentencias;

		public For(Asignacion asignacion, Declaracion declaracion, Condicion condicion, Expresion operacion,List<Sentencia> sentencias, int linea, int columna):base(linea,columna)
		{
			this.asignacion = asignacion;
			this.declaracion = declaracion;
			this.condicion = condicion;
			this.operacion = operacion;
			this.sentencias = sentencias;
		}

		internal Asignacion Asignacion { get => asignacion; set => asignacion = value; }
		internal Declaracion Declaracion { get => declaracion; set => declaracion = value; }
		internal Condicion Condicion { get => condicion; set => condicion = value; }
		internal Expresion Operacion { get => operacion; set => operacion = value; }
		internal List<Sentencia> Sentencias { get => sentencias; set => sentencias = value; }

		public override object Ejecutar(TablaSimbolos ts,Sesion sesion)
		{
			TablaSimbolos local = new TablaSimbolos(ts);
			List<ThrowError> errores = new List<ThrowError>();
			if (asignacion != null) {
				object res = asignacion.Ejecutar(local,sesion);
				if (res != null)if (res.GetType() == typeof(ThrowError))
						return res;
				
			} else if (declaracion!=null) {
				object res = declaracion.Ejecutar(local,sesion);
				if (res != null) if (res.GetType() == typeof(ThrowError))
						return res;
			}
			int contador = 0;
			bool ejecutar = true;
			while (contador<ITERACIONESMAXIMAS && ejecutar && errores.Count==0) {
				object res = condicion.GetValor(local,sesion);
				if (res != null)
				{
					if (res.GetType() == typeof(ThrowError))
					{
						return res;
					}
				}
				if ((bool)res)
				{
					//EJECUTANDO SENTENCIAS ******************************************************************
					object respuesta;
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
								Analizador.ResultadosConsultas.Add((ResultadoConsulta)respuesta);
							}
							else
							{
								//return 
								if (errores.Count > 0) return errores;
								return respuesta;
							}
						}
					}
					//******************************************************************************************
					//ejecutando operacion
					if (ejecutar) {
						res = operacion.GetValor(local,sesion);
						if (res != null)
						{
							if (res.GetType() == typeof(ThrowError))
							{
								return res;
							}
						}
					}
				}
				else {
					break;
				}

				contador++;
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
