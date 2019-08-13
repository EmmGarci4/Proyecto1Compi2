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
		List<object> objetos;
		bool ifExist;

		public CrearTabla(String tabla,List<object> objetos,bool ifexist, int linea, int columna) : base(linea, columna)
		{
			this.nombre = tabla;
			this.objetos = objetos;
			this.ifExist = ifexist;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public List<object> Objetos { get => objetos; set => objetos = value; }
		public bool IfExist { get => ifExist; set => ifExist = value; }

		public override object Ejecutar()
		{
			Console.WriteLine("Creando tabla..."+this.nombre);
			foreach (Columna cl in this.objetos) {
				Console.WriteLine(cl.Nombre);
			}
			return null;
		}
	}
}
