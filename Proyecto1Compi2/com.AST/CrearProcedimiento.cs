﻿using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class CrearProcedimiento : Sentencia
	{
		String nombre;
		List<Simbolo> parametros;
		List<Sentencia> sentencias;

		public CrearProcedimiento(string nombre, List<Simbolo> parametros, List<Sentencia> sentencias, int linea, int columna) : base(linea, columna)
		{
			this.Nombre = nombre;
			this.Parametros = parametros;
			this.Sentencias = sentencias;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		internal List<Simbolo> Parametros { get => parametros; set => parametros = value; }
		internal List<Sentencia> Sentencias { get => sentencias; set => sentencias = value; }

		public override object Ejecutar(Sesion sesion, TablaSimbolos tb)
		{
			Console.WriteLine("Creando Procedimiento..." + this.nombre);
			foreach (Simbolo cl in this.parametros)
			{
				Console.WriteLine(cl.Nombre);
			}
			return null;
		}
	}
}
