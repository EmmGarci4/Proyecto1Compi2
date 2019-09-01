using Proyecto1Compi2.com.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Util
{
	class Info
	{
		Expresion expresion1;
		Expresion expresion2;

		public Info(Expresion expresion1, Expresion expresion2)
		{
			this.expresion1 = expresion1;
			this.expresion2 = expresion2;
		}

		internal Expresion Expresion1 { get => expresion1; set => expresion1 = value; }
		internal Expresion Expresion2 { get => expresion2; set => expresion2 = value; }
	}
}
