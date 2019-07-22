using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
namespace com.Analisis
{
	class generadorDOT
	{
		private static int contadorNodos;
		private static StringBuilder codigo;

		public static void GenerarDOT(ParseTreeNode raiz,String path) {
			codigo = new StringBuilder();
			codigo.Append("digraph Arbol{\n");
			codigo.Append("raiz[label=\"" + CorregirCaracteres(raiz.ToString()) + "\"];\n");
			contadorNodos = 1;
			recorrerArbol("raiz",raiz);
			codigo.Append("}");
			//guardando
			using (StreamWriter we = new StreamWriter(path))
			{
				we.Write(codigo);
				we.Close();
			}
		}

		private static void recorrerArbol(String nombrePadre,ParseTreeNode nodo) {
			foreach (ParseTreeNode hijo in nodo.ChildNodes) {
				String nombreNodo = "nodo" + contadorNodos.ToString();
				codigo.Append(nombreNodo + "[label=\"" + CorregirCaracteres(hijo.ToString()) + "\"];\n");
				codigo.Append(nombrePadre + "->" + nombreNodo + ";\n");
				contadorNodos++;
				recorrerArbol(nombreNodo, hijo);
			}
		}


		private static string CorregirCaracteres(string v)
		{
			v = v.Replace("\\", "\\\\");
			v= v.Replace("\"", "\\\"");
			v = v.Replace("<", "\\<");
			v = v.Replace(">", "\\>");
			return v;
		}

	}
}
