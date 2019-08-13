namespace Proyecto1Compi2.com.AST
{
	internal class AsignacionColumna
	{
		Acceso izquierda;
		Expresion exp;

		public AsignacionColumna(Acceso izquierda, Expresion exp)
		{
			this.izquierda = izquierda;
			this.exp = exp;
		}

		internal Acceso Izquierda { get => izquierda; set => izquierda = value; }
		internal Expresion Exp { get => exp; set => exp = value; }
	}
}