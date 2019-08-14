using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Batch:Sentencia
	{
		List<Sentencia> sentencias;

		public Batch(int linea,int columna):base(linea,columna)
		{
			this.sentencias = new List<Sentencia>();
		}

		internal List<Sentencia> Sentencias { get => sentencias; set => sentencias = value; }

		public override object Ejecutar(Sesion sesion)
		{
			throw new NotImplementedException();
		}
	}
}
