using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Insertar:Sentencia
	{
		string tabla;
		List<Expresion> valores;
		List<string> columnas;

		public Insertar(string tabla, List<string> columnas, List<Expresion> valores, int linea,int columna):base(linea,columna)
		{
			this.Tabla = tabla;
			this.Valores = valores;
			this.Columnas = columnas;
		}

		public Insertar(string tabla, List<Expresion> valores, int linea, int columna) : base(linea, columna)
		{
			this.Tabla = tabla;
			this.Valores = valores;
			this.Columnas = columnas;
		}

		public string Tabla { get => tabla; set => tabla = value; }
		public List<Expresion> Valores { get => valores; set => valores = value; }
		public List<string> Columnas { get => columnas; set => columnas = value; }

		public override object Ejecutar(Usuario usuario)
		{
			Console.WriteLine("Insertando..."+this.tabla);
			return null;
		}
	}
}
