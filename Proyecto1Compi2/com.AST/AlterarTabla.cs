using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class AlterarTabla:Sentencia
	{
		TipoAccion accion;
		string tabla;
		List<Columna> agregarCols;
		List<string> quitarCols;

		public AlterarTabla(TipoAccion accion, string tabla, List<string> quitarCols)
		{
			this.accion = accion;
			this.tabla = tabla;
			this.agregarCols = null;
			this.quitarCols = quitarCols;
		}

		public AlterarTabla(TipoAccion accion, string tabla, List<Columna> agregarCols)
		{
			this.accion = accion;
			this.tabla = tabla;
			this.agregarCols = agregarCols;
			this.quitarCols = null;
		}

		public TipoAccion Accion { get => accion; set => accion = value; }
		public string Tabla { get => tabla; set => tabla = value; }
		public List<string> QuitarCols { get => quitarCols; set => quitarCols = value; }
		internal List<Columna> AgregarCols { get => agregarCols; set => agregarCols = value; }

		public override object Ejecutar()
		{
			Console.WriteLine("Alterando tabla..."+this.tabla+"->"+this.accion.ToString().ToLower());
			
			return null;
		}
	}
}
