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

		public override object Ejecutar(Usuario usuario)
		{
			throw new System.NotImplementedException();
		}
	}
}