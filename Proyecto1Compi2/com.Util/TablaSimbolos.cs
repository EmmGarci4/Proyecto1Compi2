﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.Analisis.Util
{
	class TablaSimbolos:Stack<Simbolo>
	{
		
		public TablaSimbolos():base()
		{

		}

		public TablaSimbolos(TablaSimbolos tpadre) : base()
		{
			while (tpadre.Count>0) {
				Push(tpadre.Pop());
			}
		}

		public void AgregarSimbolo(Simbolo s)
		{
			Push(s);
		}

		public Simbolo GetSimbolo(string nombre)
		{
			foreach (Simbolo s in this)
			{
				if (s.Nombre == nombre)
				{
					return s;
				}
			}
			return null;
		}

		public bool ExisteSimboloEnAmbito(string nombre) {
				foreach (Simbolo s in this)
				{
					if (s.Nombre == nombre)
					{
						return true;
					}
					if (s.Nombre == "%$SEPARADOR$%")
					{
						return false;
					}
				}
				return false;
			}

		public bool ExisteSimbolo(string nombre)
		{
			foreach (Simbolo s in this)
			{
				if (s.Nombre==nombre) {
					return true;
				}
			}
			return false;
		}

		public void Mostrar() {
			Console.WriteLine("********** Tabla de simbolos **********");
			foreach (Simbolo sim in this)
			{
				Console.WriteLine("Nombre: " + sim.Nombre + " Valor: " + sim.GetDatos());
			}
		}

		public Stack<Simbolo> GetSimbolos()
		{
			return this;
		}


		internal void Limpiar()
		{
			Clear();
		}
	}
}
