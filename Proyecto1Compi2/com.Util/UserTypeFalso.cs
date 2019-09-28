using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Util
{
	class UserTypeFalso
	{
		string nombre;
		Dictionary<string, TipoObjetoDB> atributos;
		List<int> lineasNum;
		List<int> columnasNum;

		public UserTypeFalso()
		{
			this.lineasNum = new List<int>();
			this.columnasNum = new List<int>();
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public Dictionary<string, TipoObjetoDB> Atributos { get => atributos; set => atributos = value; }
		public List<int> LineasNum { get => lineasNum; set => lineasNum = value; }
		public List<int> ColumnasNum { get => columnasNum; set => columnasNum = value; }
	}
}
