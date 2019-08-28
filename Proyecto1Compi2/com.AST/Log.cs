using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;

namespace Proyecto1Compi2.com.AST
{
	class Log:Sentencia
	{
		Expresion valor;

		public Log(Expresion valor,int linea,int columna):base(linea,columna)
		{
			this.valor = valor;
		}

		internal Expresion Valor { get => valor; set => valor = value; }

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			object respuesta = valor.GetValor(tb,sesion);
			if (respuesta != null)
			{
				if (respuesta.GetType() == typeof(ThrowError))
				{
					return respuesta;
				}
				sesion.Mensajes.Add(respuesta.ToString());
				Form1.MostrarMensajeAUsuario(respuesta.ToString());
			}
			else {
				Form1.MostrarMensajeAUsuario("null");
			}
			return null;
		}
	}
}
