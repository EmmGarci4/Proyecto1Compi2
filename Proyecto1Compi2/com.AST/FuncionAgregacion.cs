using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class FuncionAgregacion : Sentencia
	{
		string nombre;
		Seleccionar select;

		public FuncionAgregacion(string nombre,Seleccionar select,int linea,int columna):base(linea,columna)
		{
			this.Nombre = nombre;
			this.select = select;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		internal Seleccionar Select { get => select; set => select = value; }

		public override object Ejecutar(Sesion sesion)
		{
			throw new NotImplementedException();
		}
	}
}
