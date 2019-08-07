﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class CrearBaseDatos : Sentencia
	{
		string nombre;

		protected CrearBaseDatos(string nombre, int linea, int columna) : base(linea, columna)
		{
			this.Nombre = nombre;
		}

		public string Nombre { get => nombre; set => nombre = value; }

		public override object Ejecutar()
		{
			Console.WriteLine("Creando base de datos..."+this.nombre);
			return null;
		}
	}
}
