using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;

namespace Proyecto1Compi2.com.AST
{
	class Condicion : Expresion
	{
		TipoOperacion tipoOp;
		Expresion izquierda;
		Expresion derecha;

		public Condicion( Expresion izquierda, Expresion derecha, TipoOperacion tipoOp, int linea, int columna) : base(linea, columna)
		{
			this.tipoOp = tipoOp;
			this.izquierda = izquierda;
			this.derecha = derecha;
		}

		public Condicion(Expresion izquierda, TipoOperacion tipoOp, int linea, int columna) : base(linea, columna)
		{
			this.tipoOp = tipoOp;
			this.izquierda = izquierda;
			this.derecha = null;
		}


		public TipoOperacion TipoOp { get => tipoOp; set => tipoOp = value; }
		internal Expresion Izquierda { get => izquierda; set => izquierda = value; }
		internal Expresion Derecha { get => derecha; set => derecha = value; }

		public override TipoOperacion GetTipo(TablaSimbolos ts)
		{
			throw new NotImplementedException();
		}

		public override string GetValor(TablaSimbolos ts)
		{
			throw new NotImplementedException();
		}
		//VALIDAR TIPO DE LA EXPRESION DEL NOT
	}
}
