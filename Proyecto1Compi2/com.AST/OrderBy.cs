using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using System.Collections.Generic;

namespace Proyecto1Compi2.com.AST
{
	internal class OrderBy:Sentencia
	{
		List<PropOrderBy> propiedades;

		public OrderBy(int linea,int columna):base(linea,columna)
		{
			this.propiedades = propiedades = new List<PropOrderBy>(); ;
		}

		internal List<PropOrderBy> Propiedades { get => propiedades; set => propiedades = value; }

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			throw new System.NotImplementedException();
		}
	}

	internal class PropOrderBy
	{
		Acceso acceso;
		bool isAsc;

		public PropOrderBy(Acceso acceso, bool isAsc)
		{
			this.Acceso = acceso;
			this.IsAsc = isAsc;
		}

		public bool IsAsc { get => isAsc; set => isAsc = value; }
		internal Acceso Acceso { get => acceso; set => acceso = value; }
	}
}