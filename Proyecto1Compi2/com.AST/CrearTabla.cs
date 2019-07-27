using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class CrearTabla : Sentencia
	{
		String nombre;
		List<Columna> columnas;
		
		protected CrearTabla(String tabla,List<Columna> cols) 
		{
			this.nombre = tabla;
			this.columnas = cols;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public List<Columna> Columnas { get => columnas; set => columnas = value; }

		public override object Ejecutar()
		{
			Console.WriteLine("Creando tabla..."+this.nombre);
			foreach (Columna cl in this.columnas) {
				Console.WriteLine(cl.Titulo);
			}
			return null;
		}
	}
}
