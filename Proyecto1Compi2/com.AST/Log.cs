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
	class Log : Sentencia
	{
		Expresion valor;

		public Log(Expresion valor, int linea, int columna) : base(linea, columna)
		{
			this.valor = valor;
		}

		internal Expresion Valor { get => valor; set => valor = value; }

		public override object Ejecutar(TablaSimbolos tb, Sesion sesion)
		{
			object respuesta = valor.GetValor(tb, sesion);
			if (respuesta != null)
			{
				if (respuesta.GetType() == typeof(ThrowError))
				{
					return respuesta;
				}
				if (respuesta.GetType() == typeof(Objeto))
				{
					sesion.Mensajes.Add(((Objeto)respuesta).GetLinealizado());
				}
				else if (respuesta.GetType() == typeof(CollectionListCql))
				{
					sesion.Mensajes.Add(((CollectionListCql)respuesta).GetLinealizado());
				}
				else if (respuesta.GetType() == typeof(CollectionMapCql))
				{
					sesion.Mensajes.Add(((CollectionMapCql)respuesta).GetLinealizado());
				}
				else
				{
					sesion.Mensajes.Add(respuesta.ToString());
				}

				//Form1.MostrarMensajeAUsuario(respuesta.ToString());
			}
			else
			{
				sesion.Mensajes.Add("null");
				//Form1.MostrarMensajeAUsuario("null");
			}
			return null;
		}
	}
}
