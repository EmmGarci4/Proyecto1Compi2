using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Seleccionar:Sentencia
	{
		List<Expresion> listaAccesos;
		string tabla;
		Where condicion;
		OrderBy order;
		Limit limit;

		public Seleccionar(List<Expresion> listaAccesos, string tabla, int linea,int columna):base(linea,columna)
		{
			this.listaAccesos = listaAccesos;
			this.tabla = tabla;
			this.condicion = null;
			this.order = null;
			this.limit = null;
		}

		public string Tabla { get => tabla; set => tabla = value; }
		internal List<Expresion> ListaAccesos { get => listaAccesos; set => listaAccesos = value; }
		internal Where PropiedadWhere { get => condicion; set => condicion = value; }
		internal OrderBy PropiedadOrderBy { get => order; set => order = value; }
		internal Limit PropiedadLimit { get => limit; set => limit = value; }

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			throw new NotImplementedException();
		}
	}
}
