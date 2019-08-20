using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class LlamadaFuncion : Sentencia
	{
		string nombre;
		List<Expresion> parametros;

		public LlamadaFuncion(string nombre, List<Expresion> parametros, int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
			this.parametros = parametros;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		internal List<Expresion> Parametros { get => parametros; set => parametros = value; }

		public override object Ejecutar(Sesion sesion, TablaSimbolos ts)
		{
			Console.WriteLine("Llamando funcion..."+this.nombre);
			return null;
		}

		internal string getLlave(TablaSimbolos ts)
		{
			StringBuilder llave = new StringBuilder();
			llave.Append(nombre + "(");
			int contador = 0;
			foreach (Expresion ex in parametros)
			{
				TipoOperacion t= ex.GetTipo(ts);
				if (t == TipoOperacion.Numero)
				{
					if (ex.GetValor(ts).ToString().Contains("."))
					{
						llave.Append("double");
					}
					else
					{
						llave.Append("int");
					}
				}
				else {
					llave.Append(t.ToString().ToLower());
				}
				if (contador < parametros.Count - 1)
				{
					llave.Append(",");
				}
				contador++;
			}
			llave.Append(")");
			return llave.ToString();
		}
	}
}
