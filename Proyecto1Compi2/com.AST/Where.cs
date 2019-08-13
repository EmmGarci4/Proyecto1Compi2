using System.Collections.Generic;
using com.Analisis.Util;

namespace Proyecto1Compi2.com.AST
{
	internal class Where:Expresion
	{
		Condicion condicion;
		Expresion izquierda; //para clausula in
		Expresion expresiones;
		List<Expresion> listaExpresiones;

		public Where(Condicion condicion,int linea,int columna):base(linea,columna)
		{
			this.condicion = condicion;
			this.izquierda = null;
			this.expresiones = null;
			this.listaExpresiones = null;
		}

		public Where(Expresion izquierda, Expresion expresiones, int linea, int columna) : base(linea, columna)
		{
			this.condicion = null;
			this.izquierda = izquierda;
			this.expresiones = expresiones;
			this.listaExpresiones = null;
		}

		public Where(Expresion izquierda, List<Expresion> expresiones, int linea, int columna) : base(linea, columna)
		{
			this.condicion = null;
			this.izquierda = izquierda;
			this.expresiones = null;
			this.listaExpresiones = expresiones;
		}

		internal Condicion Condicion { get => condicion; set => condicion = value; }
		internal Expresion Izquierda { get => izquierda; set => izquierda = value; }
		internal Expresion Expresiones { get => expresiones; set => expresiones = value; }
		internal List<Expresion> ListaExpresiones { get => listaExpresiones; set => listaExpresiones = value; }

		public override TipoOperacion GetTipo(TablaSimbolos ts)
		{
			return TipoOperacion.Booleano;
		}

		public override string GetValor(TablaSimbolos ts)
		{
			throw new System.NotImplementedException();
		}
	}
}