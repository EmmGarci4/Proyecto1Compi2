using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;

namespace Proyecto1Compi2.com.AST
{
	class CerrarCursor:Sentencia
	{
		string nombre;

		public CerrarCursor(string nombre,int linea,int columna):base(linea,columna)
		{
			this.nombre = nombre;
		}

		public string Nombre { get => nombre; set => nombre = value; }

		public override object Ejecutar(TablaSimbolos ts)
		{
			throw new NotImplementedException();
		}
	}
}
