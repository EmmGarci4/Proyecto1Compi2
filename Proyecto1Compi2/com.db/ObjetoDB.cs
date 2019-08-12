using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	abstract class ObjetoDB
	{
		int linea;
		int columna;

		protected ObjetoDB(int linea, int columna)
		{
			this.linea = linea;
			this.columna = columna;
		}

		public int Linea { get => linea; set => linea = value; }
		public int Columna { get => columna; set => columna = value; }
	}
}
