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
		TipoDato tipoDato;
		int linea;
		int columna;
		Visibilidad visibilidad;
		int tamaño;

		public string Nombre { get => nombre; set => nombre = value; }
		public object Valor { get => valor; set => valor = value; }
		public TipoDato TipoDato { get => tipoDato; set => tipoDato = value; }
		public int Linea { get => linea; set => linea = value; }
		public int Columna { get => columna; set => columna = value; }
		public Visibilidad Visibilidad { get => visibilidad; set => visibilidad = value; }
		public int Tamaño { get => tamaño; set => tamaño = value; }

		//ARREGLOS
		public Simbolo(string nombre, object valor, TipoDato tipo, int linea, int columna, Visibilidad visibilidad, int tamaño)
		{
			this.Nombre = nombre;
			this.Valor = valor;
			this.TipoDato = tipo;
			this.Linea = linea;
			this.Columna = columna;
			this.Visibilidad = visibilidad;
			this.Tamaño = tamaño;
		}

		//CLASES O SIMBOLOS
		public Simbolo(string nombre, object valor, TipoDato tipo, int linea, int columna, Visibilidad visibilidad)
		{
			this.Nombre = nombre;
			this.Valor = valor;
			this.TipoDato = tipo;
			this.Linea = linea;
			this.Columna = columna;
			this.Visibilidad = visibilidad;
			this.Tamaño = 1;
		}

		public Simbolo() {
			/*
			this.Nombre = "";
			this.Valor = res.Valor;
			this.TipoDato = res.Tipo;
			this.Linea = res.Linea;
			this.Columna = res.Columna;
			this.Visibilidad = Visibilidad.Private;
			this.Tamaño = 1;
			*/
		}

		public string GetDatos() {
			return "Nombre: " + this.nombre + " Tipo: " + this.tipoDato + " Linea: " + this.linea + " Columna: " + this.columna;
		}
	}
	}
