using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Util
{
	class TablaFalsa
	{
		List<Columna> columnas;
		List<int> lineasNum;
		List<int> columnasNum;
		List<FilaDatos> datos;
		string nombre;


		public TablaFalsa()
		{
			this.columnas = null;
			this.datos = null;
			this.nombre = null;
			this.lineasNum = new List<int>();
			this.columnasNum = new List<int>();
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public List<int> LineasNum { get => lineasNum; set => lineasNum = value; }
		public List<int> ColumnasNum { get => columnasNum; set => columnasNum = value; }
		internal List<Columna> Columnas { get => columnas; set => columnas = value; }
		internal List<FilaDatos> Datos { get => datos; set => datos = value; }
	}
}
