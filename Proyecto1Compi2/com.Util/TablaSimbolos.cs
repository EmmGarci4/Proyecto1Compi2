using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.Analisis.Util
{
	class TablaSimbolos:Dictionary<string,Simbolo>
	{
		TablaSimbolos anterior;
		
		public TablaSimbolos():base()
		{
			anterior = null;
		}

		public TablaSimbolos(TablaSimbolos tpadre) : base()
		{
			this.anterior = tpadre;
		}

		public void AgregarSimbolo(Simbolo s)
		{
			try {
				Add(s.Nombre, s);
			}
			catch (Exception) {
				Console.WriteLine("Ya existe");
			}
		}

		public Simbolo GetSimbolo(string nombre)
		{
			if (TryGetValue(nombre, out Simbolo s))
			{
				return s;
			}
			else {
				if (this.anterior!=null) {
					return this.anterior.GetSimbolo(nombre);
				}
			}
			return null;
		}

		public bool ExisteSimbolo(string nombre)
		{
			if (ContainsKey(nombre))
			{
				return true;
			}
			else {
				if (this.anterior!=null) {
					return anterior.ExisteSimbolo(nombre);
				}
			}
			return false;
		}

		public void Mostrar() {
			Console.WriteLine("********** Tabla de simbolos **********");
			foreach (KeyValuePair<string,Simbolo> sim in this)
			{
				Console.WriteLine("Nombre: " + sim.Value.Nombre + " Valor: " + sim.Value.GetDatos());
			}
		}

		internal bool ExisteSimboloEnAmbito(string nombre)
		{
			return ContainsKey(nombre);
		}

		internal void Limpiar()
		{
			Clear();
		}
	}
}
