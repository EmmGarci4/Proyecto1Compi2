using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;

namespace Proyecto1Compi2.com.AST
{
	class ModificadorExp : Expresion
	{
		string variable;
		bool sumar;

		public ModificadorExp(string variable, bool sumar,int linea,int columna):base(linea,columna)
		{
			this.variable = variable;
			this.sumar = sumar;
		}

		protected ModificadorExp(int linea, int columna) : base(linea, columna)
		{
		}

		public string Variable { get => variable; set => variable = value; }
		public bool Sumar { get => sumar; set => sumar = value; }

		public override TipoOperacion GetTipo(TablaSimbolos ts)
		{
			throw new NotImplementedException();
		}

		public override object GetValor(TablaSimbolos ts)
		{
			throw new NotImplementedException();
		}
	}
}
