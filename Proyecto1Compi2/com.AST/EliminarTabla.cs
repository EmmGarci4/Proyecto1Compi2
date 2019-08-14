using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class EliminarTabla:Sentencia
	{
		string nombre;
		bool ifExist;

		public EliminarTabla(string nombre, bool ifExist,int linea,int columna):base(linea,columna)
		{
			this.nombre = nombre;
			this.ifExist = ifExist;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public bool IfExist { get => ifExist; set => ifExist = value; }

		public override object Ejecutar(Usuario usuario)
		{
			Console.WriteLine("Eliminar..."+this.nombre);
			return null;
		}
	}
}
