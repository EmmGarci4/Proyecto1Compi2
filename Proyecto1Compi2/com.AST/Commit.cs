using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Commit : Sentencia
	{
		public Commit(int linea, int columna) : base(linea, columna)
		{
		}

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			Analizador.GenerarArchivos("principal.chison");
			Console.WriteLine("ARCHIVO PRINCIPAL.chison GENERADO");
			return null;
		}
	}
}
