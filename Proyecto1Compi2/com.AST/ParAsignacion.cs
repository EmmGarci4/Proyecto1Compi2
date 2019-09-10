namespace Proyecto1Compi2.com.AST
{
	internal class ParAsignacion
	{

		object estructura;
		string nombre;

		public ParAsignacion(object estructura, string nombre)
		{
			this.estructura = estructura;
			this.nombre = nombre;
		}

		public object Estructura { get => estructura; set => estructura = value; }
		public string Nombre { get => nombre; set => nombre = value; }
	}
}