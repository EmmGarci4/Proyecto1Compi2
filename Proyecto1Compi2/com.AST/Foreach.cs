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
	class Foreach:Sentencia
	{
		List<Parametro> parametros;
		String nombre;
		List<Sentencia> sentencias;

		public Foreach(List<Parametro> parametros, string nombre, List<Sentencia> sentencias,int linea,int columna):base(linea,columna)
		{
			this.parametros = parametros;
			this.nombre = nombre;
			this.sentencias = sentencias;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		internal List<Parametro> Parametros { get => parametros; set => parametros = value; }
		internal List<Sentencia> Sentencias { get => sentencias; set => sentencias = value; }

		public override object Ejecutar(TablaSimbolos ts,Sesion sesion)
		{
			if (ts.ExisteSimbolo(nombre))
			{
				Simbolo s = ts.GetSimbolo(nombre);
				if (s.TipoDato.Tipo == TipoDatoDB.CURSOR)
				{
					Cursor cursor = (Cursor)s.Valor;
					if (cursor.Resultado != null)
					{
						if (parametros.Count == cursor.Resultado.Titulos.Count)
						{
							int contadorFilas = 0;
							bool ejecutar = true;
							if (ejecutar) {
								for (contadorFilas = 0; contadorFilas < cursor.Resultado.Count; contadorFilas++)
								{
									//AGREGANDO PARAMETROS 
									int contadorParametros = 0;
									TablaSimbolos local = new TablaSimbolos(ts);
									List<ThrowError> errores = new List<ThrowError>();
									FilaDatos fila = cursor.Resultado.ElementAt(contadorFilas);
									foreach (Parametro par in this.parametros)
									{
										if (Datos.IsTipoCompatibleParaAsignar(par.Tipo, fila.Datos.ElementAt(contadorParametros).Valor))
										{
											local.AgregarSimbolo(new Simbolo(par.Nombre, fila.Datos.ElementAt(contadorParametros).Valor, par.Tipo, Linea, Columna));
										}
										else
										{
											return new ThrowError(Util.TipoThrow.Exception,
												"No se puede asignar el valor",
												Linea, Columna);
										}
										contadorParametros++;
									}
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
									if (errores.Count > 0) return errores;
									//******************************************************************************************
								}
							}
						}
						else {
							return new ThrowError(TipoThrow.Exception,
						"La cantidad de parámetros no concuerda con el resultado de la consulta",
						Linea, Columna);
						}
					}
					else {
						return new ThrowError(TipoThrow.NullPointerException,
							"El cursor no ha sido abierto ",
							Linea, Columna);
					}
				}
				else
				{
					return new ThrowError(TipoThrow.Exception,
						"La variable '" + nombre + "' no es un cursor",
						Linea, Columna);
				}
			}
			else
			{
				return new ThrowError(TipoThrow.Exception,
						"La variable '" + nombre + "' no existe",
						Linea, Columna);
			}
			
			return null;
		}

	}
}
