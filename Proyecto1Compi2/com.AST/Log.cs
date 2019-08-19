using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		public override object Ejecutar(Sesion sesion, TablaSimbolos tb)
		{
			object respuesta = valor.GetValor(tb);
				if (respuesta.GetType() == typeof(ThrowError))
				{
					return respuesta;
				}
				sesion.Mensajes.Add(respuesta.ToString());
				Form1.MostrarMensajeAUsuario(respuesta.ToString());
			
			return null;
		}
	}
}
