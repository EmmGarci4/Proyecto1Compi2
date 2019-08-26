using com.Analisis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class AbrirCursor:Sentencia
	{
		string nombre;

		public AbrirCursor(string nombre, int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
		}

		public string Nombre { get => nombre; set => nombre = value; }

		public override object Ejecutar(TablaSimbolos ts)
		{
			//HACER CONSULTA Y GUARDAR EL RESULTADO
			throw new NotImplementedException();
		}
	}
}
