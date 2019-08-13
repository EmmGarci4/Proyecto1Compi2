using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Borrar:Sentencia
	{
		string tabla;
		object algo; //para eliminar de la tabla
					 //puede ser un nombre o un acceso a collection
		Where condicion;

		public Borrar(string tabla,int linea,int columna):base(linea,columna)
		{
			this.tabla = tabla;
			this.algo = null;
			this.condicion = null;
		}

		public Borrar(string tabla, object algo, int linea, int columna) : base(linea, columna)
		{
			this.tabla = tabla;
			this.algo = algo;
			this.condicion = null;
		}
		public Borrar(string tabla,Where where, int linea, int columna) : base(linea, columna)
		{
			this.tabla = tabla;
			this.algo = null;
			this.condicion = where;
		}

		public Borrar(string tabla, object algo, Where where,int linea, int columna) : base(linea, columna)
		{
			this.tabla = tabla;
			this.algo = algo;
			this.condicion = where;
		}


		public string Tabla { get => tabla; set => tabla = value; }
		public object Algo { get => algo; set => algo = value; }
		internal Where Condicion { get => condicion; set => condicion = value; }

		public override object Ejecutar()
		{
			throw new NotImplementedException();
		}
	}
}
