using com.Analisis.Util;
using Proyecto1Compi2.com.db;

namespace Proyecto1Compi2.com.AST
{
	internal class Limit:Sentencia
	{
		Expresion limitante;

		public Limit(Expresion limitante,int linea,int columna):base(linea,columna)
		{
			this.limitante = limitante;
		}

		internal Expresion Limitante { get => limitante; set => limitante = value; }

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			throw new System.NotImplementedException();
		}
	}
}