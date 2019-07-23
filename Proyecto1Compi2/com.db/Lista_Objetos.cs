using com.Analisis.Util;
using Proyecto1Compi2.com.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class Lista_Objetos:List<Objeto>
	{
		string path;

		public string Path { get => path; set => path = value; }

		public Lista_Objetos(string path) {
			this.path = path;
		}

		internal void GenerarArchivo()
		{
			StringBuilder archivoObj = new StringBuilder();
			foreach (Objeto obj in this)
			{
				archivoObj.AppendLine("<Obj>");
				archivoObj.AppendLine("<Nombre>" + obj.Nombre + "</Nombre>");
				//atributos
				archivoObj.AppendLine("<attr>");
				foreach (Simbolo attr in obj.Atributos) {
					archivoObj.AppendLine("<"+attr.TipoDato.ToString().ToLower()+">"+attr.Nombre+ "</" + attr.TipoDato.ToString().ToLower() + ">");

				}
				archivoObj.AppendLine("</attr>");
				archivoObj.AppendLine("</Obj>");
			}

			Console.WriteLine("***********************************");
			Console.WriteLine(archivoObj.ToString());
		}
	}
}
