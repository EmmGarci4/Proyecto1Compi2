using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;

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

		public override object Ejecutar(TablaSimbolos ts)
		{
			throw new NotImplementedException();
		}
	}
}
