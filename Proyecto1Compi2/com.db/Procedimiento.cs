using com.Analisis.Util;
using Proyecto1Compi2.com.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class Procedimiento
	{
		string nombre;
		Dictionary<string,TipoDatoDB> parametros;
		Dictionary<string, TipoDatoDB> retornos;
		NodoAST instrucciones;

		public Procedimiento(string nombre, Dictionary<string, TipoDatoDB> parametros, Dictionary<string, TipoDatoDB> retornos,NodoAST inst)
		{
			this.nombre = nombre;
			this.parametros = parametros;
			this.retornos = retornos;
			this.instrucciones = inst;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public Dictionary<string, TipoDatoDB> Parametros { get => parametros; set => parametros = value; }
		public Dictionary<string, TipoDatoDB> Retornos { get => retornos; set => retornos = value; }
		internal NodoAST Instrucciones { get => instrucciones; set => instrucciones = value; }

		public string GetCodigoFuente() {
			return "Codigo";
		}

		internal void Mostrar()
		{
			Console.WriteLine("______________________________________________");
			Console.WriteLine("|	Procedimiento:"+Nombre+"				 |");
			Console.WriteLine("______________________________________________");
			Console.WriteLine("Parametros");
			foreach (KeyValuePair<string,TipoDatoDB> par in parametros) {
				Console.WriteLine("|"+par.Key+"|	|"+par.Value+"|");
			}
			Console.WriteLine("______________________________________________");
			Console.WriteLine("Retornos");
			foreach (KeyValuePair<string, TipoDatoDB> par in retornos)
			{
				Console.WriteLine("|" + par.Key + "|	|" + par.Value + "|");
			}
		}
	}
}
