using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class AsignacionCasteo:Sentencia
	{
		TipoObjetoDB tipo;
		Expresion derecha;
		private Acceso acceso;
		private TipoObjetoDB tipoObjetoDB;

		public AsignacionCasteo(Acceso izquierda, TipoObjetoDB tipo, Expresion derecha,int linea,int columna):base(linea,columna)
		{
			this.acceso = izquierda;
			this.tipo = tipo;
			this.derecha = derecha;
		}

		public TipoObjetoDB Tipo { get => tipo; set => tipo = value; }
		public TipoObjetoDB TipoObjetoDB { get => tipoObjetoDB; set => tipoObjetoDB = value; }
		internal Expresion Derecha { get => derecha; set => derecha = value; }
		internal Acceso Acceso { get => acceso; set => acceso = value; }

		public override object Ejecutar(Sesion sesion, TablaSimbolos ts)
		{
			object respuesta = derecha.GetValor(ts);
			if (respuesta.GetType()==typeof(ThrowError)) {
				return respuesta;
			}
			TipoObjetoDB tipo = Datos.GetTipoObjetoDB(respuesta);
			object NuevoValor = null;
			if (this.tipo.Tipo == TipoDatoDB.TIME)
			//TIME-STRING
			{
				if (tipo.Tipo == TipoDatoDB.STRING)
				{
					if (DateTime.TryParse(respuesta.ToString().Replace("'",string.Empty),out DateTime tim))
					{
						NuevoValor = new MyDateTime(TipoDatoDB.TIME, tim);
					}
					else {
						return new ThrowError(Util.TipoThrow.Exception,
						"No se puede convertir la cadena a hora",
						Linea, Columna);
					}
				}
				else {
					return new ThrowError(Util.TipoThrow.Exception,
						"No se puede hacer una conversión entre un dato tipo '"+this.tipo.ToString()+"' y uno tipo '"+tipo.ToString()+"'",
						Linea, Columna);
				}
			}
			else if (this.tipo.Tipo == TipoDatoDB.DATE)
			{
				//DATE-STRING 
				if (tipo.Tipo == TipoDatoDB.STRING)
				{
					if (DateTime.TryParse(respuesta.ToString().Replace("'",string.Empty), out DateTime tim))
					{
						NuevoValor = new MyDateTime(TipoDatoDB.DATE, tim);
					}
					else
					{
						return new ThrowError(Util.TipoThrow.Exception,
						"No se puede convertir la cadena a fecha",
						Linea, Columna);
					}
				}
				else
				{
					return new ThrowError(Util.TipoThrow.Exception,
						"No se puede hacer una conversión entre un dato tipo '" + this.tipo.ToString() + "' y uno tipo '" + tipo.ToString() + "'",
						Linea, Columna);
				}
			}
			else if (this.tipo.Tipo == TipoDatoDB.STRING)
			{
				if (tipo.Tipo == TipoDatoDB.DATE)
				{
					//STRING - DATE
					NuevoValor = respuesta.ToString();
				}
				else
				if (tipo.Tipo == TipoDatoDB.TIME)
				{
					//STRING - TIME
					NuevoValor = respuesta.ToString();
				}else
				if (tipo.Tipo == TipoDatoDB.INT)
				{
					//STRING - INT
					NuevoValor = respuesta.ToString();
				}
				else 
				if (tipo.Tipo == TipoDatoDB.DOUBLE)
				{
					//STRING -DOUBLE
					NuevoValor = respuesta.ToString();
				}
				else
				{
					return new ThrowError(Util.TipoThrow.Exception,
						"No se puede hacer una conversión entre un dato tipo '" + this.tipo.ToString() + "' y uno tipo '" + tipo.ToString() + "'",
						Linea, Columna);
				}
			}
			else if (this.tipo.Tipo == TipoDatoDB.DOUBLE)
			{
				if (tipo.Tipo == TipoDatoDB.STRING)
				{
					//DOUBLE - STRING
					if (double.TryParse(respuesta.ToString(), out double val))
					{
						NuevoValor = val;
					}
					else {
						return new ThrowError(Util.TipoThrow.Exception,
							"No se puede convertir la cadena decimal",
							Linea, Columna);
					}
				}
				else
				{
					return new ThrowError(Util.TipoThrow.Exception,
						"No se puede hacer una conversión entre un dato tipo '" + this.tipo.ToString() + "' y uno tipo '" + tipo.ToString() + "'",
						Linea, Columna);
				}
			}
			else if (this.tipo.Tipo == TipoDatoDB.INT)
			{
				
				if (tipo.Tipo == TipoDatoDB.STRING)
				{
					//INT - STRING
					if (double.TryParse(respuesta.ToString(), out double val))
					{
						NuevoValor = (int)val;
					}
					else
					{
						return new ThrowError(Util.TipoThrow.Exception,
							"No se puede convertir la cadena  entero",
							Linea, Columna);
					}
				}
				else
				{
					return new ThrowError(Util.TipoThrow.Exception,
						"No se puede hacer una conversión entre un dato tipo '" + this.tipo.ToString() + "' y uno tipo '" + tipo.ToString() + "'",
						Linea, Columna);
				}
			}
			else {
				return new ThrowError(Util.TipoThrow.Exception,
						"No se puede hacer una conversión entre un dato tipo '" + this.tipo.ToString() + "' y uno tipo '" + tipo.ToString() + "'",
						Linea, Columna);
			}
			//si llega hasta acá, no hay errores y el valor está preparado para asignarse
			return null;
		}
	}
}
