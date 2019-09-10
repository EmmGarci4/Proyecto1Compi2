using System;
using System.Collections.Generic;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	internal class AsignacionColumna : Sentencia
	{
		Acceso izquierda;
		Expresion exp;
		Tabla tabla;
		private Queue<AccesoPar> objetos;
		int posicionDato;

		public AsignacionColumna(Acceso izquierda, Expresion exp, int linea, int columna) : base(linea, columna)
		{
			this.izquierda = izquierda;
			this.exp = exp;
			tabla = null;
			posicionDato = 0;
		}

		internal Acceso Izquierda { get => izquierda; set => izquierda = value; }
		internal Expresion Exp { get => exp; set => exp = value; }

		public override object Ejecutar(TablaSimbolos ts, Sesion sesion)
		{
			if (tabla != null)
			{
				//OBTENIENDO RESPUESTA DE EXPRESION
				object respuesta = exp.GetValor(ts, sesion);
				TipoOperacion tipoRespuesta = exp.GetTipo(ts, sesion);
				if (respuesta != null)
				{
					if (respuesta.GetType() == typeof(ThrowError))
					{
						return respuesta;
					}
				}
				//ASIGNANDO
				TipoObjetoDB tipo = Datos.GetTipoObjetoDB(respuesta);
				object posibleError = Asignar(respuesta, tipo, ts, sesion);
				if (posibleError != null)
				{
					if (posibleError.GetType() == typeof(ThrowError))
					{
						return posibleError;
					}
				}
			}
			return null;
		}

		private object Asignar(object nuevoValor, TipoObjetoDB tipo, TablaSimbolos ts, Sesion sesion)
		{
			if (this.tabla!=null) {
				LlenarCola();
				object respuesta = GetSimbolosApilados(ts, sesion);
				Stack<ParAsignacion> simbolos;
				if (respuesta.GetType() == typeof(ThrowError))
				{
					return respuesta;
				}
				else
				{
					simbolos = (Stack<ParAsignacion>)respuesta;
				}

				//****
				if (simbolos.Count > 0)
				{
					
					ParAsignacion sim = simbolos.Pop();
					//MODIFICAR VALOR EN COLUMNA
					if (sim.Estructura.GetType() == typeof(Columna))
					{
						Columna s = (Columna)sim.Estructura;
						if (Datos.IsTipoCompatibleParaAsignar(s.Tipo, nuevoValor))
						{
							object nuevoDato = Datos.CasteoImplicito(s.Tipo, nuevoValor, ts, sesion, Linea, Columna);
							if (nuevoDato != null)
							{
								if (nuevoDato.GetType() == typeof(ThrowError))
								{
									return nuevoDato;
								}
								s.Datos[posicionDato] = nuevoDato;
							}
						}
						else
						{
							return new ThrowError(Util.TipoThrow.Exception,
								"No se puede asignar el valor",
								Linea, Columna);
						}
					} else
					 //MODIFICAR VALOR EN OBJETO
					 if (sim.Estructura.GetType() == typeof(Objeto))
					{
						Objeto obj = (Objeto)sim.Estructura;
						if (obj.Atributos.ContainsKey(sim.Nombre))
						{
							TipoObjetoDB tipo1 = obj.Plantilla.Atributos[sim.Nombre];
							if (Datos.IsTipoCompatibleParaAsignar(tipo1, nuevoValor))
							{
								obj.Atributos[sim.Nombre] = nuevoValor;
							}
							else {
								return new ThrowError(Util.TipoThrow.Exception,
									"No se puede asignar el valor al atributo '"+sim.Nombre+"'",
									Linea, Columna);
							}
						}
						else
						{
							return new ThrowError(Util.TipoThrow.Exception,
											"El objeto tipo '"+obj.Plantilla.Nombre+"' no contiene al atributo '"+sim.Nombre+"'",
											Linea, Columna);
						}
					}
					else
					{
						return new ThrowError(Util.TipoThrow.Exception,
											"No se puede asignar el valor",
											Linea, Columna);
					}
				}
			}
			return null;
		}

		internal void PasarPosicion(int i)
		{
			this.posicionDato = i;
		}

		private object GetSimbolosApilados(TablaSimbolos ts, Sesion sesion)
		{
			Stack<ParAsignacion> simbolosApilados = new Stack<ParAsignacion>();
			if (objetos.Count > 0)
			{
				AccesoPar valor = objetos.Dequeue();
				switch (valor.Tipo) {
					case TipoAcceso.AccesoArreglo:
						AccesoArreglo acceso = (AccesoArreglo)valor.Value;
						if (this.tabla.ExisteColumna(acceso.Nombre))
						{
							Columna cl = this.tabla.BuscarColumna(acceso.Nombre);
							if (Datos.IsLista(cl.Tipo.Tipo))
							{
								object indice = acceso.Valor.GetValor(ts, sesion);
								if (indice!=null) {
									if (indice.GetType()==typeof(ThrowError)) {
										return indice;
									}
									if (cl.Datos[posicionDato].GetType() == typeof(CollectionListCql))
									{
										if (indice.GetType() != typeof(int))
										{
											CollectionListCql cllection = (CollectionListCql)cl.Datos[posicionDato];
											simbolosApilados.Push(new ParAsignacion(cllection[(int)indice], cllection.TipoDato.ToString()));
											return GetSimbolosApilados(simbolosApilados, ts, sesion);
										}
										else {
											return new ThrowError(TipoThrow.Exception,
											"Solo se puede acceder a un Collection con un inidce tipo entero",
											Linea, Columna);
										}
									}
									else {
										//es un map
									}
									
								}
							}
							else
							{
								return new ThrowError(TipoThrow.Exception,
							"La Columna '" + valor.Value.ToString() + "' no es una lista o map",
							Linea, Columna);
							}
						}
						else
						{
							return new ThrowError(TipoThrow.Exception,
							"La Columna '" + valor.Value.ToString() + "' no existe",
							Linea, Columna);
						}
						break;
					case TipoAcceso.Campo:
						if (this.tabla.ExisteColumna(valor.Value.ToString()))
						{
							Columna cl = this.tabla.BuscarColumna(valor.Value.ToString());
							if (Datos.IsPrimitivo(cl.Tipo.Tipo)||this.objetos.Count==0)
							{
								simbolosApilados.Push(new ParAsignacion(cl, valor.Value.ToString()));
							}
							else {
								simbolosApilados.Push(new ParAsignacion(cl.Datos[posicionDato], valor.Value.ToString()));
							}
							return GetSimbolosApilados(simbolosApilados, ts, sesion);
						}
						else
						{
							return new ThrowError(TipoThrow.Exception,
							"La Columna '" + valor.Value.ToString() + "' no existe",
							Linea, Columna);
						}
					case TipoAcceso.LlamadaFuncion:
					case TipoAcceso.Variable:
						return new ThrowError(TipoThrow.Exception,
						"Se debe especificar la columna a actualizar",
						Linea, Columna);
				}
			}

			return simbolosApilados;
		}

		private object GetSimbolosApilados(Stack<ParAsignacion> simbolosApilados, TablaSimbolos ts, Sesion sesion)
		{
			if (objetos.Count > 0)
			{
				AccesoPar valor = objetos.Dequeue();
				switch (valor.Tipo) {
					case TipoAcceso.AccesoArreglo:
						if (simbolosApilados.Peek().Estructura.GetType()==typeof(CollectionListCql)) {

						
						}
						break;
					case TipoAcceso.Campo:
						if (simbolosApilados.Peek().Estructura.GetType() == typeof(Objeto))
						{
							Objeto obj = (Objeto)simbolosApilados.Peek().Estructura;
							if (obj.Atributos.ContainsKey(valor.Value.ToString())) {
								simbolosApilados.Push(new ParAsignacion(obj, valor.Value.ToString()));
								return GetSimbolosApilados(simbolosApilados,ts,sesion);
							}
							else {
								//error
								return new ThrowError(Util.TipoThrow.Exception,
							"El atributo '"+valor.Value.ToString()+"' no existe en el objeto '"+obj.Plantilla.Nombre+"'",
							Linea, Columna);
							}
						}
						else {
							//error
							
						}
						break;
					case TipoAcceso.LlamadaFuncion:
					case TipoAcceso.Variable:
						return new ThrowError(Util.TipoThrow.Exception,
							"El acceso a una columna de tabla es incorrecta",
							Linea, Columna);
				}
			}
			return simbolosApilados;
		}
		internal void PasarTabla(Tabla miTabla)
		{
			this.tabla = miTabla;
		}

		internal void LimpiarTabla()
		{
			this.tabla = null;
		}

		private void LlenarCola()
		{
			objetos = new Queue<AccesoPar>();
			foreach (AccesoPar acceso in izquierda.Objetos)
			{
				objetos.Enqueue(acceso);
			}
		}
	}

}