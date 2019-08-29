using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;

namespace Proyecto1Compi2.com.AST
{
	class AccesoFuncion:Sentencia
	{
		Acceso acceso;

		public AccesoFuncion(Acceso acceso,int linea,int columna):base(linea,columna)
		{
			this.acceso = acceso;
		}

		internal Acceso Acceso { get => acceso; set => acceso = value; }

		public override object Ejecutar(TablaSimbolos ts, Sesion sesion)
		{
			object res = acceso.GetValor(ts, sesion);
			if (res.GetType()==typeof(ThrowError)) {
				return res;
			}
			return null;
		}
	}
}
