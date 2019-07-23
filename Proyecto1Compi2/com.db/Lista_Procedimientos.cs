using com.Analisis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class Lista_Procedimientos :List<Procedimiento>
	{
		string path;

		public string Path { get => path; set => path = value; }

		public Lista_Procedimientos(string path)
		{
			this.path = path;
		}

		internal void GenerarArchivo()
		{
			StringBuilder archivoProc = new StringBuilder();
			foreach (Procedimiento proc in this) {
				archivoProc.AppendLine("<Proc>");
				archivoProc.AppendLine("<Nombre>"+proc.Nombre+"</Nombre>");
				//parametros y codigo
				archivoProc.AppendLine("<params>");
				foreach (Simbolo attr in proc.Parametros)
				{
					archivoProc.AppendLine("<" + attr.TipoDato.ToString().ToLower() + ">" + attr.Nombre + "</" + attr.TipoDato.ToString().ToLower() + ">");

				}
				archivoProc.AppendLine("</params>");
				archivoProc.AppendLine("<src>");
				archivoProc.AppendLine(proc.GetCodigoFuente());
				archivoProc.AppendLine("</src>");

				archivoProc.AppendLine("</Proc>");
			}

			Console.WriteLine("***********************************");
			Console.WriteLine(archivoProc.ToString());
		}
	}
}
