using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.Analisis.Util
{
	class TablaSimbolos
	{
		private Hashtable simbolos;
		private int nivel;
		private string ambito;

		public TablaSimbolos( int nivel, string ambito)
		{
			this.simbolos = new Hashtable();
			this.nivel = nivel;
			this.ambito = ambito;
		}

		public void AgregarSimbolo(Simbolo s) {
			this.simbolos.Add(s.Nombre, s);
		}

		public Simbolo getSimbolo(string nombre) {
			return (Simbolo)this.simbolos[nombre];
		}

		public bool existeSimbolo(string nombre) {
			return this.simbolos.ContainsKey(nombre);
		}

		public void eliminarSimbolo(String nombre) {
			this.simbolos.Remove(nombre);
		}

		public void mostrar() {
			Console.WriteLine("********** Tabla de simbolos "+ambito+ "**********");
			foreach (string HashKey in simbolos.Keys)
			{
				Console.WriteLine("Key: " + HashKey + " Value: " + ((Simbolo)simbolos[HashKey]).GetDatos());
			}
		}

		public List<Simbolo> GetSimbolos()
		{
			List<Simbolo> sims = new List<Simbolo>();
			foreach (string HashKey in simbolos.Keys)
			{
				sims.Add((Simbolo)simbolos[HashKey]);
			}
			return sims;
		}

		public int Nivel { get => nivel; set => nivel = value; }
		public string Ambito { get => ambito; set => ambito = value; }

		internal void Limpiar()
		{
			this.simbolos.Clear();
		}
	}
}
