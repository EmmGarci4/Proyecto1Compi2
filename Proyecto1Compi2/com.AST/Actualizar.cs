using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Actualizar:Sentencia
	{
		List<AsignacionColumna> asignaciones;
		string nombre;
		Where condicion;

		public Actualizar(List<AsignacionColumna> asignaciones, string nombre, Where condicion,int linea,int columna):base(linea,columna)
		{
			this.asignaciones = asignaciones;
			this.nombre = nombre;
			this.condicion = condicion;
		}

		public Actualizar(List<AsignacionColumna> asignaciones, string nombre, int linea, int columna) : base(linea, columna)
		{
			this.asignaciones = asignaciones;
			this.nombre = nombre;
			this.condicion = null;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		internal List<AsignacionColumna> Asignaciones { get => asignaciones; set => asignaciones = value; }
		internal Where Condicion { get => condicion; set => condicion = value; }

		public override object Ejecutar(Sesion sesion, TablaSimbolos tb)
		{
			Console.WriteLine("Actualizando...");
			return null;
		}
	}
}
