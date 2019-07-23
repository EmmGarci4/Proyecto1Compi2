using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.Analisis.Util
{
	class Simbolo
	{
		string nombre;
		object valor;
		TipoDatoDB tipoDato;
		int linea;
		int columna;


		public string Nombre { get => nombre; set => nombre = value; }
		public object Valor { get => valor; set => valor = value; }
		public TipoDatoDB TipoDato { get => tipoDato; set => tipoDato = value; }
		public int Linea { get => linea; set => linea = value; }
		public int Columna { get => columna; set => columna = value; }


		public Simbolo(string nombre, object valor, TipoDatoDB tipo, int linea, int columna)
		{
			this.Nombre = nombre;
			this.Valor = valor;
			this.TipoDato = tipo;
			this.Linea = linea;
			this.Columna = columna;

		}

		public string GetDatos() {
			return "Nombre: " + this.nombre + " Tipo: " + this.tipoDato + " Linea: " + this.linea + " Columna: " + this.columna;
		}
	}
	}
