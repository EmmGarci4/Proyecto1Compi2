using com.Analisis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class EliminarTabla:Sentencia
	{
		TipoObjeto objeto;
		string nombre;

		public EliminarTabla(TipoObjeto objeto, string nombre)
		{
			this.Objeto = objeto;
			this.Nombre = nombre;
		}

		public TipoObjeto Objeto { get => objeto; set => objeto = value; }
		public string Nombre { get => nombre; set => nombre = value; }

		public override object Ejecutar()
		{
			Console.WriteLine("Eliminar..."+this.nombre+":"+this.objeto.ToString());
			return null;
		}
	}
}
