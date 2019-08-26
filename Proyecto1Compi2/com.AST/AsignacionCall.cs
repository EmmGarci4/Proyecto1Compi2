using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class AsignacionCall:Sentencia
	{
		List<Acceso> variables;
		LlamadaProcedimiento llamada;

		public AsignacionCall(List<Acceso> variables, LlamadaProcedimiento llamada,int linea,int columna):base(linea,columna)
		{
			this.variables = variables;
			this.llamada = llamada;
		}

		public List<Acceso> Variables { get => variables; set => variables = value; }
		internal LlamadaProcedimiento Llamada { get => llamada; set => llamada = value; }

		public override object Ejecutar(TablaSimbolos ts)
		{
			object respuesta = llamada.Ejecutar(ts);
			if (respuesta != null)
				if (respuesta.GetType() == typeof(ThrowError))
					return respuesta;
				
			List<object> valores = (List<object>)respuesta;
			//ASIGNANDO
			int indice = 0;
			foreach (Acceso valor in variables)
			{
				object res=valor.Asignar(valores.ElementAt(indice),Datos.GetTipoObjetoDB(valores.ElementAt(indice)),ts);
				if (res != null) return res;
					indice++;
			}
			return null;
		}
	}
}
