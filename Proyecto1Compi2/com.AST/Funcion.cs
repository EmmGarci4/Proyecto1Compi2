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
	class Funcion:Sentencia
	{
		string nombre;
		Dictionary<string, TipoObjetoDB> parametros;
		TipoObjetoDB tipoRetorno;

		public string Nombre { get => nombre; set => nombre = value; }
		public Dictionary<string, TipoObjetoDB> Parametros { get => parametros; set => parametros = value; }
		public TipoObjetoDB TipoRetorno { get => tipoRetorno; set => tipoRetorno = value; }

		public Funcion(string nombre, Dictionary<string, TipoObjetoDB> parametros, TipoObjetoDB retorno, int linea,int columna):base(linea,columna)
		{
			this.nombre = nombre;
			this.parametros = parametros;
			this.tipoRetorno = retorno;
		}

		public Funcion(string nombre, TipoObjetoDB retorno, int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
			this.tipoRetorno = retorno;
			this.parametros = new Dictionary<string, TipoObjetoDB>();
		}

		public override object Ejecutar(Sesion sesion, TablaSimbolos tb)
		{

			return 1;
		}

		public string GetLlave()
		{
			StringBuilder llave = new StringBuilder();
			llave.Append(Nombre + "(");
			int contador = 0;
			foreach (KeyValuePair<string,TipoObjetoDB> par in this.parametros)
			{
				llave.Append(par.Value.Tipo.ToString());
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
