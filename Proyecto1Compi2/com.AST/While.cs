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
	class While : Sentencia
	{
		Condicion condicion;
		List<Sentencia> sentencias;

		public While(Condicion condicion, List<Sentencia> sentencias, int linea, int columna) : base(linea, columna)
		{
			this.condicion = condicion;
			this.sentencias = sentencias;
		}

		internal Condicion Condicion { get => condicion; set => condicion = value; }
		internal List<Sentencia> Sentencias { get => sentencias; set => sentencias = value; }

		public override object Ejecutar(TablaSimbolos ts,Sesion sesion)
		{
			TablaSimbolos tsLocal = new TablaSimbolos(ts);
			List<ThrowError> errores = new List<ThrowError>();
			int contador = 0;
			object respuesta = condicion.GetValor(tsLocal,sesion);
			if (respuesta != null)
			{
				if (respuesta.GetType() == typeof(ThrowError))
				{
					return respuesta;
				}
				if ((bool)respuesta)
				{
					bool ejecutar = true;
					//repetir y evaluar
					while (contador < For.ITERACIONESMAXIMAS && ejecutar && errores.Count == 0)
					{
						//EJECUTANDO SENTENCIAS ******************************************************************
						foreach (Sentencia sentencia in sentencias)
						{
							respuesta = sentencia.Ejecutar(tsLocal,sesion);
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

								}
								else {
									//return 
									if (errores.Count > 0) return errores;
									return respuesta;
								}
							}
						}
						//******************************************************************************************
						if (ejecutar)
						{
							//EVALUANDO CONDICION
							respuesta = condicion.GetValor(tsLocal,sesion);
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

							if (contador == For.ITERACIONESMAXIMAS)
							{
								//error ciclo infinito
								return new ThrowError(TipoThrow.Exception,
										"Posiblemente existe un ciclo infinito en su código",
									   Linea, Columna);
							}
						}
					}
				}
			}
			if (errores.Count > 0) return errores;
			return null;
		}
	}
}
