using Proyecto1Compi2.com.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Util
{
	class ProcedimientoFalso
	{
		string nombre;
		List<Parametro> parametros;
		List<Parametro> retornos;
		List<Sentencia> sentencias;
		string instrucciones;
		List<int> lineasNum;
		List<int> columnasNum;
		int linea;
		int columna;

		public ProcedimientoFalso(int linea,int columna)
		{
			this.linea = linea;
			this.columna = columna;
			this.nombre = null;
			this.parametros = new List<Parametro>();
			this.retornos = new List<Parametro>();
			this.sentencias = null;
			this.instrucciones = null;
			this.lineasNum = new List<int>();
			this.columnasNum = new List<int>();
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public string Instrucciones { get => instrucciones; set => instrucciones = value; }
		public List<int> LineasNum { get => lineasNum; set => lineasNum = value; }
		public List<int> ColumnasNum { get => columnasNum; set => columnasNum = value; }
		public int Linea { get => linea; set => linea = value; }
		public int Columna { get => columna; set => columna = value; }
		internal List<Parametro> Parametros { get => parametros; set => parametros = value; }
		internal List<Parametro> Retornos { get => retornos; set => retornos = value; }
		internal List<Sentencia> Sentencias { get => sentencias; set => sentencias = value; }
	}
}
