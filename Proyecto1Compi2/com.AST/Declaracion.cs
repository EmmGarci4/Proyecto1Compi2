using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Declaracion : Sentencia
	{
		List<string> variables;
		TipoObjetoDB tipo;
		Expresion expresion;

		public Declaracion(List<string> variables, TipoObjetoDB tipo, Expresion expresion, int linea, int columna) : base(linea, columna)
		{
			this.variables = variables;
			this.tipo = tipo;
			this.expresion = expresion;
		}

		public List<string> Variables { get => variables; set => variables = value; }
		public TipoObjetoDB Tipo { get => tipo; set => tipo = value; }
		internal Expresion Expresion { get => expresion; set => expresion = value; }

		public override object Ejecutar(TablaSimbolos ts, Sesion sesion)
		{
			//VALIDANDO NOMBRE DE VARIABLE
			foreach (string variable in variables)
			{
				if (ts.ExisteSimboloEnAmbito(variable))
				{
					return new ThrowError(TipoThrow.Exception,
						"La variable '" + variable + "' ya existe",
						Linea, Columna);
				}
			}
			//VALIDAR TIPO DE DATO DE OBJETO
			//***********VALIDAR LISTAS TAMBIEN
			if (this.Tipo.Tipo == TipoDatoDB.OBJETO)
			{
				object respuestaComprobacion = ExisteUserType(this.tipo, sesion);
				if (respuestaComprobacion.GetType() == typeof(ThrowError))
				{
					return respuestaComprobacion;
				}

				if (!(bool)respuestaComprobacion)
				{
					return new ThrowError(Util.TipoThrow.TypeDontExists,
							"No existe el User Type '" + this.tipo.ToString() + "'",
							Linea, Columna);
				}
			}

			object respuesta = null;
			if (expresion != null)
			{
				//OBTENIENDO RESPUESTA DE EXPRESION
				respuesta = expresion.GetValor(ts, sesion);
				TipoOperacion tipoRespuesta = expresion.GetTipo(ts, sesion);
				if (respuesta != null)
				{
					if (respuesta.GetType() == typeof(ThrowError))
					{
						return respuesta;
					}

					if (this.tipo.Tipo == TipoDatoDB.OBJETO && tipoRespuesta == TipoOperacion.Nulo) {
						object nuevares = GetValorPredeterminado(tipo, sesion, Linea, Columna);
						if (nuevares != null) {
							if (nuevares.GetType() == typeof(ThrowError)) {
								return nuevares;
							}
							respuesta = nuevares;
						}
					} else if (this.tipo.Tipo == TipoDatoDB.DATE && tipoRespuesta == TipoOperacion.Nulo)
					{
						object nuevares = GetValorPredeterminado(tipo, sesion, Linea, Columna);
						if (nuevares != null)
						{
							if (nuevares.GetType() == typeof(ThrowError))
							{
								return nuevares;
							}
							respuesta = nuevares;
						}
					}
					else if (this.tipo.Tipo == TipoDatoDB.TIME && tipoRespuesta == TipoOperacion.Nulo)
					{
						object nuevares = GetValorPredeterminado(tipo, sesion, Linea, Columna);
						if (nuevares != null)
						{
							if (nuevares.GetType() == typeof(ThrowError))
							{
								return nuevares;
							}
							respuesta = nuevares;
						}
					}
				}
			}

			//AGREGANDO A TABLA DDE SIMBOLOS
			int contador = 0;
			foreach (string variable in variables)
			{
				if (contador == variables.Count - 1 && expresion != null && respuesta != null)
				{
					object nuevaRespuesta = Datos.CasteoImplicito(tipo, respuesta,ts,sesion,Linea,Columna);
					if (nuevaRespuesta != null)
					{
						if (nuevaRespuesta.GetType() == typeof(ThrowError))
						{
							return nuevaRespuesta;
						}
						ts.AgregarSimbolo(new Simbolo(variable, nuevaRespuesta, tipo, Linea, Columna));
					}
					
				}
				else
				{
					object valorPre = GetValorPredeterminado(this.tipo, sesion, Linea, Columna);
					if (valorPre!=null) {
						if (valorPre.GetType()==typeof(ThrowError)) {
							return valorPre;
						}
						ts.AgregarSimbolo(new Simbolo(variable,valorPre , tipo, Linea, Columna));

					}
				}
				contador++;
			}
			return null;
		}

		private object ExisteUserType(TipoObjetoDB tipo, Sesion sesion)
		{
			//VALIDANDO BASEDATOS
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				return (db.ExisteUserType(tipo.ToString()));
			}
			else
			{
				return new ThrowError(Util.TipoThrow.UseBDException,
					"No se puede ejecutar la sentencia porque no hay una base de datos seleccionada",
					Linea, Columna);
			}
		}

		public static object GetValorPredeterminado(TipoObjetoDB tipo,Sesion sesion,int Linea,int Columna)
		{
			switch (tipo.Tipo)
			{
				case TipoDatoDB.BOOLEAN:
					return false;
				case TipoDatoDB.DOUBLE:
					return 0.0;
				case TipoDatoDB.COUNTER:
				case TipoDatoDB.INT:
					return 0;
				case TipoDatoDB.STRING:
				case TipoDatoDB.LISTA_OBJETO:
				case TipoDatoDB.LISTA_PRIMITIVO:
				case TipoDatoDB.MAP_OBJETO:
				case TipoDatoDB.MAP_PRIMITIVO:
				case TipoDatoDB.SET_OBJETO:
				case TipoDatoDB.SET_PRIMITIVO:
					return "null";
				case TipoDatoDB.DATE:
				case TipoDatoDB.TIME:
					return new MyDateTime();
				case TipoDatoDB.OBJETO:
					//VALIDANDO BASEDATOS
					if (sesion.DBActual != null)
					{
						BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
						if (db.ExisteUserType(tipo.Nombre))
						{

							//buscar objeto;
							Objeto objeto = new Objeto(db.BuscarUserType(tipo.Nombre),true);
							return objeto;
						}
						else
						{
								return new ThrowError(Util.TipoThrow.TypeDontExists,
							"El user Type '" + tipo.Nombre + "' no existe",
							Linea, Columna);
						}
					}
					else
					{
						return new ThrowError(Util.TipoThrow.UseBDException,
							"No se puede ejecutar la sentencia porque no hay una base de datos seleccionada",
							Linea, Columna);
					}
				default:
					return "null";
			}
		}
	}
}
