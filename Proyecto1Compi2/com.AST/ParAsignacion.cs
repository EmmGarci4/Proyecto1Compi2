﻿namespace Proyecto1Compi2.com.AST
{
	internal class ParAsignacion
	{

		object estructura;
		object nombre;

		public ParAsignacion(object estructura, object nombre)
		{
			this.estructura = estructura;
			this.nombre = nombre;
		}

		public object Estructura { get => estructura; set => estructura = value; }
		public object Nombre { get => nombre; set => nombre = value; }
	}
}