using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;

namespace Proyecto1Compi2.com.AST
{
	class OperacionAsignacion:Sentencia
	{
		Acceso acceso;
		Expresion exp;
		TipoOperacion operacion;

		public OperacionAsignacion(Acceso acceso,TipoOperacion op, Expresion exp,int linea,int columna):base(linea,columna)
		{
			this.operacion = op;
			this.acceso = acceso;
			this.exp = exp;
		}

		public TipoOperacion Operacion { get => operacion; set => operacion = value; }
		internal Acceso Acceso { get => acceso; set => acceso = value; }
		internal Expresion Exp { get => exp; set => exp = value; }

		public override object Ejecutar(Sesion sesion, TablaSimbolos tb)
		{
			object respuesta=acceso.GetValor(tb);

			return null;
		}
	}
}
